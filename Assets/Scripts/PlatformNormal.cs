using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    // 普通のプラットフォーム。乗るとスコア加算
    public class PlatformNormal : PlatformBehaviour, ICustomDragEvent
    {
        [SerializeField]
        private int score = 1;

        [SerializeField]
        private Color landedColor = Color.white;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Collider2D anyCollider;

        [SerializeField]
        private float offsetFactor = 0.1f;

        [SerializeField]
        private float springDuration = 2f;

        [SerializeField]
        private float springFactor = 1f;

        [SerializeField]
        private float springAttenuation = 0.9f;

        [SerializeField]
        private float landingSpeed = 5f;

        [SerializeField]
        private float landingDuration = 1f;

        [SerializeField]
        private float landingFactor = 1f;

        [SerializeField]
        private float landingAttenuation = 0.8f;

        [SerializeField]
        private float gravityFactor = 0.1f;

        [SerializeField]
        private float threshold = 0.01f;

        private int _landedCount;

        private Vector3 _startPos;
        private Transform _passenger;
        private Vector3 _passengerPos;

        private Coroutine _coroutine;
        private bool _dragging;
        private Vector2 _dragPos;
        private Vector2 _dragPosBegin;
        private Coroutine _landingAnimationCoroutine;
        private Animator _animator;

        private float _startSize;

        // 内部状態をリセット
        public void ResetState()
        {
            _landedCount = 0;

            _startPos = Vector3.zero;
            _passenger = null;
            _passengerPos = Vector3.zero;

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            if (_landingAnimationCoroutine != null)
            {
                StopCoroutine(_landingAnimationCoroutine);
            }

            _dragging = false;
            _dragPos = _dragPosBegin = Vector2.zero;

            anyCollider.enabled = true;
        }

        public void ChangeSize(float size)
        {
            var scl = _startSize * size;
            transform.localScale = new Vector3(scl, scl, 1);
        }

        protected override void Awake()
        {
            base.Awake();

            TryGetComponent(out _animator);
        }

        void Start()
        {
            ResetState();

            _startSize = transform.localScale.x;
        }

        protected override void Update()
        {
            base.Update();

            if (!_dragging)
            {
                return;
            }

            // 引っ張った方向にオフセット
            var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(_dragPos.x, _dragPos.y, -Camera.main.transform.position.z));
            var worldPosBegin = Camera.main.ScreenToWorldPoint(new Vector3(_dragPosBegin.x, _dragPosBegin.y, -Camera.main.transform.position.z));

            var diff = worldPos - worldPosBegin;

            var offset = diff.normalized * Mathf.Log(diff.magnitude * offsetFactor + 1, 2);

            transform.position = _startPos + offset;
            _passenger.position = _passengerPos + offset;
        }

        protected override void OnPassengerEnter(Transform passenger, Vector2 velocity)
        {
            _startPos = transform.position;
            _passenger = passenger;
            _passengerPos = _passenger.position;

            BroadcastReceivers.RegisterBroadcastReceiver<ICustomDragEvent>(gameObject);

            //Debug.Log($"OnPassengerEnter");

            _landingAnimationCoroutine = StartCoroutine(LandingAnimationCoroutine(velocity));

            _animator.SetTrigger("flap");

            ++_landedCount;

            if (score > 0)
            {
                // プラットフォームの中心と、プレイヤー位置の差分によって、Good か Nice を出す
                var distance = Mathf.Abs(_startPos.x - _passengerPos.x);
                BroadcastExecuteEvents.Execute<IGameControllerRequests>(null,
                    (handler, eventData) => handler.OnLandedPlatform(_passengerPos, distance, _landedCount));

                // スコア加算済みなら以降はスキップ
                if (_landedCount == 1)
                {
                    // スコアを加算する
                    BroadcastExecuteEvents.Execute<IGameControllerRequests>(null,
                        (handler, eventData) => handler.AddScore(score));
                }
            }
        }

        protected override void OnPassengerExit(Transform passenger)
        {
            _passenger = null;

            //Debug.Log($"OnPassengerExit");

            BroadcastExecuteEvents.Execute<IGameControllerRequests>(null,
                (handler, eventData) => handler.OnLeftPlatform());

            BroadcastReceivers.UnregisterBroadcastReceiver<ICustomDragEvent>(gameObject);
        }

        protected override void OnPassengerStay(Transform passenger)
        {

        }

        public void OnBeginDrag(Vector2 screenPos)
        {
        }

        public void OnDrag(Vector2 screenPos, Vector2 beginScreenPos)
        {
            _dragging = true;
            _dragPosBegin = beginScreenPos;
            _dragPos = screenPos;

            if (_landingAnimationCoroutine != null)
            {
                StopCoroutine(_landingAnimationCoroutine);
            }
        }

        public void OnEndDrag(Vector2 screenPos)
        {
            _dragging = false;
            _coroutine = StartCoroutine(SpringCoroutine());

            _animator.SetTrigger("flap");
        }

        void OnAnimatorMove()
        {
            var deltaPos = _animator.deltaPosition;
            transform.position += deltaPos;

            if (_passenger)
            {
                _passenger.position += deltaPos;
            }
        }

        private IEnumerator SpringCoroutine()
        {
            anyCollider.enabled = false;

            Vector3 velocity = Vector3.zero;

            float timer = 0;
            while (timer < springDuration)
            {
                var diff = _startPos - transform.position;
                var acc = diff * springFactor;
                velocity += acc;

                // 減衰
                velocity *= springAttenuation;

                transform.position += velocity * Time.deltaTime;

                yield return null;

                timer += Time.deltaTime;
            }

            transform.position = _startPos;

            anyCollider.enabled = true;
        }

        private IEnumerator LandingAnimationCoroutine(Vector2 _velocity)
        {
            //var velocity = (Vector3)_velocity.normalized * landingSpeed;
            var velocity = Vector3.Lerp(Vector3.down, (Vector3)_velocity.normalized, 0.5f) * landingSpeed;
            //var velocity = Vector3.down * landingSpeed;

            float timer = 0;
            while (timer < landingDuration)
            {
                var diff = _startPos - transform.position;
                var acc = diff * landingFactor;
                Debug.DrawRay(transform.position, acc, Color.red);
                velocity += acc;

                // 原点から離れるとき、鉛直方向に近づくような力が働く。
                if (velocity.magnitude > threshold)
                {
                    var newDiff = _startPos - (transform.position + velocity * Time.deltaTime);
                    if (diff.magnitude < newDiff.magnitude)
                    {
                        var dotproduct = Vector3.Dot(velocity.y > 0 ? Vector3.up : Vector3.down, velocity.normalized);
                        var acc2 = new Vector3(-acc.y, acc.x, 0).normalized * (1.0f - dotproduct) * gravityFactor;
                        Debug.DrawRay(transform.position + velocity * Time.deltaTime, acc2, Color.blue);
                        velocity += acc2;
                    }
                }

                // 減衰
                velocity *= landingAttenuation;

                // プラットフォームと上に乗ったプレイヤーを移動する
                Vector3 offset = velocity * Time.deltaTime;
                transform.position += offset;
                if (_passenger)
                {
                    _passenger.position += offset;
                }

                yield return null;

                timer += Time.deltaTime;
            }
        }

    }
}
