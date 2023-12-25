using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


namespace Unity1Week
{
    public class InputWait : MonoBehaviour
    {
        [HideInInspector]
        public bool IsWaiting => !_isPointerDown;

        private bool _isPointerDown;

        void Start()
        {
            _isPointerDown = false;
        }

        public void OnPointerDown()
        {
            _isPointerDown = true;
        }
    }
}
