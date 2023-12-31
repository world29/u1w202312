﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace u1w202312
{
    public enum GameState { Title, Gameplay, Result, None }

    public class RailroadGameController : MonoBehaviour, IRailroadGameControllerRequests
    {
        [SerializeField]
        public PathFollower2D pathFollower;
        public AudioSource Main_BGM;
        public AudioSource GameOver_BGM;

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
        public float speedDownFactor = 0.25f;

        [SerializeField]
        public float maxSpeed = 8f;

        [SerializeField]
        private RailroadSpawnerTitle railroadSpawnerTitle;

        [SerializeField]
        private RailroadSpawner railroadSpawnerGameplay;

        [SerializeField]
        public float eventTriggerScore = 100f;

        // イベントオブジェクトを生成してから、デモが始まるまでに待機する距離
        [SerializeField]
        private float eventPreparedToReadyDistance = 15f;

        [SerializeField]
        private CanvasGroup gameplayCanvasGroup;

        [SerializeField]
        private GameObject switchPanel;

        // スコアが規定値を超えているか
        [HideInInspector]
        public bool IsEventPrepareReady { get { return _totalDistanceTravelled > eventTriggerScore; } }

        // イベントの準備ができたか
        // = スコアが規定値を超え、線路上にイベントオブジェクトが生成された
        [HideInInspector]
        public bool IsEventReady { get { return _isEventReady; } }

        private bool _isEventReady;

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

        [HideInInspector]
        public HumanType SelectedHumanType { get; private set; }

        [HideInInspector]
        public int SelectedHumanCount { get; private set; }

        private float _totalDistanceTravelled;

        private float _currentFuel;

        private GameState _gameState;
        private GameState _nextGameState;
        double FadeDeltaTime = 0;

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

            pathFollower.SetSpeed(initialSpeed);

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

                    // 減速する
                    DOVirtual.Float(pathFollower.speed, 0f, 2f, value => pathFollower.SetSpeed(value));
                    //pathFollower.enabled = false;

                    OnPlayerDied.Invoke();

                    //Fadeout
                    FadeDeltaTime += Time.deltaTime;
                    var audioSource = pathFollower.GetComponent<AudioSource>();
                    audioSource.volume = 0;
                    Main_BGM.volume = 0;
                    DOVirtual.DelayedCall(1.5f, ()=>GameOver_BGM.Play());
                    
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

        public void SelectHuman(HumanType type, int count)
        {
            SelectedHumanCount = count;
            SelectedHumanType = type;
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

                // オブジェクトが生成されてから数秒後にレディにする
                railroadSpawnerGameplay.onEventPrepared += () => StartCoroutine(EventReadyCoroutine());
            }
        }

        private IEnumerator EventReadyCoroutine()
        {
            // 一定距離走ったらレディにする
            float start = _totalDistanceTravelled;

            while ((_totalDistanceTravelled - start) < eventPreparedToReadyDistance)
            {
                yield return null;
            }

            _isEventReady = true;
        }

        public void SetNextGameStateToGameplay()
        {
            SetNextGameState(GameState.Gameplay);

            // ハードコード
            DOVirtual.DelayedCall(.5f, () =>
            {
                gameplayCanvasGroup.gameObject.SetActive(true);
                gameplayCanvasGroup.DOFade(1f, .5f);
            });
            DOVirtual.DelayedCall(1f, () =>
            {
                switchPanel.SetActive(true);
            });
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
            pathFollower.SetSpeed(Mathf.Min(newSpeed, maxSpeed));

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

            var speed = pathFollower.speed / (1f + speedDownFactor);
            speed = Mathf.Max(speed, initialSpeed);

            pathFollower.SetSpeed(speed);
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


        [ContextMenu("Pause")]
        public void Pause()
        {
            if (Application.isPlaying)
            {
                Debug.Log("Pause");

                //Time.timeScale = 0f;
                pathFollower.SetSpeed(0f);
            }
        }

        [ContextMenu("Resume")]
        public void Resume()
        {
            if (Application.isPlaying)
            {
                Debug.Log("Resume");

                //Time.timeScale = 1f;
                pathFollower.SetSpeed(initialSpeed);
            }
        }
    }
}
