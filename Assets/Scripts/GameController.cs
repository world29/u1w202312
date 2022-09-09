using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
        private GameEvent firstPlatformLanded;

        [SerializeField]
        private float distanceThreshold = 0.2f;

        [SerializeField]
        private List<ComboPhase> comboPhaseTable = new List<ComboPhase>();

        [HideInInspector]
        public UnityEvent<float> OnScoreChanged = new UnityEvent<float>();

        [HideInInspector]
        public UnityEvent<int> OnComboChanged = new UnityEvent<int>();

        [HideInInspector]
        public UnityEvent<float> OnComboTimerStoped = new UnityEvent<float>();

        [HideInInspector]
        public UnityEvent<Vector3> OnLandingNear = new UnityEvent<Vector3>();

        [HideInInspector]
        public UnityEvent<Vector3> OnLandingFar = new UnityEvent<Vector3>();

        [HideInInspector]
        public float Score => _score;

        [HideInInspector]
        public int Combo => _combo;

        [HideInInspector]
        public float ComboTimeWindow => GetComboTimeWindow();

        private float _score;
        private int _combo;
        private float _comboTimer;
        private bool _landing;
        private int _platformCount;

        void OnEnable()
        {
            BroadcastReceivers.RegisterBroadcastReceiver<IGameControllerRequests>(gameObject);
        }

        void OnDisable()
        {
            BroadcastReceivers.UnregisterBroadcastReceiver<IGameControllerRequests>(gameObject);
        }

        void Start()
        {
            _score = 0;

            _landing = false;

            _platformCount = 0;

            Time.timeScale = 1f;
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
        }

        private void IncrementCombo()
        {
            ++_combo;

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

        public void OnLandedPlatform(Vector3 landingPosition, float distance, int landedCount)
        {
            if (landedCount != 1)
            {
                return;
            }

            if (distance <= distanceThreshold)
            {
                OnLandingNear.Invoke(landingPosition);
            }
            else
            {
                OnLandingFar.Invoke(landingPosition);
            }

            // コンボタイマーが残っていればコンボ継続。タイマーをリセットする
            if (_comboTimer > 0 || _combo == 0)
            {
                IncrementCombo();

                _comboTimer = ComboTimeWindow;
            }

            _landing = true;

            // 最初のプラットフォームならイベントを発行する
            if (_platformCount == 0)
            {
                firstPlatformLanded.Raise();
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void BackToTitle()
        {
            //todo: タイトルシーンをロードする
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

        public void DamageToPlayer(int damageAmount)
        {
            // game over
            Pause();
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
    }
}
