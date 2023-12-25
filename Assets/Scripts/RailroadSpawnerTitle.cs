using UnityEngine;

using PathCreation;

using System.Collections;
using System.Collections.Generic;

namespace u1w202312
{
    public class RailroadSpawnerTitle : MonoBehaviour
    {
        [SerializeField] GameObject railroadPrefabStrait;

        [SerializeField] PathFollower2D pathFollower2D;
        [SerializeField] RailroadTrigger railroadTrigger;

        [SerializeField]
        private float recycleRailroadPosX = -20f;

        private class RailroadWithUse
        {
            public GameObject obj;
            public bool inUse;
        };

        [HideInInspector]
        public Vector3 _spawnPosition; // GameController からアクセスする

        private List<RailroadWithUse> _railroads = new List<RailroadWithUse>();

        // 末尾の線路
        private Railroad _railroadTail;

        private void OnEnable()
        {
            Debug.Log("RailroadSpawnerTitle.OnEnable");

            if (railroadTrigger)
            {
                railroadTrigger.onMove += OnTriggerMove;
            }
        }

        private void OnDisable()
        {
            Debug.Log("RailroadSpawnerTitle.OnDisable");

            if (railroadTrigger)
            {
                railroadTrigger.onMove -= OnTriggerMove;
            }
        }

        private void Start()
        {
            Debug.Log("RailroadSpawnerTitle.Start");

            _spawnPosition = Vector3.zero;

            var railroad = SpawnRailroad();
            pathFollower2D.pathCreator = railroad.path;
            pathFollower2D.enabled = true;
        }

        private void Update()
        {
            // 見切れた線路を再利用できるようにする
            for (int i = 0; i < _railroads.Count; ++i)
            {
                if (_railroads[i].obj.transform.position.x < (recycleRailroadPosX + pathFollower2D.transform.position.x))
                {
                    _railroads[i].inUse = false;
                }
            }
        }

        private Railroad SpawnRailroad()
        {
            // 見切れた線路があれば再利用する
            GameObject spawned = null;
            for (int i = 0; i < _railroads.Count; ++i)
            {
                if (!_railroads[i].inUse)
                {
                    _railroads[i].inUse = true;
                    _railroads[i].obj.transform.position = _spawnPosition;
                    spawned = _railroads[i].obj;
                    break;
                }
            }
            // 再利用できなければ生成
            if (spawned == null)
            {
                spawned = GameObject.Instantiate(railroadPrefabStrait, _spawnPosition, Quaternion.identity);
                var entry = new RailroadWithUse();
                entry.inUse = true;
                entry.obj = spawned;
                _railroads.Add(entry);
            }

            var railroad = spawned.GetComponentInChildren<Railroad>();
            // 再利用された場合はリセットする必要がある
            railroad.next1 = null;

            PathCreation.Examples.RoadMeshCreator meshCreator;
            if (railroad.TryGetComponent<PathCreation.Examples.RoadMeshCreator>(out meshCreator))
            {
                meshCreator.TriggerUpdate();
            }

            // 次の線路生成位置を更新する
            _spawnPosition = new Vector3(_spawnPosition.x + railroad.path.path.length, _spawnPosition.y, _spawnPosition.z);

            // 接続
            if (_railroadTail != null)
            {
                _railroadTail.next1 = railroad;
            }
            _railroadTail = railroad;

            return railroad;
        }

        private void OnTriggerMove(Vector3 triggerPos)
        {
            if (triggerPos.x > _spawnPosition.x)
            {
                SpawnRailroad();
            }
        }
    }
}