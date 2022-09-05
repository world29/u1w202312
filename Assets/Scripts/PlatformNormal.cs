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
        private float springDuration = 2f;

        [SerializeField]
        private float springFactor = 1f;

        private bool _scoreAdded;

        private Vector3 _startPos;
        private Transform _passenger;
        private Vector3 _passengerPos;

        private Coroutine _coroutine;

        // 内部状態をリセット
        public void ResetState()
        {
            _scoreAdded = false;

            _startPos = Vector3.zero;
            _passenger = null;
            _passengerPos = Vector3.zero;

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            anyCollider.enabled = true;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        void Start()
        {
            ResetState();
        }

        protected override void OnPassengerEnter(Transform passenger)
        {
            _startPos = transform.position;
            _passenger = passenger;
            _passengerPos = _passenger.position;

            BroadcastReceivers.RegisterBroadcastReceiver<ICustomDragEvent>(gameObject);

            // スコア加算済みなら以降はスキップ
            if (_scoreAdded)
            {
                return;
            }

            // スコアを加算する
            BroadcastExecuteEvents.Execute<IGameControllerRequests>(null,
                (handler, eventData) => handler.AddScore(score));

            spriteRenderer.color = landedColor;

            _scoreAdded = true;
        }

        protected override void OnPassengerExit(Transform passenger)
        {
            _passenger = null;

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
            // 引っ張った方向にオフセット
            var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z));
            var worldPosBegin = Camera.main.ScreenToWorldPoint(new Vector3(beginScreenPos.x, beginScreenPos.y, -Camera.main.transform.position.z));

            var diff = worldPos - worldPosBegin;

            var offset = diff * 0.1f;

            transform.position = _startPos + offset;
            _passenger.position = _passengerPos + offset;
        }

        public void OnEndDrag(Vector2 screenPos)
        {
            _coroutine = StartCoroutine(SpringCoroutine());
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
                velocity *= 0.9f;

                transform.position += velocity * Time.deltaTime;

                yield return null;

                timer += Time.deltaTime;
            }

            transform.position = _startPos;

            anyCollider.enabled = true;
        }

    }
}
