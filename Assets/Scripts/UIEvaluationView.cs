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

        [SerializeField]
        private Vector2 offset;

        [SerializeField]
        private Vector2 velocity;

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
            ShowImage(spriteGood, worldPosition);
        }

        public void ShowNice(Vector3 worldPosition)
        {
            ShowImage(spriteNice, worldPosition);
        }

        private void ShowImage(Sprite sprite, Vector3 worldPosition)
        {
            if (WorldToScreenLocalPoint(worldPosition, out var localPos))
            {
                image.rectTransform.localPosition = localPos + offset;
                image.sprite = sprite;
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

                var targetPos = (Vector2)image.rectTransform.localPosition + velocity;
                image.rectTransform.DOAnchorPos(targetPos, 1);
                image.DOFade(0, 1).OnComplete(() => image.enabled = false);

                image.enabled = true;
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
