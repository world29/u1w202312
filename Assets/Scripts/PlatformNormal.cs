using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity1Week
{
    // 普通のプラットフォーム。乗るとスコア加算
    public class PlatformNormal : PlatformBehaviour, ICustomDragEvent
    {
        [SerializeField]
        private int score = 1;

        [SerializeField]
        private Color landedColor = Color.white;

        // 中心からの距離がこの値以下なら Good 判定となる
        [SerializeField]
        private float goodRadius = 0.2f;

        [SerializeField]
        private float goodRadiusOffset = 0.1f;

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
        private float springCollisionDisableTime = 0.2f;

        [SerializeField]
        private float cancelSpringDuration = 1f;

        [SerializeField]
        private float cancelSpringFactor = 0.5f;

        [SerializeField]
        private float cancelSpringAttenuation = 0.8f;

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

        [HideInInspector]
        public UnityEvent OnLanded = new UnityEvent();

        private int _landedCount;

        private Vector3 _startPos;
        private Transform _passenger;
        private Vector3 _passengerPos;

        private Coroutine _springCoroutine;
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

            CancelAnimation();

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

            Debug.Log($"OnPassengerEnter");
            CancelAnimation();

            _landingAnimationCoroutine = StartCoroutine(LandingAnimationCoroutine(velocity));

            _animator.SetTrigger("flap");

            ++_landedCount;

            if (score > 0)
            {
                // プラットフォームの中心と、プレイヤー位置の差分によって、Good か Nice を出す
                var distance = Mathf.Abs((_startPos.x + goodRadiusOffset) - _passengerPos.x);

                bool isGood = distance <= goodRadius;

#if UNITY_EDITOR
                string judge = isGood ? "Good" : "Nice";
                Debug.Log($"Landed: dist={distance.ToString("F3")}, {judge}");
#endif

                BroadcastExecuteEvents.Execute<IGameControllerRequests>(null,
                    (handler, eventData) => handler.OnLandedPlatform(_passengerPos, isGood, _landedCount));

                // スコア加算済みなら以降はスキップ
                if (_landedCount == 1)
                {
                    // スコアを加算する
                    BroadcastExecuteEvents.Execute<IGameControllerRequests>(null,
                        (handler, eventData) => handler.AddScore(score));
                }
            }

            OnLanded.Invoke();
        }

        protected override void OnPassengerExit(Transform passenger)
        {
            _passenger = null;

            Debug.Log($"OnPassengerExit");

            BroadcastExecuteEvents.Execute<IGameControllerRequests>(null,
                (handler, eventData) => handler.OnLeftPlatform());

            BroadcastReceivers.UnregisterBroadcastReceiver<ICustomDragEvent>(gameObject);
        }

        protected override void OnPassengerStay(Transform passenger)
        {

        }

        public void OnBeginDrag(Vector2 screenPos)
        {
            _dragging = true;
        }

        public void OnDrag(Vector2 screenPos, Vector2 beginScreenPos)
        {
            if (_dragging)
            {
                _dragPosBegin = beginScreenPos;
                _dragPos = screenPos;

                CancelAnimation();
            }
        }

        public void OnEndDrag(Vector2 screenPos)
        {
            if (_dragging)
            {
                _springCoroutine = StartCoroutine(SpringCoroutine(springDuration, springFactor, springAttenuation, false));

                StartCoroutine(DisableCollisionCoroutine(springCollisionDisableTime));

                _animator.SetTrigger("flap");

                _dragging = false;
            }
        }

        public void OnDragCancel()
        {
            if (_dragging)
            {
                _dragging = false;
                _springCoroutine = StartCoroutine(SpringCoroutine(cancelSpringDuration, cancelSpringFactor, cancelSpringAttenuation, true));

                // 発射しないのでコリジョンは無効化しない
                // flap アニメーションもなし
            }
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

        private void CancelAnimation()
        {
            if (_landingAnimationCoroutine != null)
            {
                StopCoroutine(_landingAnimationCoroutine);
            }

            if (_springCoroutine != null)
            {
                StopCoroutine(_springCoroutine);
            }
        }

        private IEnumerator DisableCollisionCoroutine(float duration)
        {
            anyCollider.enabled = false;

            yield return new WaitForSeconds(duration);

            anyCollider.enabled = true;
        }

        private IEnumerator SpringCoroutine(float duration, float factor, float attenuation, bool isPassengerMove)
        {
            Vector3 velocity = Vector3.zero;

            float timer = 0;
            while (timer < duration)
            {
                var diff = _startPos - transform.position;
                var acc = diff * factor;
                velocity += acc;

                // 減衰
                velocity *= attenuation;

                var offset = velocity * Time.deltaTime;
                transform.position += offset;

                if (isPassengerMove && _passenger)
                {
                    _passenger.position += offset;
                }

                yield return null;

                timer += Time.deltaTime;
            }

            transform.position = _startPos;
            if (isPassengerMove && _passenger)
            {
                _passenger.position = _passengerPos;
            }
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

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var gizmoColor = Gizmos.color;

            Gizmos.color = new Color(1, 0, 0, .4f);
            Gizmos.DrawCube(transform.position + Vector3.right * goodRadiusOffset, Vector3.one * goodRadius * 2);

            Gizmos.color = gizmoColor;
        }
#endif
    }
}
