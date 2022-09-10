using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity1Week
{
    [CreateAssetMenu]
    public class GameplayConfig : ScriptableObject
    {
        [SerializeField]
        public bool SkipTimeline = false;
    }
}
