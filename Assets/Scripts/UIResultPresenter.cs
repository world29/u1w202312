using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Unity1Week
{
    public class UIResultPresenter : MonoBehaviour
    {
        [SerializeField] private RectTransform windowRect;

        [SerializeField] private TextMeshProUGUI resultTitle;

        [SerializeField] private TextMeshProUGUI scoreLabel;

        [SerializeField] private TextMeshProUGUI scoreValue;

        [SerializeField] private TextMeshProUGUI goodLabel;

        [SerializeField] private TextMeshProUGUI goodValue;

        [SerializeField] private TextMeshProUGUI maxComboLabel;

        [SerializeField] private TextMeshProUGUI maxComboValue;

        [SerializeField] private TextMeshProUGUI totalScoreLabel;

        [SerializeField] private TextMeshProUGUI totalScoreValue;

        [SerializeField] private TextMeshProUGUI rankText;

        [SerializeField] private CanvasGroup retryButtonCanvas;

        [SerializeField] private GridLayoutGroup gridLayoutGroup;

        private GameController _gameController;

        void Initialize()
        {
            resultTitle.alpha = 0;
            scoreLabel.alpha = scoreValue.alpha = 0;
            goodLabel.alpha = goodValue.alpha = 0;
            maxComboLabel.alpha = maxComboValue.alpha = 0;
            totalScoreLabel.alpha = totalScoreValue.alpha = 0;
            rankText.alpha = 0;

            retryButtonCanvas.alpha = 0;

            if (_gameController != null)
            {
                scoreValue.text = $"{(int)_gameController.Score}";
                goodValue.text = $"{_gameController.GoodCount}";

                if (_gameController.MaxCombo > 0)
                {
                    maxComboValue.text = _gameController.MaxCombo.ToString() + " x2";
                }
                else
                {
                    maxComboValue.text = $"{_gameController.MaxCombo}";
                }
            }
            else
            {
                scoreValue.text = $"{23}";
                goodValue.text = $"{13}";
                maxComboValue.text = $"{7} x2";
            }
        }

        void Animation(UnityEngine.Events.UnityAction completeCallback)
        {
            // ウィンドウ
            var sequence = DOTween.Sequence()
                .Append(windowRect.DOAnchorPosY(-3000, 1).SetEase(Ease.OutQuad).From(true));

            // リザルト
            sequence
                .Append(resultTitle.DOFade(1f, 1));

            // スコア
            sequence
                .Append(scoreLabel.DOFade(1f, 0.5f))
                .Append(scoreValue.DOFade(1f, 0.5f))
                //.Join(scoreValue.rectTransform.DOAnchorPosY(-30f, 0.2f).SetEase(Ease.OutQuad).From(true))
                .AppendInterval(0.2f);

            // Good
            sequence
                .Append(goodLabel.DOFade(1f, 0.5f))
                .Append(goodValue.DOFade(1f, 0.5f))
                //.Join(goodValue.rectTransform.DOAnchorPosY(-30f, 0.2f).SetEase(Ease.OutQuad).From(true))
                .AppendInterval(0.2f);

            // Max コンボ
            sequence
                .Append(maxComboLabel.DOFade(1f, 0.5f))
                .Append(maxComboValue.DOFade(1f, 0.5f))
                //.Join(maxComboValue.rectTransform.DOAnchorPosY(-30f, 0.2f).SetEase(Ease.OutQuad).From(true))
                .AppendInterval(0.2f);

            // 合計スコア
            var totalScore = _gameController ? _gameController.TotalScore : 5;

            sequence
                .Append(totalScoreLabel.DOFade(1f, 0.5f))
                .Append(totalScoreValue.DOFade(1, 1f))
                //.Join(DOVirtual.Float(0, totalScore, 1f, (value) => { totalScoreValue.text = $"{(int)value}"; }))
                .Join(DOTween.To(() => 0, x => totalScoreValue.text = $"{x}", totalScore, 1f))
                .AppendCallback(() => totalScoreValue.transform.localScale = Vector3.one * 1.25f)
                .Append(totalScoreValue.transform.DOScale(Vector3.one, 0.5f));

            // ハイスコア演出

            // リトライボタンとランキングを表示する
            sequence
                .Append(retryButtonCanvas.DOFade(1, 0.2f))
                .AppendCallback(() => completeCallback.Invoke());

            sequence.Play();
        }

        void Start()
        {
            var go = GameObject.FindGameObjectWithTag("GameController");
            if (go != null)
            {
                go.TryGetComponent(out _gameController);

                Initialize();

                Animation(() =>
                {
                    if (_gameController)
                    {
                        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(_gameController.TotalScore);
                    }

                    retryButtonCanvas.transform.DOScale(Vector3.one * 1.1f, 1).SetLoops(-1, LoopType.Yoyo);
                });

                //naichilab.RankingLoader.Instance.SendScoreAndShowRanking(_gameController.Score);
            }
        }

        [ContextMenu("TestPlay")]
        public void TestPlay()
        {
            if (Application.isPlaying)
            {
                Initialize();

                Animation(() =>
                {
                    if (_gameController)
                    {
                        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(_gameController.TotalScore);
                    }
                });
            }
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
