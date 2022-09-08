using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

using System.Linq;

namespace Unity1Week
{
    /// <summary>
    /// 着地の UI
    /// </summary>
    public class UIEvaluationView : MonoBehaviour
    {
        [SerializeField]
        private Image image;

        [SerializeField]
        private Sprite spriteGood;

        [SerializeField]
        private Sprite spriteNice;

        private RectTransform _rectTransform;

        void Awake()
        {
            TryGetComponent(out _rectTransform);
        }

        void Start()
        {
            image.enabled = false;
        }

        public void ShowGood(Vector3 worldPosition)
        {
            image.sprite = spriteGood;

            var worldPos = worldPosition + Vector3.up * 2f;

            if (WorldToScreenLocalPoint(worldPos, out var localPos))
            {
                image.rectTransform.localPosition = localPos;

                image.enabled = true;

                DOVirtual.DelayedCall(1f, () => image.enabled = false);
            }
        }

        public void ShowNice(Vector3 worldPosition)
        {
            image.sprite = spriteNice;

            var worldPos = worldPosition + Vector3.up * 2f;

            if (WorldToScreenLocalPoint(worldPos, out var localPos))
            {
                image.rectTransform.localPosition = localPos;

                image.enabled = true;

                DOVirtual.DelayedCall(1f, () => image.enabled = false);
            }
        }

        private bool WorldToScreenLocalPoint(Vector3 worldPos, out Vector2 localPos)
        {
            var screenPos = Camera.main.WorldToScreenPoint(worldPos);

            RectTransform parentUI = _rectTransform.parent.GetComponent<RectTransform>();

            // スクリーン座標 → UIローカル座標変換
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentUI,
                screenPos,
                Camera.main,
                out localPos);
        }
    }
}
