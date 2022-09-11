using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Unity1Week
{
    // スコアに対応したフェーズによってスプライトを切り替える
    public class PhaseShift : MonoBehaviour
    {
        [SerializeField] private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

        [SerializeField] private float duration;

        private int _prevPhase;
        private GameController _gameController;

        void Awake()
        {
            GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out _gameController);
        }

        void Start()
        {
            _prevPhase = 0;

            _gameController.OnPhaseChanged.AddListener((phase) => ChangePhase(phase));

            ResetState();
        }

        private void ResetState()
        {
            // 全部のスプライトのアルファ1に
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.gameObject.SetActive(true);
                spriteRenderer.color = Color.white;
            }
        }

        private void ChangePhase(int newPhase)
        {
            // 現在のフェーズに対応するスプライトをフェードアウト
            spriteRenderers[_prevPhase].DOFade(0, duration);

            _prevPhase = newPhase;
        }
    }
}
