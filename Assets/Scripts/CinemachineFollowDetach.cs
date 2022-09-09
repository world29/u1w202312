using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    [ExecuteAlways]
    public class CinemachineFollowDetach : MonoBehaviour
    {
        [SerializeField]
        private Cinemachine.CinemachineVirtualCamera vcam;

        [SerializeField]
        private Transform followTarget;

        public void DetachFollowTarget()
        {
            vcam.m_Follow = null;
        }

        public void AttachFollowTarget()
        {
            vcam.m_Follow = followTarget;
        }
    }
}
