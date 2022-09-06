using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


namespace Unity1Week
{
    public class MouseControl : MonoBehaviour, ICustomDragEvent
    {
        [SerializeField]
        private float gravity = 9.8f;

        [SerializeField]
        private float speed = 5f;

        [SerializeField]
        private float timeMax = 3f;

        [SerializeField]
        private float timeStep = 0.2f;

        [SerializeField]
        private LineRenderer projectileLine;

        [SerializeField]
        private PlayerMovement playerMovement;

        private float _launchSpeed;
        private float _launchAngle;
        private bool _landing;

        void OnEnable()
        {
            BroadcastReceivers.RegisterBroadcastReceiver<ICustomDragEvent>(gameObject);
        }

        void OnDisable()
        {
            BroadcastReceivers.UnregisterBroadcastReceiver<ICustomDragEvent>(gameObject);
        }

        void Start()
        {
            //projectileLine.material = new Material(Shader.Find("Sprites/Default"));
            projectileLine.widthMultiplier = 0.1f;
            projectileLine.enabled = false;
            _landing = false;
        }

        public void OnBeginDrag(Vector2 screenPos)
        {
        }

        public void OnDrag(Vector2 screenPos, Vector2 beginScreenPos)
        {
            if (!playerMovement.IsGrounded)
            {
                return;
            }

            _landing = true;

            var z = -Camera.main.transform.position.z;

            var worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, z));
            var worldPointDown = Camera.main.ScreenToWorldPoint(new Vector3(beginScreenPos.x, beginScreenPos.y, z));

            var diff = worldPoint - worldPointDown;
            if (diff.x == 0.0f)
            {
                diff.x = 0.1f;
            }

            float speedMultiplier = diff.magnitude;
            _launchSpeed = speed * speedMultiplier;

            // マウスの移動と逆方向に飛ぶ
            diff *= -1.0f;
            _launchAngle = Mathf.Atan2(diff.y, diff.x);

            // 予測の表示
            var points = ProjectileArcPoints(_launchSpeed, _launchAngle, gravity, timeMax, timeStep);
            if (points.Length >= 2)
            {
                Vector3[] positions = new Vector3[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    var center = playerMovement.Position;
                    positions[i] = new Vector3(points[i].x + center.x, points[i].y + center.y, z);
                }
                projectileLine.SetPositions(positions);
                projectileLine.positionCount = positions.Length;

                projectileLine.enabled = true;
            }
        }

        public void OnEndDrag(Vector2 screenPos)
        {
            if (_landing)
            {
                LaunchPlayer(_launchSpeed, _launchAngle);

                projectileLine.enabled = false;

                _landing = false;
            }
        }

        // 発射
        private void LaunchPlayer(float speed, float angle)
        {
            playerMovement.Jump(speed, angle);
        }

        private static Vector2[] ProjectileArcPoints(float speed, float angle, float gravity, float timeMax, float timeStep)
        {
            int iterationCount = Mathf.CeilToInt(timeMax / timeStep);

            Vector2[] points = new Vector2[iterationCount + 1];

            float t = 0f;
            for (int i = 0; i <= iterationCount; i++)
            {
                var x = speed * Mathf.Cos(angle) * t;
                var y = speed * Mathf.Sin(angle) * t - 0.5f * gravity * (t * t);

                Vector2 p = new Vector2(x, y);

                points[i] = p;

                t += timeStep;
            }

            return points;
        }
    }
}
