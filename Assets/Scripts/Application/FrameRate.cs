using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class FrameRate : MonoBehaviour
    {
        [SerializeField] private int frameRate = 60;
        void Awake()
        {
            Application.targetFrameRate = frameRate;
        }
    }
}
