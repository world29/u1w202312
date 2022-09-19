using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Unity1Week
{
    public enum IO_RESULT
    {
        NONE = 0,
        SAVE_SUCCESS = 1,
        SAVE_FAILED = -1,
        LOAD_SUCCESS = 10,
        LOAD_FAILED = -10,
    }

    public struct DataInfo
    {
        public ISerializer serializer;
        public string filePath;
        public FinishHandler finishHandler;
        public bool async;
    }

    public delegate void FinishHandler(IO_RESULT rESULT, ref DataInfo dataInfo);

    public sealed class StorageManager
    {
        private static readonly string BACKUP_KEY = ".dup";

        private System.Threading.WaitCallback saveThreadHandler = null;
        private System.Threading.WaitCallback loadThreadHandler = null;

        private BinaryFormatter binaryFormatter = null;

        public StorageManager()
        {
            this.saveThreadHandler = new System.Threading.WaitCallback(this.SaveThreadMain);
            this.loadThreadHandler = new System.Threading.WaitCallback(this.LoadThreadMain);

            this.binaryFormatter = new BinaryFormatter();

#if UNITY_IOS
            System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
        }

        public void Save(ISerializer saveInterface, FinishHandler finishHandler, bool async = true)
        {
            DataInfo dataInfo = new DataInfo();
            dataInfo.serializer = saveInterface.Clone();
            dataInfo.filePath = Application.persistentDataPath + saveInterface.filePath;
            dataInfo.finishHandler = finishHandler;
            dataInfo.async = async;

            if (async)
            {
                //todo:
                Debug.Assert(false);
            }
            else
            {
                SaveThreadMain(dataInfo);
            }
        }

        public void Load(ISerializer loadInterface, FinishHandler finishHandler, bool async = true)
        {
            DataInfo dataInfo = new DataInfo();
            dataInfo.serializer = loadInterface;
            dataInfo.filePath = Application.persistentDataPath + loadInterface.filePath;
            dataInfo.finishHandler = finishHandler;
            dataInfo.async = async;

            if (!File.Exists(dataInfo.filePath))
            {
                FinishAccessing(IO_RESULT.NONE, ref dataInfo);
                return;
            }

            if (async)
            {
                //todo:
                Debug.Assert(false);
            }
            else
            {
                LoadThreadMain(dataInfo);
            }
        }

        private void SaveThreadMain(object state)
        {
            DataInfo dataInfo = (DataInfo)state;
            int size = 0;
            byte[] binary = null;

            using (MemoryStream inMs = new MemoryStream())
            {
                switch (dataInfo.serializer.format)
                {
                    case FORMAT.BINARY:
                        this.binaryFormatter.Serialize(inMs, dataInfo.serializer);
                        break;
                    case FORMAT.JSON:
                        using (BinaryWriter bw = new BinaryWriter(inMs))
                        {
                            string json = JsonUtility.ToJson(dataInfo.serializer, true);
                            bw.Write(json);
                        }
                        break;
                }

                binary = inMs.ToArray();
                size = binary.Length;
            }

            using (FileStream outFs = File.Create(dataInfo.filePath))
            {
                using (BinaryWriter writer = new BinaryWriter(outFs))
                {
                    writer.Write(dataInfo.serializer.magic);
                    writer.Write(dataInfo.serializer.version);
                    writer.Write(size);
                    //writer.Write(hash);
                    writer.Write(dataInfo.serializer.encrypt);
                    writer.Write((int)dataInfo.serializer.format);

                    if (dataInfo.serializer.encrypt)
                    {
                        //todo:
                        Debug.Assert(false);
                        //EncryptFile(outFs, binary, size);
                    }
                    else
                    {
                        writer.Write(binary);
                    }
                }
            }

            this.FinishAccessing(IO_RESULT.SAVE_SUCCESS, ref dataInfo);
        }

        private void LoadThreadMain(object state)
        {
            DataInfo dataInfo = (DataInfo)state;
            int version = -1;
            int size = 0;
            byte[] binary;
            bool encrypt = dataInfo.serializer.encrypt;
            FORMAT format = dataInfo.serializer.format;

            // todo: 他のリクエスト消化待ち

            using (FileStream inFs = File.OpenRead(dataInfo.filePath))
            {
                using (BinaryReader reader = new BinaryReader(inFs))
                {
                    string magic = reader.ReadString();
                    if (magic != dataInfo.serializer.magic)
                    {
                        // マジック不一致
                        this.FinishAccessing(IO_RESULT.LOAD_FAILED, ref dataInfo);
                        return;
                    }

                    version = reader.ReadInt32();
                    size = reader.ReadInt32();
                    //hash = reader.ReadBytes(HASH_SIZE);
                    binary = new byte[size];
                    encrypt = reader.ReadBoolean();
                    format = (FORMAT)reader.ReadInt32();

                    if (encrypt)
                    {
                        //todo:
                        Debug.Assert(false);
                        //this.DecryptFile(inFs, binary, size);
                    }
                    else
                    {
                        inFs.Read(binary, 0, size);
                    }
                }
            }

            //todo: ハッシュチェック

            using (MemoryStream outMs = new MemoryStream(binary))
            {
                switch (format)
                {
                    case FORMAT.BINARY:
                        dataInfo.serializer = this.binaryFormatter.Deserialize(outMs) as ISerializer;
                        break;
                    case FORMAT.JSON:
                        using (BinaryReader br = new BinaryReader(outMs))
                        {
                            string json = br.ReadString();
                            System.Type type = dataInfo.serializer.type;
                            dataInfo.serializer = JsonUtility.FromJson(json, type) as ISerializer;
                        }
                        break;
                }
            }

            int nowVersion = dataInfo.serializer.version;
            if (nowVersion > version)
            {
                dataInfo.serializer.UpdateVersion(version);
            }
            else if (nowVersion < version)
            {
                this.FinishAccessing(IO_RESULT.LOAD_FAILED, ref dataInfo);
                return;
            }

            this.FinishAccessing(IO_RESULT.LOAD_SUCCESS, ref dataInfo);
        }

        private void FinishAccessing(IO_RESULT ret, ref DataInfo dataInfo)
        {
            switch (ret)
            {
                case IO_RESULT.LOAD_FAILED:
                    if (!dataInfo.filePath.Contains(BACKUP_KEY))
                    {
                        string backupPath = dataInfo.filePath + BACKUP_KEY;

                        if (File.Exists(backupPath))
                        {
                            //todo: this.isAccessing = false;

                            dataInfo.filePath = backupPath;
                            if (dataInfo.async)
                            {
                                //todo:
                                Debug.Assert(false);
                            }
                            else
                            {
                                this.LoadThreadMain(dataInfo);
                            }
                            return;
                        }
                    }
                    break;
                case IO_RESULT.SAVE_SUCCESS:
                    // バックアップ生成
                    if (dataInfo.serializer.backup)
                    {
                        File.Copy(dataInfo.filePath, dataInfo.filePath + BACKUP_KEY, true);
                    }

#if UNITY_EDITOR
                    Debug.Log($"save succes. {dataInfo.filePath}");
#endif
                    break;
            }

            if (dataInfo.finishHandler != null)
            {
                dataInfo.finishHandler(ret, ref dataInfo);
            }

            //todo: 処理の終了通知
        }
    }
}
