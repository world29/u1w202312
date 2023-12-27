using UnityEngine;

using PathCreation;

using System.Collections;
using System.Collections.Generic;

namespace u1w202312
{
    public class RailroadTreeSpawner : MonoBehaviour
    {
        [SerializeField] GameObject prafabTrees;

        [SerializeField] float spawnInterval;

        [SerializeField] PathFollower2D pathFollower2D;
        [SerializeField] RailroadTrigger railroadTrigger;

        [SerializeField]
        private float recycleTreesPosX = -20f;

        private class TreesWithUse
        {
            public GameObject obj;
            public bool inUse;
        };

        [HideInInspector]
        public Vector3 _spawnPosition;

        private List<TreesWithUse> _treesList = new List<TreesWithUse>();

        private void OnEnable()
        {
            Debug.Log("RailroadTreeSpawner.OnEnable");

            if (railroadTrigger)
            {
                railroadTrigger.onMove += OnTriggerMove;
            }
        }

        private void OnDisable()
        {
            Debug.Log("RailroadTreeSpawner.OnDisable");

            if (railroadTrigger)
            {
                railroadTrigger.onMove -= OnTriggerMove;
            }
        }

        private void Start()
        {
            Debug.Log("RailroadTreeSpawner.Start");

            _spawnPosition = Vector3.zero;

            SpawnTrees();
        }

        private void Update()
        {
            // 見切れたオブジェクトを再利用できるようにする
            for (int i = 0; i < _treesList.Count; ++i)
            {
                if (_treesList[i].obj.transform.position.x < (recycleTreesPosX + pathFollower2D.transform.position.x))
                {
                    _treesList[i].obj.SetActive(false);
                    _treesList[i].inUse = false;
                }
            }
        }

        private void SpawnTrees()
        {
            // 見切れた線路があれば再利用する
            GameObject spawned = null;
            for (int i = 0; i < _treesList.Count; ++i)
            {
                if (!_treesList[i].inUse)
                {
                    _treesList[i].obj.SetActive(true);
                    _treesList[i].inUse = true;
                    _treesList[i].obj.transform.position = _spawnPosition;
                    spawned = _treesList[i].obj;
                    break;
                }
            }
            // 再利用できなければ生成
            if (spawned == null)
            {
                spawned = GameObject.Instantiate(prafabTrees, _spawnPosition, Quaternion.identity, transform);
                var entry = new TreesWithUse();
                entry.inUse = true;
                entry.obj = spawned;
                _treesList.Add(entry);
            }

            // 次の線路生成位置を更新する
            _spawnPosition = new Vector3(_spawnPosition.x + spawnInterval, _spawnPosition.y, _spawnPosition.z);
        }

        private void OnTriggerMove(Vector3 triggerPos)
        {
            if (triggerPos.x > _spawnPosition.x)
            {
                SpawnTrees();
            }
        }
    }
}