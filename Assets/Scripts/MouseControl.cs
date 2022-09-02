using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


namespace Unity1Week
{
    public class MouseControl : MonoBehaviour
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

        private Mouse _mouse;

        private Vector2 _screenPointDown;
        private Vector2 _screenPointUp;
        private float _launchSpeed;
        private float _launchAngle;

        void Start()
        {
            _mouse = Mouse.current;

            //projectileLine.material = new Material(Shader.Find("Sprites/Default"));
            projectileLine.widthMultiplier = 0.1f;
            projectileLine.enabled = false;
        }

        void Update()
        {
            if (!playerMovement.IsGrounded)
            {
                return;
            }

            // マウス押している
            if (_mouse.leftButton.isPressed)
            {
                var z = -Camera.main.transform.position.z;

                var screenPoint = _mouse.position.ReadValue();

                var worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, z));
                var worldPointDown = Camera.main.ScreenToWorldPoint(new Vector3(_screenPointDown.x, _screenPointDown.y, z));

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

            // マウス押された
            if (_mouse.leftButton.wasPressedThisFrame)
            {
                _screenPointDown = _mouse.position.ReadValue();
            }

            // マウス離した
            if (_mouse.leftButton.wasReleasedThisFrame)
            {
                // 発射
                _screenPointUp = _mouse.position.ReadValue();

                projectileLine.enabled = false;

                LaunchPlayer(_launchSpeed, _launchAngle);
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
