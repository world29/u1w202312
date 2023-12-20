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

        [SerializeField] GameObject pickupPrefab;

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
                var road = (Random.Range(0f, 1f) > 0.5f) ? roads[1] : roads[2];

                var placer = road.gameObject.AddComponent<PathPlacerPickups>();

                placer.holder = holder.gameObject;
                placer.offset = new Vector3(5f, 0.3f, 0);
                placer.prefab = pickupPrefab;
                placer.pathCreator = road.path;

                placer.TriggerUpdate();
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