using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace u1w202312
{
    public class BlinkCanvasGroup : MonoBehaviour
    {
        public float durationSeconds;
        public Ease easeType;

        private CanvasGroup _canvasGroup;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            _canvasGroup.DOFade(0.0f, durationSeconds).SetEase(easeType).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
