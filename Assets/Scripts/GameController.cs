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
    }

    public class GameController : MonoBehaviour, IGameControllerRequests
    {
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
    }
}
