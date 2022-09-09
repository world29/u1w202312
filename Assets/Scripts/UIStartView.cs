using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Unity1Week
{
    public class UIStartView : MonoBehaviour
    {
        [SerializeField]
        private Image image;

        [SerializeField]
        private float blinkDuration = 1f;

        private RectTransform _rectTransform;

        void Awake()
        {
            TryGetComponent(out _rectTransform);
        }

        void Start()
        {
            image.DOFade(0, blinkDuration).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
