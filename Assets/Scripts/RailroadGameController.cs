using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace u1w202312
{
    public enum GameState { Title, Gameplay, Result, None }

    public class RailroadGameController : MonoBehaviour, IRailroadGameControllerRequests
    {
        [SerializeField]
        public PathFollower2D pathFollower;

        [SerializeField]
        public List<PathFollower2DFollower> cars;

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

        [SerializeField]
        private RailroadSpawnerTitle railroadSpawnerTitle;

        [SerializeField]
        private RailroadSpawner railroadSpawnerGameplay;

        [HideInInspector]
        public float DistanceTravelled { get { return _totalDistanceTravelled; } }

        [HideInInspector]
        public float Fuel { get { return _currentFuel; } }

        [HideInInspector]
        public UnityEvent OnPlayerDied;

        [HideInInspector]
        public GameState CurrentGameState
        {
            get { return _gameState; }
        }

        private float _totalDistanceTravelled;

        private float _currentFuel;

        private GameState _gameState;
        private GameState _nextGameState;

        private void SetupCarsOffset()
        {
            Debug.Log("GameController.SetupCarsOffset");

            float offset = 0f;
            for (int i = 0; i < cars.Count; ++i)
            {
                PathFollower2DFollower car = cars[i];

                // スプライトサイズからオフセットを自動算出する
                SpriteRenderer previousSpriteRenderer = null;
                if (i == 0)
                {
                    previousSpriteRenderer = pathFollower.GetComponentInChildren<SpriteRenderer>();
                }
                else
                {
                    previousSpriteRenderer = cars[i - 1].GetComponentInChildren<SpriteRenderer>();
                }

                SpriteRenderer thisSpriteRenderer = car.GetComponentInChildren<SpriteRenderer>();

                offset += (previousSpriteRenderer.sprite.bounds.size.x + thisSpriteRenderer.sprite.bounds.size.x) * 0.5f;

                car.SetOffset(-offset + pathFollower.Offset);
            }

        }

        private void Start()
        {
            Debug.Assert(pathFollower != null);

            _gameState = GameState.Title;

            pathFollower.speed = initialSpeed;

            _totalDistanceTravelled = 0;
            _currentFuel = initialFuel;

            SetupCarsOffset();
        }

        private void Update()
        {
            if (_nextGameState != GameState.None)
            {
                _gameState = _nextGameState;
                _nextGameState = GameState.None;
            }

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

        public void SetNextGameState(GameState nextState)
        {
            _nextGameState = nextState;

            // タイトルからゲームに遷移するときに、線路のスポーナーを切り替える
            if (_nextGameState == GameState.Gameplay)
            {
                railroadSpawnerTitle.enabled = false;
                railroadSpawnerGameplay.enabled = true;

                // 次の線路生成位置を引き継ぐ
                railroadSpawnerGameplay._spawnPosition = railroadSpawnerTitle._spawnPosition;
            }
        }

        public void SetNextGameStateToGameplay()
        {
            SetNextGameState(GameState.Gameplay);
        }

        // 走行距離
        // フレームごとに PathFollower2D から呼び出される
        public void OnTravelled(float distanceThisFrame)
        {
            if (_gameState != GameState.Gameplay)
            {
                return;
            }

            if (distanceThisFrame > 0f)
            {
                _totalDistanceTravelled += distanceThisFrame;

                // 走った分だけ燃料を消費する
                if (_currentFuel > 0f)
                {
                    var fuelConsumed = distanceThisFrame * fuelConsumptionPerMeter;
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
