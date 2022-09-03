using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    // 普通のプラットフォーム。乗るとスコア加算
    public class PlatformNormal : PlatformBehaviour
    {
        [SerializeField]
        private int score = 1;

        [SerializeField]
        private Color landedColor = Color.white;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        private bool _landed;

        // 内部状態をリセット
        public void ResetState()
        {
            _landed = false;
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
            // すでに乗っていたら何もしない
            if (_landed)
            {
                return;
            }

            spriteRenderer.color = landedColor;

            _landed = true;
        }

        protected override void OnPassengerExit(Transform passenger)
        {

        }

        protected override void OnPassengerStay(Transform passenger)
        {

        }
    }
}
