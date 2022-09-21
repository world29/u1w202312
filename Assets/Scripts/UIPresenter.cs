using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField]
        private int showComboLimit = 2;

        [SerializeField]
        private UIBonusView bonusView;

        [SerializeField]
        private Button optionButton;

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
                if (combo >= showComboLimit)
                {
                    comboView.ShowCombo(combo, _gameController.ComboTimeWindow);
                }
                else
                {
                    comboView.HideCombo();
                }
            });

            _gameController.OnComboTimerStoped.AddListener((timeRemained) =>
            {
                comboView.StopComboTimer();
            });

            _gameController.OnLandingNear.AddListener((landingPosition) => evaluationView.ShowGood(landingPosition));
            _gameController.OnLandingFar.AddListener((landingPosition) => evaluationView.ShowNice(landingPosition));

            _gameController.OnPickupItem.AddListener((worldPos, score) => bonusView.ShowBonus(worldPos, score));

            // オプションボタンはプレイヤーが操作可能になるまで無効化しておく
            optionButton.onClick.AddListener(() => _gameController.OpenDialogOption());
            _gameController.OnPlayerBegin.AddListener(() => optionButton.gameObject.SetActive(true));
            optionButton.gameObject.SetActive(_gameController.IsPlayerBegin);
        }

    }
}
