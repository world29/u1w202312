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

        // プラットフォームの間隔が一定以上のときだけ敵を生成する
        [SerializeField]
        private float enemySpawnPlatformDistance = 4f;

        // 敵を生成する範囲のマージン (画面端に生成されて見切れるのを防ぐ)
        [SerializeField]
        private float enemySpawnMerginTop = 0.1f;

        [SerializeField]
        private float enemySpawnMerginBottom = 0.3f;

        private float _worldTop;
        private bool _isItemSpawned;
        private bool _isEnemySpawned; // 連続でアイテムが出ないようにするためのフラグ

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

            _isItemSpawned = false;
            _isEnemySpawned = false;
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
            var phase = GetPhaseParam();

            // 敵を生成するための十分な空間があるか
            bool isEnoughSpace = (Mathf.Abs(platform.position.x - prevPlatform.position.x) >= enemySpawnPlatformDistance);

            if (_isEnemySpawned)
            {
                // 敵を連続で生成しないためのフラグ
                _isEnemySpawned = false;
            }
            else if ((Random.Range(0, 100) < phase.enmeySpawnPercent) && isEnoughSpace)
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

                _isEnemySpawned = true;
            }

            // アイテムについて抽選
            if (_isItemSpawned)
            {
                // アイテムを連続で生成しないためのフラグ
                _isItemSpawned = false;
            }
            else if (Random.Range(0, 100) < phase.itemDropPercent)
            {
                var mid = Vector3.Lerp(prevPlatform.position, platform.position, 0.5f);
                Debug.DrawLine(prevPlatform.position, platform.position, Color.red, 5f);
                Debug.DrawLine(prevPlatform.position, mid, Color.cyan, 5f);

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

                _isItemSpawned = true;
            }
        }
    }
}
