using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace u1w202312
{
    public enum GameState { Title, Gameplay, Result }

    public class RailroadGameController : MonoBehaviour, IRailroadGameControllerRequests
    {
        [SerializeField]
        public PathFollower2D pathFollower;

        [SerializeField]
        public float initialFuel = 30f;

        // １メートルあたりの燃料消費量
        [SerializeField]
        public float fuelConsumptionPerMeter = 0.2f;

        // 残り時間の増加量
        [SerializeField]
        public float fuelIncreasingSmall = 5f;

        // 残り時間の増加量
        [SerializeField]
        public float fuelIncreasingLarge = 10f;

        // 障害物にあたったときの燃料の減る量
        [SerializeField]
        public float fuelDecreasing = 5f;

        [SerializeField]
        public float initialSpeed = 3f;

        [SerializeField]
        public float speedUpFactor = 0.1f;

        [SerializeField]
        public float maxSpeed = 8f;

        [HideInInspector]
        public float DistanceTravelled { get { return _distanceTravelled; } }

        [HideInInspector]
        public float Fuel { get { return _currentFuel; } }

        [HideInInspector]
        public UnityEvent OnPlayerDied;

        [HideInInspector]
        public GameState CurrentGameState
        {
            get { return _gameState; }
        }

        private float _distanceTravelled;

        private float _currentFuel;

        private GameState _gameState;

        private void Start()
        {
            Debug.Assert(pathFollower != null);

            _gameState = GameState.Title;

            pathFollower.speed = initialSpeed;

            _distanceTravelled = 0;
            _currentFuel = initialFuel;
        }

        private void Update()
        {
            if (_gameState == GameState.Gameplay)
            {
                // ゲームオーバー
                if (_currentFuel <= 0f)
                {
                    _gameState = GameState.Result;

                    pathFollower.enabled = false;

                    OnPlayerDied.Invoke();
                }
            }
        }

        void OnEnable()
        {
            //todo: 名前空間を変更する
            Unity1Week.BroadcastReceivers.RegisterBroadcastReceiver<IRailroadGameControllerRequests>(gameObject);
        }

        void OnDisable()
        {
            Unity1Week.BroadcastReceivers.UnregisterBroadcastReceiver<IRailroadGameControllerRequests>(gameObject);
        }

        // 走行距離更新
        // PathFollower2D から呼び出される
        public void OnUpdateDistanceTravelled(float distanceTravelled)
        {
            if (_gameState == GameState.Gameplay)
            {
                var diff = distanceTravelled - _distanceTravelled;
                Debug.Assert(diff >= 0f);
                if (diff == 0)
                {
                    return;
                }
                _distanceTravelled = distanceTravelled;

                // 走った分だけ燃料を消費する
                if (_currentFuel > 0f)
                {
                    var fuelConsumed = diff * fuelConsumptionPerMeter;
                    _currentFuel -= fuelConsumed;

                    if (_currentFuel < 0f)
                    {
                        _currentFuel = 0f;
                    }
                }
            }
        }

        // アイテムを拾った
        public void OnItemPickup(Vector3 itemPosition, ItemType itemType)
        {
            // 速度アップはクリスタルの大きさに関わらず一定とする
            var newSpeed = pathFollower.speed * (1f + speedUpFactor);
            pathFollower.speed = Mathf.Min(newSpeed, maxSpeed);

            // 燃料の増える量は大きさで変わる
            if (itemType == ItemType.Small)
            {
                _currentFuel += fuelIncreasingSmall;
            }
            else
            {
                _currentFuel += fuelIncreasingLarge;
            }

            if (_currentFuel > initialFuel)
            {
                _currentFuel = initialFuel;
            }
        }

        public void OnObstacleHit(Vector3 obstaclePosition)
        {
            // 燃料を減らす
            // 列車の速度を落とす？
            _currentFuel -= fuelDecreasing;

            pathFollower.speed = initialSpeed;
        }

        // リトライ
        public void Retry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // タイトルへ戻る
        public void BackToTitle()
        {
            Debug.Log("Back to Title");
        }
    }
}
