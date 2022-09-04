using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Unity1Week
{
    public class GameController : MonoBehaviour, IGameControllerRequests
    {
        [HideInInspector]
        public UnityEvent<float> OnScoreChanged = new UnityEvent<float>();

        [HideInInspector]
        public float Score => _score;

        private float _score;

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

            Time.timeScale = 1f;
        }

        // IGameControllerRequests
        public void AddScore(float scoreToAdd)
        {
            _score += scoreToAdd;

            OnScoreChanged.Invoke(_score);
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
    }
}
