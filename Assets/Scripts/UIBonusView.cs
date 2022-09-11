using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

using System.Linq;

namespace Unity1Week
{
    public class UIBonusView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI bonusText;

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
            bonusText.enabled = false;
        }

        public void ShowBonus(Vector3 worldPos, float scoreToAdd)
        {
            bonusText.DOKill();

            bonusText.transform.position = worldPos + (Vector3)offset;
            bonusText.alpha = 1f;
            bonusText.enabled = true;
            bonusText.text = $"+{scoreToAdd}";

            var targetPos = (Vector2)bonusText.transform.position + velocity;
            bonusText.transform.DOMove(targetPos, 1).SetEase(Ease.OutQuad);
            bonusText.DOFade(0, 1).OnComplete(() => bonusText.enabled = false);
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
