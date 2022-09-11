using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Unity1Week
{
    public class ItemEnemySpawner : MonoBehaviour, IPlatformEvent
    {
        [SerializeField]
        private ItemEnemySetting setting;

        // アイテムを生成する範囲のマージン (画面端に生成されて見切れるのを防ぐ)
        [SerializeField]
        private float itemSpawnMerginTop = 0.1f;

        [SerializeField]
        private float itemSpawnMerginBottom = 0.3f;

        // 敵を生成する範囲のマージン (画面端に生成されて見切れるのを防ぐ)
        [SerializeField]
        private float enemySpawnMerginTop = 0.1f;

        [SerializeField]
        private float enemySpawnMerginBottom = 0.3f;

        private float _worldTop;
        private bool _isSpawned; // 連続でアイテムが出ないようにするためのフラグ

        private GameController _gameController;

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
            GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out _gameController);

            // 画面の上端
            _worldTop = Camera.main.ViewportToWorldPoint(Vector2.one).y;

            _isSpawned = false;
        }

        public ItemEnemySetting.Phase GetPhaseParam()
        {
            var phaseTable = setting.phaseTable;

            int idx = phaseTable.Count - 1;
            for (; 0 < idx; idx--)
            {
                if (_gameController.Score >= phaseTable[idx].score)
                {
                    break;
                }
            }

            return setting.phaseTable[idx];
        }

        public void OnPlatformSpawned(Transform prevPlatform, Transform platform)
        {
            if (_isSpawned)
            {
                _isSpawned = false;
                return;
            }

            var phase = GetPhaseParam();

            // まず、敵をスポーンするか
            if (Random.Range(0, 100) < phase.enmeySpawnPercent)
            {
                var mid = Vector3.Lerp(prevPlatform.position, platform.position, 0.5f);

                // プラットフォームの中点から画面の上端の間のランダムな位置に生成する。
                var distance = _worldTop - mid.y;

                float spawnBottom = mid.y + distance * enemySpawnMerginBottom;
                float spawnTop = _worldTop - distance * enemySpawnMerginTop;

                if (spawnBottom > spawnTop)
                {
                    return;
                }

                // どの敵を生成するか抽選
                int rand = Random.Range(0, 100);
                int idx = 0;
                int totalWeight = 0;
                for (; idx < phase.enemyLootTable.Count; idx++)
                {
                    totalWeight += phase.enemyLootTable[idx].weight;

                    if (rand < totalWeight)
                    {
                        break;
                    }
                }

                var spawnPosition = new Vector3(mid.x, Random.Range(spawnBottom, spawnTop), 0);
                Instantiate(phase.enemyLootTable[idx].prefab, spawnPosition, Quaternion.identity);

                Debug.Log($"Enemy spawned. {phase.enemyLootTable[idx].prefab.name} ({spawnPosition.ToString("F1")})");

                _isSpawned = true;
            }

            // 敵を生成しない場合、アイテムについて再度抽選
            if (Random.Range(0, 100) < phase.itemDropPercent)
            {
                var mid = Vector3.Lerp(prevPlatform.position, platform.position, 0.5f);

                // プラットフォームの中点から画面の上端の間のランダムな位置に生成する。
                var distance = _worldTop - mid.y;

                float spawnBottom = mid.y + distance * itemSpawnMerginBottom;
                float spawnTop = _worldTop - distance * itemSpawnMerginTop;

                if (spawnBottom > spawnTop)
                {
                    return;
                }

                // どのアイテムがドロップするか抽選
                int rand = Random.Range(0, 100);
                int idx = 0;
                int totalWeight = 0;
                for (; idx < phase.itemLootTable.Count; idx++)
                {
                    totalWeight += phase.itemLootTable[idx].weight;

                    if (rand < totalWeight)
                    {
                        break;
                    }
                }

                var spawnPosition = new Vector3(mid.x, Random.Range(spawnBottom, spawnTop), 0);
                Instantiate(phase.itemLootTable[idx].prefab, spawnPosition, Quaternion.identity);

                Debug.Log($"Item spawned. {phase.itemLootTable[idx].prefab.name} ({spawnPosition.ToString("F1")})");

                _isSpawned = true;
            }
        }
    }
}
