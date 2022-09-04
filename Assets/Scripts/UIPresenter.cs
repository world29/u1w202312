using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class UIPresenter : MonoBehaviour
    {
        [SerializeField]
        private UIScoreView scoreView;

        private GameController _gameController;

        void Start()
        {
            GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out _gameController);

            _gameController.OnScoreChanged.AddListener((score) => scoreView.UpdateScore(score));
        }
    }
}
