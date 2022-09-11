using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity1Week
{
    [CreateAssetMenu]
    public class PhaseSetting : ScriptableObject
    {
        [System.Serializable]
        public struct Phase
        {
            [Tooltip("このスコア以上のとき、以下の設定値が採用される")]
            public float score;
        }

        [SerializeField]
        public List<Phase> phaseTable = new List<Phase>();
    }
}
