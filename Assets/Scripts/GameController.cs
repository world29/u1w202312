using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

namespace Unity1Week
{
    [System.Serializable]
    public struct ComboPhase
    {
        // このコンボ数以上なら、以下の設定値が採用される
        public float comboCount;

        public float timeWindow; // 猶予時間

        public AudioClip se;

        public float pitch;
    }

    public class GameController : MonoBehaviour, IGameControllerRequests
    {
        [SerializeField]
        private GameplayConfig gameplayConfig;

        [SerializeField]
        private PhaseSetting phaseSetting;

        [SerializeField]
        private PlayableDirector director;

        [SerializeField]
        private GameEvent firstPlatformLanded;

        [SerializeField]
        private bool comboOnlyGood = false;

        [SerializeField]
        private List<ComboPhase> comboPhaseTable = new List<ComboPhase>();

        [SerializeField]
        private AudioClip bgm;

        [SerializeField]
        private FORMAT saveFormat = FORMAT.BINARY;

        [HideInInspector]
        public UnityEvent<float> OnScoreChanged = new UnityEvent<float>();

        [HideInInspector]
        public UnityEvent<int> OnComboChanged = new UnityEvent<int>();

        [HideInInspector]
        public UnityEvent<float> OnComboTimerStoped = new UnityEvent<float>();

        [HideInInspector]
        public UnityEvent<Vector3, float> OnPickupItem = new UnityEvent<Vector3, float>();

        [HideInInspector]
        public UnityEvent<Vector3> OnLandingNear = new UnityEvent<Vector3>();

        [HideInInspector]
        public UnityEvent<Vector3> OnLandingFar = new UnityEvent<Vector3>();

        [HideInInspector]
        public UnityEvent<int> OnPhaseChanged = new UnityEvent<int>();

        // UI を有効化するタイミングを知らせる。
        // タイトルデモを再生する場合とスキップする場合で異なる。
        [HideInInspector]
        public UnityEvent OnPlayerBegin = new UnityEvent();

        [HideInInspector]
        public bool IsPlayerBegin { get; private set; }

        [HideInInspector]
        public float Score => _score;

        [HideInInspector]
        public int Combo => _combo;

        [HideInInspector]
        public int GoodCount => _goodCount;

        [HideInInspector]
        public int MaxCombo => _maxCombo;

        [HideInInspector]
        public int TotalScore => (int)_score + _goodCount + _maxCombo * 2;

        [HideInInspector]
        public float ComboTimeWindow => GetComboTimeWindow();

        [HideInInspector]
        public int FrameRate
        {
            get { return _userSettings.frameRate; }
            set
            {
                _userSettings.frameRate = value;
                Application.targetFrameRate = value;
                Debug.Log($"Application.targetFrameRate = {value}");
            }
        }

        [HideInInspector]
        public float BgmVolume
        {
            get { return _userSettings.bgmVolume; }
            set
            {
                _userSettings.bgmVolume = value;
                SoundManager.SetVolume(value, "BGM");
            }
        }

        [HideInInspector]
        public float SeVolume
        {
            get { return _userSettings.seVolume; }
            set
            {
                _userSettings.seVolume = value;
                SoundManager.SetVolume(value, "SE");
            }
        }

        private float _score;
        private int _combo;
        private float _comboTimer;
        private bool _landing;
        private int _platformCount;

        private int _goodCount;
        private int _maxCombo;

        private int _phase = 0;

        private StorageManager _storageManager = null;
        private UserSettings _userSettings = null;

        void OnEnable()
        {
            BroadcastReceivers.RegisterBroadcastReceiver<IGameControllerRequests>(gameObject);
        }

        void OnDisable()
        {
            BroadcastReceivers.UnregisterBroadcastReceiver<IGameControllerRequests>(gameObject);
        }

        void Awake()
        {
            _storageManager = new StorageManager();
            _userSettings = new UserSettings();

            IsPlayerBegin = false;
        }

        void Start()
        {
            Load();

            _score = gameplayConfig.initScore;
            _combo = gameplayConfig.initCombo;
            if (_combo > 0)
            {
                _comboTimer = ComboTimeWindow;
            }
            _maxCombo = _combo;
            _goodCount = gameplayConfig.initGoodCount;

            _landing = false;

            _platformCount = 0;

            Time.timeScale = 1f;

            if (!gameplayConfig.SkipTimeline)
            {
                SoundManager.StopBgm(1);
                director.Play();
            }
            else
            {
                SoundManager.PlayBgm(bgm, 0);
                NotifyPlayerBegin();
            }
        }

        void Update()
        {
            if (_landing && _comboTimer > 0)
            {
                _comboTimer -= Time.deltaTime;

                // コンボ猶予時間が過ぎたらリセット
                if (_comboTimer <= 0)
                {
                    _comboTimer = 0f;

                    ResetCombo();
                }
            }
        }

        // IGameControllerRequests
        public void AddScore(float scoreToAdd)
        {
            _score += scoreToAdd;

            OnScoreChanged.Invoke(_score);

            UpdatePhase((int)_score);
        }

