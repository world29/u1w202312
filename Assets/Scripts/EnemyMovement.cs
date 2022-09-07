using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Unity1Week
{
    // 上下に動く敵
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField]
        private float speed = 2f;

        [SerializeField]
        private float radius = 5f;

        private Vector3 _startPos;

        void Start()
        {
            _startPos = transform.position;

            transform.position = _startPos + Vector3.up * radius;

            var targetPos = new Vector3(_startPos.x, _startPos.y - radius * 2f, _startPos.z);

            transform
                .DOMove(targetPos, 1 / speed)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutQuart);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                return;
            }

            var gizmoColor = Gizmos.color;

            Gizmos.color = new Color(1, 0, 0, .2f);
            Gizmos.DrawCube(transform.position, new Vector3(1f, radius));

            Gizmos.color = gizmoColor;
        }
#endif
    }
}
