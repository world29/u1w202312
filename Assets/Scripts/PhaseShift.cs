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
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.gameObject.SetActive(true);
                spriteRenderer.enabled = false;
            }

            spriteRenderers[0].enabled = true;
        }

        private void ChangePhase(int newPhase)
        {
            // 現在のフェーズに対応するスプライトをフェードアウト
            spriteRenderers[_prevPhase].DOFade(0, duration);

            spriteRenderers[newPhase].enabled = true;

            // アルファを 0 から初期値へ
            var col = spriteRenderers[newPhase].color;
            var targetAlpha = col.a;
            spriteRenderers[newPhase].color = new Color(col.r, col.g, col.b, 0);
            spriteRenderers[newPhase].DOFade(targetAlpha, duration);

            _prevPhase = newPhase;
        }
    }
}
