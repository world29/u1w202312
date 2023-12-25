using UnityEngine;

using PathCreation;

using System.Collections;
using System.Collections.Generic;

namespace u1w202312
{
    public class RailroadTrigger : MonoBehaviour
    {
        [HideInInspector]
        public System.Action<Vector3> onMove;

        private void Update()
        {
            onMove?.Invoke(transform.position);
        }
    }
}