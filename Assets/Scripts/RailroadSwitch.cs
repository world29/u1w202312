using UnityEngine;

using PathCreation;

using System.Collections;
using System.Collections.Generic;

namespace u1w202312
{
    // 線路の分岐切り替え
    public class RailroadSwitch : MonoBehaviour
    {
        public PathCreator GetNextPath(PathCreator currentPath)
        {
            var railroad = currentPath.GetComponent<Railroad>();
            Debug.Assert(railroad != null);

            Debug.Assert(railroad.next1 != null);

            // 分岐ありならランダムでいずれかを選ぶ
            if (railroad.next2 != null)
            {
                float rand = Random.Range(0f, 1f);
                if (rand > 0.5f)
                {
                    return railroad.next2.path;
                }
            }

            return railroad.next1.path;
        }
    }
}