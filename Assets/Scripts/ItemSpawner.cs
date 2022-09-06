using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class ItemSpawner : MonoBehaviour, IPlatformEvent
    {
        [SerializeField]
        private Item itemPrefab;

        [SerializeField, Range(0, 100)]
        private int percent = 50;

        // アイテムを生成する範囲のマージン (画面端に生成されて見切れるのを防ぐ)
        [SerializeField]
        private float spawnMerginTop = 0.1f;

        [SerializeField]
        private float spawnMerginBottom = 0.3f;

        private float _worldTop;

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
        }

        public void OnPlatformSpawned(PlatformNormal prevPlatform, PlatformNormal platform)
        {
            if (percent <= Random.Range(0, 100))
            {
                return;
            }

            var mid = Vector3.Lerp(prevPlatform.transform.position, platform.transform.position, 0.5f);

            // プラットフォームの中点から画面の上端の間のランダムな位置に生成する。
            var distance = _worldTop - mid.y;

            Debug.Log($"{mid.y}, {_worldTop}");

            float spawnBottom = mid.y + distance * spawnMerginBottom;
            float spawnTop = _worldTop - distance * spawnMerginTop;

            if (spawnBottom > spawnTop)
            {
                return;
            }

            var spawnPosition = new Vector3(mid.x, Random.Range(spawnBottom, spawnTop), 0);
            Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
