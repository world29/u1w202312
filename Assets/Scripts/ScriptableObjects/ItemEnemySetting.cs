using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity1Week
{
    [CreateAssetMenu]
    public class ItemEnemySetting : ScriptableObject
    {
        [System.Serializable]
        public class ItemInfo
        {
            public Item prefab;

            public int weight;
        }

        [System.Serializable]
        public class EnemyInfo
        {
            public GameObject prefab;

            public int weight;
        }

        [System.Serializable]
        public struct Phase
        {
            [Tooltip("このスコア以上のとき、以下の設定値が採用される")]
            public float score;

            public float itemDropPercent;

            public List<ItemInfo> itemLootTable;

            public float enmeySpawnPercent;

            public List<EnemyInfo> enemyLootTable;
        }

        [SerializeField]
        public List<Phase> phaseTable = new List<Phase>();
    }
}
