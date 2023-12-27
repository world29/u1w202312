using UnityEngine;
using UnityEngine.Events;
using PathCreation;

using System.Collections;
using System.Collections.Generic;

namespace u1w202312
{
    // 線路を生成して、列車が走り続けられるようにする
    // todo: 線路が見切れたら削除する
    public class RailroadSpawner : MonoBehaviour
    {
        [SerializeField] GameObject railroadPrefabBranch;

        [SerializeField] PathFollower2D pathFollower2D;

        [SerializeField] float spawnIntervalX = 30;

        [SerializeField] GameObject pickupPrefabSmall; // Small
        [SerializeField] GameObject pickupPrefabLarge; // Large
        [SerializeField] GameObject prefabGirl; // 
        [SerializeField] GameObject prefabOld; // 

        // 大きなクリスタルがスポーンする確率 [0f, 1f]
        [SerializeField] float pickupPrefabLargePercent;

        [SerializeField] GameObject obstaclePrefab;

        [HideInInspector]
        public Vector3 _spawnPosition; // GameController からアクセスする

        [HideInInspector]
        public UnityAction onEventPrepared;

        private bool _eventObjectsSpawned;

        private List<PathCreator> _pathCreators = new List<PathCreator>();

        private RailroadGameController _controller;

        private void OnEnable()
        {
            Debug.Log("RailroadSpawnerTitle.OnEnable");

            if (pathFollower2D)
            {
                pathFollower2D.onEnterPath += OnTrainEnterPath;
            }
        }

        private void OnDisable()
        {
            Debug.Log("RailroadSpawnerTitle.OnDisable");

            if (pathFollower2D)
            {
                pathFollower2D.onEnterPath += OnTrainEnterPath;
            }
        }

        private void Start()
        {
            var go = GameObject.FindGameObjectWithTag("GameController");
            Debug.Assert(go != null);
            go.TryGetComponent<RailroadGameController>(out _controller);
            Debug.Assert(_controller != null);

            // GameController から設定される
            //_spawnPosition = Vector3.zero;

            // 現在列車が乗っている線路の先に、次の線路を生成する
            SpawnRailroads(pathFollower2D.pathCreator.GetComponent<Railroad>(), true);
            pathFollower2D.enabled = true;
        }

        private Railroad SpawnRailroads(Railroad currentRailroad, bool spawnObjects)
        {
            Debug.Assert(currentRailroad != null);

            // 線路生成位置を、現在の線路の先に設定する
            _spawnPosition = new Vector3(_spawnPosition.x, 0, _spawnPosition.z + currentRailroad.offsetZ);

            var go = GameObject.Instantiate(railroadPrefabBranch, _spawnPosition, Quaternion.identity);

            // 次の線路生成位置を更新する
            _spawnPosition = new Vector3(_spawnPosition.x + spawnIntervalX, 0, _spawnPosition.z);

            var roads = go.GetComponentsInChildren<Railroad>();
            var holder = go.transform.Find("ObjectsHolder");
            var holder2 = go.transform.Find("ObjectsHolder2");
            Debug.Assert(holder != null);

            foreach (var item in roads)
            {
                PathCreation.Examples.RoadMeshCreator meshCreator;
                if (item.TryGetComponent<PathCreation.Examples.RoadMeshCreator>(out meshCreator))
                {
                    meshCreator.TriggerUpdate();
                }

                _pathCreators.Add(item.path);
            }

            if (spawnObjects && !_eventObjectsSpawned && _controller.IsEventPrepareReady)
            {
                // トロッコ問題イベントオブジェクトの生成

                // 上
                {
                    var placer = roads[1].gameObject.AddComponent<PathPlacerPickups>();

                    placer.holder = holder.gameObject;
                    placer.offset = new Vector3(8f, 0.5f, 0);
                    placer.prefab = prefabGirl;
                    placer.pathCreator = roads[1].path;
                    placer.maxCount = 1;

                    placer.TriggerUpdate();
                }

                // 下
                {
                    var placer = roads[2].gameObject.AddComponent<PathPlacerPickups>();

                    placer.holder = holder2.gameObject;
                    placer.offset = new Vector3(6f, 0.5f, 0);
                    placer.spacing = 1f;
                    placer.prefab = prefabOld;
                    placer.pathCreator = roads[2].path;
                    placer.maxCount = 5;

                    placer.TriggerUpdate();
                }

                _eventObjectsSpawned = true;

                onEventPrepared?.Invoke();
            }
            else if (spawnObjects)
            {
                // アイテムなどの生成
                // ランダムで左右いずれかの線路上に生成する
                Railroad itemRoad, obstacleRoad;
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    itemRoad = roads[1];
                    obstacleRoad = roads[2];
                }
                else
                {
                    itemRoad = roads[2];
                    obstacleRoad = roads[1];
                }

                // アイテム生成
                {
                    // ランダムで小さいクリスタルか大きいクリスタルを配置する
                    var prefab = (Random.Range(0f, 1f) < pickupPrefabLargePercent) ? pickupPrefabLarge : pickupPrefabSmall;

                    var placer = itemRoad.gameObject.AddComponent<PathPlacerPickups>();

                    placer.holder = holder.gameObject;
                    placer.offset = new Vector3(10f, 1f, 0);
                    placer.prefab = prefab;
                    placer.pathCreator = itemRoad.path;
                    placer.maxCount = 1;

                    placer.TriggerUpdate();
                }

                // 障害物を生成
                {
                    var placer = obstacleRoad.gameObject.AddComponent<PathPlacerPickups>();

                    placer.holder = holder2.gameObject;
                    placer.offset = new Vector3(10f, 0, 0);
                    placer.prefab = obstaclePrefab;
                    placer.pathCreator = obstacleRoad.path;
                    placer.maxCount = 1;

                    placer.TriggerUpdate();
                }
            }

            // 現在の線路の末尾に接続する
            var tail = currentRailroad;
            while (tail.next1 != null)
            {
                tail = tail.next1;
            }
            tail.next1 = roads[0];

            return roads[0];
        }

        private void OnTrainEnterPath(PathCreator path)
        {
            // 列車が線路に乗ったとき、その先の線路がなければ生成する
            var railroad = path.GetComponent<Railroad>();

            Debug.Assert(railroad != null);

            if (railroad.next1 == null)
            {
                SpawnRailroads(railroad, true);
            }
        }
    }
}