using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class Parallax : MonoBehaviour
    {
        [SerializeField]
        private GameObject cam;

        [SerializeField]
        private float parallaxEffect;

        [SerializeField]
        private float parallaxEffectY = 1f;

        [SerializeField]
        private float cameraOrthoSize = 4f;

        [SerializeField]
        private float cameraAspect = 16f / 9f;

        [SerializeField]
        private float margin = 5f;

        [SerializeField]
        private float width = 1f;

        private float startPos;
        private float startPosY;
        private float camStartPosY;

        void Start()
        {
            startPos = transform.position.x;
            startPosY = transform.position.y;
            camStartPosY = cam.transform.position.y;
        }

        void LateUpdate()
        {
            float distance = (cam.transform.position.x * parallaxEffect);

            float distanceY = ((cam.transform.position.y - camStartPosY) * parallaxEffectY);

            transform.position = new Vector3(startPos + distance, startPosY + distanceY, transform.position.z);

            // カメラ範囲外に見切れたら次の位置に移動する
            var halfWidth = cameraAspect * cameraOrthoSize;
            var cameraLeft = cam.transform.position.x - halfWidth;

            var right = transform.position.x + width * 0.5f;

            if ((right + margin) < cameraLeft)
            {
                startPos += width * 3;
            }
        }
    }
}
