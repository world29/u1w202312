using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace u1w202312
{
    public class UIRailroadScorePresenter : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProUGUI scoreText;

        private RailroadGameController _controller;

        void Start()
        {
            var go = GameObject.FindGameObjectWithTag("GameController");
            Debug.Assert(go != null);
            go.TryGetComponent<RailroadGameController>(out _controller);
            Debug.Assert(_controller != null);
        }

        private void LateUpdate()
        {
            if (_controller != null)
            {
                scoreText.text = _controller.DistanceTravelled.ToString("F1");
            }
        }
    }
}
