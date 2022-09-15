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

#if UNITY_EDITOR
        void Start()
        {
            Debug.Log($"{Screen.width}, {Screen.height}");
            Debug.Log($"{Screen.safeArea.x}, {Screen.safeArea.y}");
            Debug.Log($"{Screen.safeArea.width}, {Screen.safeArea.height}");
        }
#endif

        private void FixAspect()
        {
            Camera[] cameras = this.GetComponentsInChildren<Camera>();
            float gameAspect = 16f / 9f;

            var safeArea = Screen.safeArea;

            float screenAspect = (float)safeArea.width / (float)safeArea.height;
            float diff = gameAspect - screenAspect;

            if (diff > Vector3.kEpsilon)
            {
                float height = (float)safeArea.width / gameAspect;
                float y = (safeArea.height - height) * 0.5f;
                Rect pixelRect = new Rect(safeArea.x, safeArea.y + y, safeArea.width, height);
                for (int i = 0; i < cameras.Length; ++i)
                {
                    cameras[i].pixelRect = pixelRect;
                }
            }
            else
            {
                float width = (float)safeArea.height * gameAspect;
                float x = (safeArea.width - width) * 0.5f;
                Rect pixelRect = new Rect(safeArea.x + x, safeArea.y, width, safeArea.height);
                for (int i = 0; i < cameras.Length; ++i)
                {
                    cameras[i].pixelRect = pixelRect;
                }
            }
        }
    }
}
