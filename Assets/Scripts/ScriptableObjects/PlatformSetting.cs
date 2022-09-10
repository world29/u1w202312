using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity1Week
{

    [CreateAssetMenu]
    public class PlatformSetting : ScriptableObject
    {
        [System.Serializable]
        public struct Phase
        {
            [Tooltip("このスコア以上のとき、以下の設定値が採用される")]
            public float score;

            [Tooltip("もともとのサイズを 1 とした倍率で指定する")]
            public float size;

            [Tooltip("サイズを多少ばらけさせたいとき")]
            public float sizeRange;

            [Tooltip("動くプラットフォームが出現する確率")]
            public float percentMove;

            [Tooltip("プラットフォームの間隔")]
            public float interval;
        }

        [SerializeField]
        public List<Phase> phaseTable = new List<Phase>();

    }
}
