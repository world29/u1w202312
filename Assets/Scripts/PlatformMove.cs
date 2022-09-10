using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

namespace Unity1Week
{
    public class PlatformMove : MonoBehaviour
    {
        [SerializeField]
        private PlatformNormal m_platform;

        [SerializeField]
        private Transform[] m_localWaypoints;

        [SerializeField]
        private float m_speed;

        [SerializeField, Range(0, 2)]
        private float m_easeAmount;

        private Vector3[] m_globalWaypoints;
        private int m_fromWaypointIndex;
        private float m_percentBetweenWaypoints;
        private bool m_landed;

        [ContextMenu("ResetState")]
        public void ResetState()
        {
            m_landed = false;

            if (m_localWaypoints != null)
            {
                m_globalWaypoints = new Vector3[m_localWaypoints.Length];
                for (int i = 0; i < m_localWaypoints.Length; i++)
                {
                    m_globalWaypoints[i] = m_localWaypoints[i].position;
                }
            }

            m_platform.ResetState();
        }

        public void ChangeSize(float size)
        {
            m_platform.ChangeSize(size);
        }

        void Awake()
        {
            m_platform.OnLanded.AddListener(() => m_landed = true);
        }

        void Start()
        {
            ResetState();
        }

        void Update()
        {
            if (m_landed)
            {
                return;
            }

            if (!IsInViewport())
            {
                return;
            }

            // このフレームでのプラットフォームの移動量を計算したのち、
            // 上に乗っている者たちと自身にそれを適用する。
            var moveAmount = CalculatePlatformMovement();

            transform.Translate(moveAmount);
        }

        private bool IsInViewport()
        {
            var viewportPos = Camera.main.WorldToViewportPoint(transform.position);

            var rect = new Rect(-0.2f, 0, 1.4f, 1);
            return rect.Contains(viewportPos);
        }

        private float Ease(float x)
        {
            float a = m_easeAmount + 1;
            return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
        }

        private Vector3 CalculatePlatformMovement()
        {
            if (m_globalWaypoints.Length == 0)
            {
                return Vector3.zero;
            }

            m_fromWaypointIndex %= m_globalWaypoints.Length;
            int toWaypointIndex = (m_fromWaypointIndex + 1) % m_globalWaypoints.Length;
            float distanceBetweenWaypoints = Vector3.Distance(m_globalWaypoints[m_fromWaypointIndex], m_globalWaypoints[toWaypointIndex]);
            m_percentBetweenWaypoints += Time.deltaTime * m_speed / distanceBetweenWaypoints;
            m_percentBetweenWaypoints = Mathf.Clamp01(m_percentBetweenWaypoints);
            float easedPercentBetweenWaypoints = Ease(m_percentBetweenWaypoints);

            Vector3 newPos = Vector3.Lerp(m_globalWaypoints[m_fromWaypointIndex], m_globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

            if (m_percentBetweenWaypoints >= 1)
            {
                m_percentBetweenWaypoints = 0;
                m_fromWaypointIndex++;

                if (m_fromWaypointIndex >= m_globalWaypoints.Length - 1)
                {
                    m_fromWaypointIndex = 0;
                    System.Array.Reverse(m_globalWaypoints);
                }
            }
            return newPos - transform.position;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (m_localWaypoints != null)
            {
                Gizmos.color = Color.red;
                float size = .3f;

                for (int i = 0; i < m_localWaypoints.Length; i++)
                {
                    Vector3 globalWaypointPos = (Application.isPlaying) ? m_globalWaypoints[i] : m_localWaypoints[i].position;
                    Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                    Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
                }
            }
        }
#endif
    }
}
