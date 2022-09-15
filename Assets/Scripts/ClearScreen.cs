using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class ClearScreen : MonoBehaviour
    {
        [SerializeField]
        private Color clearColor = Color.black;

        void OnPreRender()
        {
            GL.Viewport(new Rect(0, 0, Screen.width, Screen.height));
            GL.Clear(true, true, clearColor);
        }
    }
}
