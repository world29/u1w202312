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

        private List<PathCreator> _pathCreators = new List<PathCreator>();
        private Vector3 _spawnPosition;

        private void Start()
        {
            _spawnPosition = Vector3.zero;

            SpawnRailroads();
            pathFollower2D.onEnterPath += OnTrainEnterPath;
            pathFollower2D.pathCreator = _pathCreators[0];
            pathFollower2D.enabled = true;
        }

        private void Update()
        {
        }

        private Railroad SpawnRailroads()
        {
            var go = GameObject.Instantiate(railroadPrefab, _spawnPosition, Quaternion.identity);

            var roads = go.GetComponentsInChildren<Railroad>();

            foreach (var item in roads)
            {
                PathCreation.Examples.RoadMeshCreator meshCreator;
                if (item.TryGetComponent<PathCreation.Examples.RoadMeshCreator>(out meshCreator))
                {
                    meshCreator.TriggerUpdate();
                }
                Debug.Log(item.name);

                _pathCreators.Add(item.path);
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

                var nextRailroad = SpawnRailroads();
                railroad.next1 = nextRailroad;
            }
        }
    }
}