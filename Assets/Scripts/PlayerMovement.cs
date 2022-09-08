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
        private Animator _animator;
        private string _currentState;
        private bool _isCharging;
        private bool _isJumping;
        private bool _isLanding;
        private bool _isGroundedPrev;
        private Coroutine _landingCoroutine;

        void Start()
        {
            TryGetComponent(out _controller);
            _velocity = Vector2.zero;
            TryGetComponent(out _animator);
            _currentState = string.Empty;
            _isCharging = false;
            _isJumping = false;
            _isLanding = false;
            _isGroundedPrev = false;
            _landingCoroutine = null;
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

                _isJumping = false;

                // このフレームで着地したなら、着地アニメーションに遷移する
                if (!_isGroundedPrev)
                {
                    _landingCoroutine = StartCoroutine(LandingCoroutine());
                }
            }

            UpdateAnimation();

            _isGroundedPrev = IsGrounded;
        }

        public void SetCharging(bool isCharging)
        {
            _isCharging = isCharging;
        }

        public void Jump(float speed, float angle)
        {
            _velocity = new Vector2(speed * Mathf.Cos(angle), speed * Mathf.Sin(angle));

            _isJumping = true;
        }

        private IEnumerator LandingCoroutine()
        {
            _isLanding = true;

            yield return new WaitUntil(() =>
            {
                // landing アニメーションの再生が終わったら遷移する。
                var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("landing"))
                {
                    if (stateInfo.normalizedTime >= 1.0f)
                    {
                        return true;
                    }
                }
                return false;
            });

            _isLanding = false;
        }

        // プレイヤーの状態に応じてアニメーションステートを切り替える
        private void UpdateAnimation()
        {
            string nextState = string.Empty;

            if (_isLanding)
            {
                nextState = "landing";
            }
            else if (_isCharging)
            {
                nextState = "charge";
            }
            else if (IsGrounded)
            {
                nextState = "idle";
            }
            else if (_isJumping)
            {
                nextState = "jump";
            }

            if (_currentState != nextState)
            {
                _animator.Play(nextState);
                _currentState = nextState;
            }
        }
    }
}
