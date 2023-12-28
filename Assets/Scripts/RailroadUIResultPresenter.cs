using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using unityroom.Api;

namespace u1w202312
{
    public class RailroadUIResultPresenter : MonoBehaviour
    {
        [SerializeField] private RectTransform windowRect;

        [SerializeField] private TextMeshProUGUI resultTitle;

        [SerializeField] private TextMeshProUGUI totalScoreLabel;
        [SerializeField] private TextMeshProUGUI totalScoreLabelUnit;

        [SerializeField] private TextMeshProUGUI totalScoreValue;

        [SerializeField] private CanvasGroup retryButtonCanvas;

        [SerializeField] private AudioClip bgm;

        private RailroadGameController _gameController;
        private Sequence _sequence;

        public void SkipAnimation()
        {
            if (_sequence != null)
            {
                _sequence.Complete(true);
            }
        }

        void Initialize()
        {
            resultTitle.alpha = 0;
            totalScoreLabel.alpha = totalScoreLabelUnit.alpha = totalScoreValue.alpha = 0;

            retryButtonCanvas.alpha = 0;
        }

        void Animation(UnityEngine.Events.UnityAction completeCallback)
        {
            if (bgm != null)
            {
                Unity1Week.SoundManager.PlayBgm(bgm, 0);
            }

            // ウィンドウ
            _sequence = DOTween.Sequence()
                .Append(windowRect.DOAnchorPosY(-3000, 1).SetEase(Ease.OutQuad).From(true));

            // リザルト
            _sequence
                .Append(resultTitle.DOFade(1f, 1));

            // 合計スコア
            float totalScore = _gameController ? _gameController.DistanceTravelled : 123.4f;

            _sequence
                .Append(totalScoreLabel.DOFade(1f, 0.5f))
                .Join(totalScoreLabelUnit.DOFade(1f, 0.5f))
                .Append(totalScoreValue.DOFade(1, 1f))
                //.Join(DOVirtual.Float(0, totalScore, 1f, (value) => { totalScoreValue.text = $"{(int)value}"; }))
                .Join(DOTween.To(() => 0, x => totalScoreValue.text = x.ToString("F0"), totalScore, 1f))
                .AppendCallback(() => totalScoreValue.transform.localScale = Vector3.one * 1.25f)
                .Append(totalScoreValue.transform.DOScale(Vector3.one, 0.5f));

            // ハイスコア演出

            // リトライボタンとランキングを表示する
            _sequence
                .Append(retryButtonCanvas.DOFade(1, 0.2f))
                .AppendCallback(() => completeCallback.Invoke());

            _sequence.Play();
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
                        UnityroomApiClient.Instance.SendScore(1, _gameController.DistanceTravelled, ScoreboardWriteMode.Always);
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
                        //naichilab.RankingLoader.Instance.SendScoreAndShowRanking(_gameController.TotalScore);
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
