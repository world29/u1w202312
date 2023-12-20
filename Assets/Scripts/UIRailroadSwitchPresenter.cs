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

        private TextMeshProUGUI _switchButtonText;

        void Start()
        {
            _switchButtonText = switchButton.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Assert(_switchButtonText != null);

            switchButton.onClick.AddListener(() =>
            {
                _railroadSwitch.Toggle();

                if (_railroadSwitch.NextBranch == RailroadBranchs.Left)
                {
                    _switchButtonText.text = "↑";
                }
                else
                {
                    _switchButtonText.text = "↓";
                }
            });
        }
    }
}
