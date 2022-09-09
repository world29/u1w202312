using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private float gravity = 9.8f;

        [SerializeField]
        private float hitStop = 1f;

        [SerializeField]
        private float knockbackTime = 2f;

        [SerializeField]
        private Vector2 knockbackForce = new Vector2(-3, 6);

        [SerializeField]
        private AudioClip damageSe;

        [SerializeField]
        private GameEvent deathEvent;

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
        private Coroutine _damageCoroutine;
        private GameController _gameController;
        private bool _isLandedGood;

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
            _isLandedGood = false;

            GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out _gameController);

            _gameController.OnLandingFar.AddListener((_) => _isLandedGood = false);
            _gameController.OnLandingNear.AddListener((_) => _isLandedGood = true);
        }

        void Update()
        {
            if (_damageCoroutine != null)
            {
                return;
            }

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

        public void DealDamage()
        {
            if (_damageCoroutine != null)
            {
                return;
            }

            _damageCoroutine = StartCoroutine(DamageCoroutine());
        }

        private IEnumerator DamageCoroutine()
        {
            // 時間を止めてダメージアニメーションを再生し、落ちていく
            SoundManager.StopBgm(0);

            SoundManager.PlaySe(damageSe);

            _animator.Play("damage");
            _animator.updateMode = AnimatorUpdateMode.UnscaledTime;

            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(hitStop);
            Time.timeScale = 1f;

            // ノックバック
            _velocity = knockbackForce;

            yield return new WaitWhile(() =>
            {
                transform.Translate(_velocity * Time.unscaledDeltaTime);

                _velocity.y -= gravity * Time.unscaledDeltaTime;

                return CheckAlive();
            });

            deathEvent.Raise();

            _animator.updateMode = AnimatorUpdateMode.Normal;
        }

        private bool CheckAlive()
        {
            var viewportPos = Camera.main.WorldToViewportPoint(transform.position);

            var rect = new Rect(-0.2f, -0.2f, 1.2f, 1.2f);
            if (!rect.Contains(viewportPos))
            {
                // 画面外
                Debug.Log($"Player outside screen.");

                return false;
            }

            return true;
        }

        // プレイヤーの状態に応じてアニメーションステートを切り替える
        private void UpdateAnimation()
        {
            string nextState = string.Empty;

            if (_isLanding)
            {
                nextState = _isLandedGood ? "landing_good" : "landing";
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