        public void OnItemPickup(Vector3 itemPosition, float score)
        {
            OnPickupItem.Invoke(itemPosition, score);
        }

        private void UpdatePhase(int newScore)
        {
            var phaseTable = phaseSetting.phaseTable;

            int idx = phaseTable.Count - 1;
            for (; 0 < idx; idx--)
            {
                if (newScore >= phaseTable[idx].score)
                {
                    Debug.Log($"Phase update. {idx}");

                    break;
                }
            }

            ChangePhase(idx);
        }

        public void ChangePhase(int newPhase)
        {
            var prevPhase = _phase;

            _phase = newPhase;

            if (prevPhase != _phase)
            {
                OnPhaseChanged.Invoke(_phase);

                Debug.Log($"Phase changed. {prevPhase} -> {newPhase}");
            }
        }

        private void IncrementCombo()
        {
            ++_combo;

            _maxCombo = Mathf.Max(_maxCombo, _combo);

            {
                AudioClip clip;
                float pitch;
                GetComboSound(out clip, out pitch);
                if (clip != null)
                {
                    SoundManager.PlaySeWithPitch(clip, pitch);
                }

            }

            OnComboChanged.Invoke(_combo);
        }

        private void ResetCombo()
        {
            _combo = 0;

            OnComboChanged.Invoke(_combo);
        }

        public void OnLandedPlatform(Vector3 landingPosition, bool isGood, int landedCount)
        {
            if (landedCount != 1)
            {
                return;
            }

            if (isGood)
            {
                ++_goodCount;

                OnLandingNear.Invoke(landingPosition);
            }
            else
            {
                OnLandingFar.Invoke(landingPosition);
            }

            if (comboOnlyGood && !isGood)
            {
                // コンボ終了
                ResetCombo();
            }
            else if (_comboTimer > 0 || _combo == 0)
            {
                // コンボタイマーが残っていればコンボ継続。タイマーをリセットする
                IncrementCombo();

                _comboTimer = ComboTimeWindow;
            }

            _landing = true;

            // 最初のプラットフォームならイベントを発行する
            if (_platformCount == 0)
            {
                //hack: 現状 BGM 再生のトリガーなのでむりやり
                if (!gameplayConfig.SkipTimeline)
                {
                    firstPlatformLanded.Raise();
                    NotifyPlayerBegin();
                }
            }

            ++_platformCount;
        }

        public void OnLeftPlatform()
        {
            _landing = false;

            OnComboTimerStoped.Invoke(_comboTimer);
        }

        public void Retry()
        {
            gameplayConfig.SkipTimeline = true;

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void BackToTitle()
        {
            gameplayConfig.SkipTimeline = false;

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Pause()
        {
            Time.timeScale = 0f;
        }

        public void Resume()
        {
            Time.timeScale = 1f;
        }

        public void Save()
        {
            Debug.Assert(_userSettings != null);

            _userSettings.format = saveFormat;
            _storageManager.Save(_userSettings, IOHandler, false);
        }

        public void Load()
        {
            Debug.Assert(_userSettings != null);

            _storageManager.Load(_userSettings, IOHandler, false);
        }

        private void NotifyPlayerBegin()
        {
            IsPlayerBegin = true;
            OnPlayerBegin.Invoke();
        }

        private void IOHandler(IO_RESULT ret, ref DataInfo dataInfo)
        {
            if (ret == IO_RESULT.LOAD_SUCCESS)
            {
                _userSettings = dataInfo.serializer as UserSettings;

                OnUserSettingsLoaded(_userSettings);
            }

            if (ret == IO_RESULT.LOAD_FAILED)
            {
                Debug.Log($"load failed.");
            }

            if (ret == IO_RESULT.SAVE_FAILED)
            {
                Debug.Log($"save failed.");
            }
        }

        private void OnUserSettingsLoaded(UserSettings userSettings)
        {
            Application.targetFrameRate = userSettings.frameRate;

            SoundManager.SetVolume(userSettings.bgmVolume, "BGM");
            SoundManager.SetVolume(userSettings.seVolume, "SE");
        }

        private void GetComboSound(out AudioClip clip, out float pitch)
        {
            int idx = comboPhaseTable.Count - 1;
            for (; 0 < idx; idx--)
            {
                if (Combo > comboPhaseTable[idx].comboCount)
                {
                    break;
                }
            }

            clip = comboPhaseTable[idx].se;
            pitch = comboPhaseTable[idx].pitch;
        }

        private float GetComboTimeWindow()
        {
            int idx = comboPhaseTable.Count - 1;
            for (; 0 < idx; idx--)
            {
                if (Combo > comboPhaseTable[idx].comboCount)
                {
                    break;
                }
            }

            return comboPhaseTable[idx].timeWindow;
        }

        public void SetVolume(float volume)
        {
            SoundManager.SetVolume(volume);
        }

        public void OpenDialogOption()
        {
            UISceneLoader.LoadUIScene("OptionScene");
        }
    }
}
