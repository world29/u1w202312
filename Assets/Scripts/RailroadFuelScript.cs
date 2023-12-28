using UnityEngine;

using PathCreation;

using System.Collections;
using System.Collections.Generic;

namespace u1w202312
{
    // SpriteRenderer コンポーネントを持つ GameObject にアタッチする
    [RequireComponent(typeof(SpriteRenderer))]
    public class RailroadFuelScript : MonoBehaviour
    {
        // 閾値
        [SerializeField]
        private float threashold = 30f;

        // true なら残り燃料が閾値以上のとき表示する。
        [SerializeField]
        private bool visible = true;

        private SpriteRenderer _spriteRenderer;
        private RailroadGameController _controller;

        private void Start()
        {
            if (!TryGetComponent(out _spriteRenderer))
            {
                Debug.Assert(false);
            }

            var go = GameObject.FindGameObjectWithTag("GameController");
            Debug.Assert(go != null);
            if (!go.TryGetComponent<RailroadGameController>(out _controller))
            {
                Debug.Assert(false);
            }
        }

        private void Update()
        {
            // 残り燃料が閾値を下回ったら、スプライトの描画を無効化(あるいは有効化)する
            if (_controller.Fuel < threashold)
            {
                _spriteRenderer.enabled = !visible;
            }
            else
            {
                _spriteRenderer.enabled = visible;
            }
        }
    }
}