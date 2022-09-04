using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class UIResultPresenter : MonoBehaviour
    {
        [SerializeField]
        private UIResultScoreView scoreView;

        private GameController _gameController;

        void Start()
        {
            GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out _gameController);

            scoreView.UpdateScore(_gameController.Score);
        }

        public void Retry()
        {
            _gameController.Retry();
        }

        public void BackToTitle()
        {
            _gameController.BackToTitle();
        }
    }
}
