using UnityEngine;

using PathCreation;

using System.Collections;
using System.Collections.Generic;

namespace u1w202312
{
    public enum RailroadBranchs { Left, Right };

    // 線路の分岐切り替え
    public class RailroadSwitch : MonoBehaviour
    {
        private RailroadBranchs _nextBranch = RailroadBranchs.Left;

        [HideInInspector]
        public RailroadBranchs NextBranch { get { return _nextBranch; } }

        public PathCreator GetNextPath(PathCreator currentPath)
        {
            var railroad = currentPath.GetComponent<Railroad>();
            Debug.Assert(railroad != null);

            Debug.Assert(railroad.next1 != null);
            Debug.Assert(railroad.next2 != null);

            if (_nextBranch == RailroadBranchs.Right)
            {
                return railroad.next2.path;
            }
            return railroad.next1.path;
        }

        // 次の線路分岐での方向を切り替える
        public void Toggle()
        {
            Debug.Log("RailroadSwitch.Toggle");

            if (_nextBranch == RailroadBranchs.Left)
            {
                _nextBranch = RailroadBranchs.Right;
            }
            else
            {
                _nextBranch = RailroadBranchs.Left;
            }
        }
    }
}