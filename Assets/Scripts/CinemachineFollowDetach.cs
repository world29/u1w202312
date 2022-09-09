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

        public void DetachFollowTarget()
        {
            vcam.m_Follow = null;
        }
    }
}
