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
        [SerializeField] GameObject railroadPrefab;

        [SerializeField] PathFollower2D pathFollower2D;

        [SerializeField] float spawnIntervalX = 30;

        [SerializeField] GameObject pickupPrefabSmall; // Small
        [SerializeField] GameObject pickupPrefabLarge; // Large

        // 大きなクリスタルがスポーンする確率 [0f, 1f]
        [SerializeField] float pickupPrefabLargePercent;

        [SerializeField] GameObject obstaclePrefab;

        private List<PathCreator> _pathCreators = new List<PathCreator>();
        private Vector3 _spawnPosition;

        private void Start()
        {
            _spawnPosition = Vector3.zero;

            SpawnRailroads(false);
            pathFollower2D.onEnterPath += OnTrainEnterPath;
            pathFollower2D.pathCreator = _pathCreators[0];
            pathFollower2D.enabled = true;
        }

        private void Update()
        {
        }

        private Railroad SpawnRailroads(bool spawnObjects)
        {
            var go = GameObject.Instantiate(railroadPrefab, _spawnPosition, Quaternion.identity);

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
                    placer.offset = new Vector3(10f, 1f, 0);
                    placer.prefab = obstaclePrefab;
                    placer.pathCreator = obstacleRoad.path;
                    placer.maxCount = 1;

                    placer.TriggerUpdate();
                }
            }

            return roads[0];
        }

        private void OnTrainEnterPath(PathCreator path)
        {
            // 列車が線路に乗ったとき、その先の線路がなければ生成する
            var railroad = path.GetComponent<Railroad>();

            Debug.Assert(railroad != null);

            if (railroad.next1 == null)
            {
                _spawnPosition = new Vector3(_spawnPosition.x + spawnIntervalX, 0, _spawnPosition.z + railroad.offsetZ);

                var nextRailroad = SpawnRailroads(true);
                railroad.next1 = nextRailroad;
            }
        }
    }
}