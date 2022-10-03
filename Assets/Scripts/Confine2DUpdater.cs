using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class Confine2DUpdater : MonoBehaviour
    {
        [SerializeField]
        private PlayerMovement playerMovement;

        [SerializeField]
        private bool demoEnabled;

        [SerializeField]
        private float scrollSpeed = 0.1f;

        void LateUpdate()
        {
            if (demoEnabled)
            {
                transform.Translate(scrollSpeed * Time.deltaTime, 0, 0);
                return;
            }

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
