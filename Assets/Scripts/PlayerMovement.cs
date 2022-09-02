using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private float gravity = 9.8f;

        [HideInInspector]
        public Vector3 Position { get => transform.position; }

        [HideInInspector]
        public bool IsGrounded { get => _controller.IsGrounded; }

        private Controller2D _controller;
        private Vector2 _velocity;

        void Start()
        {
            TryGetComponent(out _controller);
            _velocity = Vector2.zero;
        }

        void Update()
        {
            if (!_controller.IsGrounded)
            {
                _velocity.y -= gravity * Time.deltaTime;
            }

            _controller.Move(_velocity * Time.deltaTime);

            if (_controller.IsGrounded)
            {
                _velocity = Vector2.zero;
            }
        }

        public void Jump(float speed, float angle)
        {
            _velocity = new Vector2(speed * Mathf.Cos(angle), speed * Mathf.Sin(angle));
        }
    }
}
