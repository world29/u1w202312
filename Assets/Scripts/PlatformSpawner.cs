using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    // 足場やアイテムの配置を制御する
    public class PlatformSpawner : MonoBehaviour
    {
        [SerializeField]
        private Rect spawnRect;

        [SerializeField]
        private float nextOffset = 10;

        [SerializeField]
        private Transform platform;

        [SerializeField]
        private PlatformManager platformManager;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // 前方に足場を生成する
                SpawnPlatform();

                // 次の位置に移動する
                MoveNextPosition();
            }
        }

        private void SpawnPlatform()
        {
            var x = Random.Range(spawnRect.xMin, spawnRect.xMax);
            var y = Random.Range(spawnRect.yMin, spawnRect.yMax);

            platform.position = new Vector3(transform.position.x + x, transform.position.y + y, platform.position.z);

            // プラットフォームの状態をリセット
            var platformNormal = platform.GetComponentInChildren<PlatformNormal>();
            if (platformNormal)
            {
                platformNormal.ResetState();

                // プラットフォーム生成通知
                platformManager.NotifyPlatformSpawned(platformNormal);
            }
        }

        private void MoveNextPosition()
        {
            transform.Translate(nextOffset, 0, 0);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var gizmoColor = Gizmos.color;

            Gizmos.color = new Color(0, 1, 0, .2f);
            Gizmos.DrawCube(transform.position + (Vector3)spawnRect.center, spawnRect.size);

            Gizmos.color = gizmoColor;
        }
#endif
    }
}
