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
    /// コンボの UI
    /// </summary>
    public class UIComboView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI comboText;

        private RectTransform _rectTransform;
        private Sequence _seq;

        void Awake()
        {
            TryGetComponent(out _rectTransform);
        }

        void Start()
        {
            comboText.enabled = false;
        }

        public void ShowCombo(int combo, float duration)
        {
            comboText.DOKill();

            comboText.text = $"{combo} Combo";

            comboText.alpha = 1f;
            comboText.enabled = true;

            _seq = DOTween.Sequence();
            _seq.Append(
                _rectTransform
                .DOPunchScale(Vector3.one * 1.05f, 0.3f, 1, 1)
                .SetEase(Ease.OutBack, 5f));

            _seq.Append(comboText
                .DOFade(0, duration));

            _seq.Play();
        }

        public void StopComboTimer()
        {
            if (_seq != null)
            {
                _seq.Kill();
                _seq = null;
            }

            comboText.alpha = 1f;
        }
    }
}
