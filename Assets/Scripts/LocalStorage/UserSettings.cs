using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    [System.Serializable]
    public sealed class UserSettings : ISerializer
    {
        public string magic { get { return "UserSettings_220919"; } }
        public int version { get { return 1; } }
        public string filePath { get { return "/UserSettings"; } }
        public FORMAT format { get { return _format; } set { _format = value; } }
        public System.Type type { get { return typeof(UserSettings); } }
        public bool encrypt { get { return false; } }
        public bool backup { get { return false; } }

        [System.NonSerialized]
        private FORMAT _format = FORMAT.BINARY;

        // ユーザー設定
        public int frameRate = 60;
        public float bgmVolume = 1.0f;
        public float seVolume = 1.0f;

        public bool UpdateVersion(int oldVersion)
        {
            return true;
        }

        public ISerializer Clone()
        {
            return this.MemberwiseClone() as ISerializer;
        }

        public void Clear()
        {
            _format = FORMAT.BINARY;

            frameRate = 60;
            bgmVolume = 1.0f;
            seVolume = 1.0f;
        }
    }
}
