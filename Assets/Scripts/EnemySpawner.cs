using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Unity1Week
{
    [System.Serializable]
    public class EnemyInfo
    {
        public GameObject prefab;

        public int weight;
    }

    public class EnemySpawner : MonoBehaviour, IPlatformEvent
    {
        [SerializeField]
        private List<EnemyInfo> lootTable = new List<EnemyInfo>();

        [SerializeField, Range(0, 100)]
        private int dropPercent = 10;

        // 敵を生成する範囲のマージン (画面端に生成されて見切れるのを防ぐ)
        [SerializeField]
        private float spawnMerginTop = 0.1f;

        [SerializeField]
        private float spawnMerginBottom = 0.3f;

        private float _worldTop;
        private bool _enemySpawned; // 連続で敵が出ないようにするためのフラグ

        void OnEnable()
        {
            BroadcastReceivers.RegisterBroadcastReceiver<IPlatformEvent>(gameObject);
        }

        void OnDisable()
        {
            BroadcastReceivers.UnregisterBroadcastReceiver<IPlatformEvent>(gameObject);
        }

        void Start()
        {
            // 画面の上端
            _worldTop = Camera.main.ViewportToWorldPoint(Vector2.one).y;

            _enemySpawned = false;

            Debug.Assert(lootTable.Sum((x) => x.weight) == 100);
        }

        public void OnPlatformSpawned(Transform prevPlatform, Transform platform)
        {
            if (_enemySpawned)
            {
                _enemySpawned = false;
                return;
            }

            // まず、敵を生成するかどうか
            if (dropPercent <= Random.Range(0, 100))
            {
                return;
            }

            var mid = Vector3.Lerp(prevPlatform.position, platform.position, 0.5f);

            // プラットフォームの中点から画面の上端の間のランダムな位置に生成する。
            var distance = _worldTop - mid.y;

            Debug.Log($"{mid.y}, {_worldTop}");

            float spawnBottom = mid.y + distance * spawnMerginBottom;
            float spawnTop = _worldTop - distance * spawnMerginTop;

            if (spawnBottom > spawnTop)
            {
                return;
            }

            // どの敵を生成するか抽選
            int rand = Random.Range(0, 100);
            int idx = 0;
            int totalWeight = 0;
            for (; idx < lootTable.Count; idx++)
            {
                totalWeight += lootTable[idx].weight;

                if (rand < totalWeight)
                {
                    break;
                }
            }

            var spawnPosition = new Vector3(mid.x, Random.Range(spawnBottom, spawnTop), 0);
            Instantiate(lootTable[idx].prefab, spawnPosition, Quaternion.identity);

            _enemySpawned = true;
        }
    }
}
