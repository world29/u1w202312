using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace u1w202312
{
    public class UIRailroadSwitchPresenter : MonoBehaviour
    {
        [SerializeField]
        public Button switchButton;

        [SerializeField]
        private Animator switchAnimator;

        [SerializeField]
        private RailroadSwitch _railroadSwitch;

        void Start()
        {
            switchButton.onClick.AddListener(() =>
            {
                if (_railroadSwitch.NextBranch == RailroadBranchs.Left)
                {
                    switchAnimator.SetTrigger("toright");
                }
                else
                {
                    switchAnimator.SetTrigger("toleft");
                }

                _railroadSwitch.Toggle();

                switchButton.enabled = false;
                DOVirtual.DelayedCall(.2f, () => switchButton.enabled = true);
            });
        }
    }
}
