using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class Confine2DUpdater : MonoBehaviour
    {
        [SerializeField]
        private PlayerMovement playerMovement;

        void LateUpdate()
        {
            var newPos = transform.position;
            newPos.x = playerMovement.Position.x;

            // +x 方向にのみ追従する
            if (newPos.x > transform.position.x)
            {
                transform.position = newPos;
            }
        }
    }
}
