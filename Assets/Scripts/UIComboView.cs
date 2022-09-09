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

            comboText
                .DOFade(0, duration);
        }

        public void StopComboTimer()
        {
            comboText.DOKill();
        }
    }
}
