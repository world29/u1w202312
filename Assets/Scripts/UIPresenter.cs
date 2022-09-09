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

        [SerializeField]
        private UIComboView comboView;

        private GameController _gameController;

        void Start()
        {
            GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out _gameController);

            _gameController.OnScoreChanged.AddListener((score) =>
            {
                if (score > 0)
                {
                    scoreView.UpdateScore(score);
                }
            });

            _gameController.OnComboChanged.AddListener((combo) =>
            {
                if (combo > 1)
                {
                    comboView.ShowCombo(combo, _gameController.ComboTimeWindow);
                }
            });

            _gameController.OnComboTimerStoped.AddListener((timeRemained) =>
            {
                comboView.StopComboTimer();
            });

            _gameController.OnLandingNear.AddListener((landingPosition) => evaluationView.ShowGood(landingPosition));
            _gameController.OnLandingFar.AddListener((landingPosition) => evaluationView.ShowNice(landingPosition));
        }

    }
}
