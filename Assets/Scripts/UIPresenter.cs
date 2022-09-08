using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class UIPresenter : MonoBehaviour
    {
        [SerializeField]
        private UIScoreView scoreView;

        [SerializeField]
        private UIEvaluationView evaluationView;

        private GameController _gameController;

        void Start()
        {
            GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out _gameController);

            _gameController.OnScoreChanged.AddListener((score) => scoreView.UpdateScore(score));

            _gameController.OnLandingNear.AddListener((landingPosition) => evaluationView.ShowGood(landingPosition));
            _gameController.OnLandingFar.AddListener((landingPosition) => evaluationView.ShowNice(landingPosition));
        }

    }
}
