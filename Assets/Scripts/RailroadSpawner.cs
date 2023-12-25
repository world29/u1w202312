using UnityEngine;

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

        [SerializeField] GameObject railroadPrefabStrait;

        [SerializeField] PathFollower2D pathFollower2D;
        [SerializeField] RailroadTrigger railroadTrigger;

        [SerializeField] float spawnIntervalX = 30;

        [SerializeField] GameObject pickupPrefabSmall; // Small
        [SerializeField] GameObject pickupPrefabLarge; // Large

        // 大きなクリスタルがスポーンする確率 [0f, 1f]
        [SerializeField] float pickupPrefabLargePercent;

        [SerializeField] GameObject obstaclePrefab;

        private List<PathCreator> _pathCreators = new List<PathCreator>();
        private Vector3 _spawnPosition;
        private Railroad _railroadTail;

        private RailroadGameController _controller;

        private void Start()
        {
            var go = GameObject.FindGameObjectWithTag("GameController");
            Debug.Assert(go != null);
            go.TryGetComponent<RailroadGameController>(out _controller);
            Debug.Assert(_controller != null);

            _spawnPosition = Vector3.zero;

            SpawnRailroads(null, false);
            //SpawnRailroadsBranch(false);
            pathFollower2D.onEnterPath += OnTrainEnterPath;
            pathFollower2D.pathCreator = _pathCreators[0];
            pathFollower2D.enabled = true;

            Debug.Assert(railroadTrigger != null);
            railroadTrigger.onMove += OnTriggerMove;
        }

        private void Update()
        {
        }

        private Railroad SpawnRailroads(Railroad currentRailroad, bool spawnObjects)
        {
            var offsetZ = currentRailroad != null ? currentRailroad.offsetZ : 0;

            if (_controller.CurrentGameState == GameState.Gameplay)
            {
                _spawnPosition = new Vector3(_spawnPosition.x + spawnIntervalX, 0, _spawnPosition.z + offsetZ);

                var nextRailroad = SpawnRailroadsBranch(spawnObjects);
                return nextRailroad;
            }
            else if (_controller.CurrentGameState == GameState.Title)
            {
                _spawnPosition = new Vector3(_spawnPosition.x + spawnIntervalX, 0, _spawnPosition.z + offsetZ);

                var nextRailroad = SpawnRailroadsStrait();
                UpdateRailroadTail(nextRailroad);
                return nextRailroad;
            }
            return null;
        }

        private Railroad SpawnRailroadsBranch(bool spawnObjects)
        {
            var go = GameObject.Instantiate(railroadPrefabBranch, _spawnPosition, Quaternion.identity);

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

            // アイテムなどの生成
            if (spawnObjects)
            {
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

            return roads[0];
        }

        private Railroad SpawnRailroadsStrait()
        {
            var go = GameObject.Instantiate(railroadPrefabStrait, _spawnPosition, Quaternion.identity);

            var roads = go.GetComponentsInChildren<Railroad>();

            foreach (var item in roads)
            {
                PathCreation.Examples.RoadMeshCreator meshCreator;
                if (item.TryGetComponent<PathCreation.Examples.RoadMeshCreator>(out meshCreator))
                {
                    meshCreator.TriggerUpdate();
                }

                _pathCreators.Add(item.path);
            }

            return roads[0];
        }

        private void UpdateRailroadTail(Railroad railroad)
        {
            var temp = railroad;

            while (temp.next1 != null)
            {
                temp = temp.next1;
            }
            _railroadTail = temp;
        }

        private void OnTrainEnterPath(PathCreator path)
        {
            // 列車が線路に乗ったとき、その先の線路がなければ生成する
            var railroad = path.GetComponent<Railroad>();

            Debug.Assert(railroad != null);

            if (railroad.next1 == null)
            {
                railroad.next1 = SpawnRailroads(railroad, true);
            }
        }

        private void OnTriggerMove(Vector3 triggerPos)
        {
            if (_controller.CurrentGameState != GameState.Title)
            {
                return;
            }

            if (_railroadTail == null)
            {
                return;
            }

            if (triggerPos.x > _spawnPosition.x)
            {
                var tail = _railroadTail;

                tail.next1 = SpawnRailroads(tail, false);
            }
        }
    }
}