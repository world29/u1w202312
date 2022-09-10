using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Unity1Week
{
    // 足場やアイテムの配置を制御する
    public class PlatformSpawner : MonoBehaviour
    {
        [SerializeField]
        private Rect spawnRect;

        [SerializeField]
        private List<Transform> platformPool = new List<Transform>();

        [SerializeField]
        private List<Transform> platformMovingPool = new List<Transform>();

        [SerializeField]
        private PlatformManager platformManager;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // 前方に足場を生成する
                SpawnPlatform();

                // 次の位置に移動する
                MoveNextPosition(platformManager.GetPlatformSpawnInterval());
            }
        }

        private Transform GetPlatformToSpawn()
        {
            // プール内のオブジェクトをx座標でソートして、一番左のものを返す
            platformPool.Sort((lhs, rhs) => lhs.position.x.CompareTo(rhs.position.x));

            return platformPool[0];
        }

        private Transform GetMovingPlatformToSpawn()
        {
            platformMovingPool.Sort((lhs, rhs) => lhs.position.x.CompareTo(rhs.position.x));

            return platformMovingPool[0];
        }

        private void SpawnPlatform()
        {
            float size;
            bool isMovingPlatform;
            platformManager.GetPlatformSpawnParams(out size, out isMovingPlatform);

            if (isMovingPlatform)
            {
                var platform = GetMovingPlatformToSpawn();
                Debug.Assert(platform != null);

                var platformMove = platform.GetComponentInChildren<PlatformMove>();
                if (platformMove)
                {
                    var x = Random.Range(spawnRect.xMin, spawnRect.xMax);
                    //var y = Mathf.Lerp(spawnRect.yMin, spawnRect.yMax, 0.5f);
                    var new_y = platform.position.y;

                    platform.position = new Vector3(transform.position.x + x, new_y, platform.position.z);

                    platformMove.ResetState();
                    platformMove.ChangeSize(size);
                    Debug.Log($"PlatformMove spawned. size={size.ToString("F1")}");

                    // プラットフォーム生成通知
                    platformManager.NotifyPlatformSpawned(platform);
                }
            }
            else
            {
                var platform = GetPlatformToSpawn();
                Debug.Assert(platform != null);

                var platformNormal = platform.GetComponentInChildren<PlatformNormal>();
                if (platformNormal)
                {
                    var x = Random.Range(spawnRect.xMin, spawnRect.xMax);
                    var y = Random.Range(spawnRect.yMin, spawnRect.yMax);

                    platform.position = new Vector3(transform.position.x + x, transform.position.y + y, platform.position.z);

                    platformNormal.ResetState();
                    platformNormal.ChangeSize(size);
                    Debug.Log($"PlatformNormal spawned. size={size.ToString("F1")}");

                    // プラットフォーム生成通知
                    platformManager.NotifyPlatformSpawned(platform);
                }
            }
        }

        private void MoveNextPosition(float offset)
        {
            transform.Translate(offset, 0, 0);
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
