using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class FlexibleCamera : MonoBehaviour
    {
        void Update()
        {
            FixAspect();
        }

        private void FixAspect()
        {
            Camera[] cameras = this.GetComponentsInChildren<Camera>();
            float gameAspect = 16f / 9f;
            float screenAspect = (float)Screen.width / (float)Screen.height;
            float diff = gameAspect - screenAspect;

            if (diff > Vector3.kEpsilon)
            {
                float height = (float)Screen.width / gameAspect;
                float y = (Screen.height - height) * 0.5f;
                Rect pixelRect = new Rect(0f, y, Screen.width, height);
                for (int i = 0; i < cameras.Length; ++i)
                {
                    cameras[i].pixelRect = pixelRect;
                }
            }
            else
            {
                float width = (float)Screen.height * gameAspect;
                float x = (Screen.width - width) * 0.5f;
                Rect pixelRect = new Rect(x, 0f, width, Screen.height);
                for (int i = 0; i < cameras.Length; ++i)
                {
                    cameras[i].pixelRect = pixelRect;
                }
            }
        }
    }
}
