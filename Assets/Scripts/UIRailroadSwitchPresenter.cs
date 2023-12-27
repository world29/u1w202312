using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace u1w202312
{
    public class UIRailroadSwitchPresenter : MonoBehaviour
    {
        [SerializeField]
        public Button switchButton;

        [SerializeField]
        private RailroadSwitch _railroadSwitch;

        [SerializeField]
        private List<SpriteRenderer> _sprites;

        void Start()
        {
            switchButton.onClick.AddListener(() =>
            {
                _railroadSwitch.Toggle();

                foreach (var spriteRenderer in _sprites)
                {
                    spriteRenderer.flipY = !spriteRenderer.flipY;
                }
            });
        }
    }
}
