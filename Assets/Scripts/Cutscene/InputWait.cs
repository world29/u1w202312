using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


namespace Unity1Week
{
    public class InputWait : MonoBehaviour, ICustomDragEvent
    {
        [HideInInspector]
        public bool IsWaiting => !_isDragBegin;

        private bool _isDragBegin;

        void Start()
        {
            _isDragBegin = false;
        }

        void OnEnable()
        {
            BroadcastReceivers.RegisterBroadcastReceiver<ICustomDragEvent>(gameObject);
        }

        void OnDisable()
        {
            BroadcastReceivers.UnregisterBroadcastReceiver<ICustomDragEvent>(gameObject);
        }

        public void OnBeginDrag(Vector2 screenPos)
        {
            _isDragBegin = true;
        }

        public void OnDrag(Vector2 screenPos, Vector2 beginScreenPos)
        {
        }

        public void OnEndDrag(Vector2 screenPos)
        {
        }

        public void OnDragCancel()
        {
        }
    }
}
