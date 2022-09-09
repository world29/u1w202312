using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Linq;

namespace Unity1Week
{
    /// <summary>
    /// スコアの UI
    /// </summary>
    public class UIScoreView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI scoreText;

        void Start()
        {
            scoreText.text = "";
        }

        public void UpdateScore(float score)
        {
            scoreText.text = $"{score}";
        }
    }
}
