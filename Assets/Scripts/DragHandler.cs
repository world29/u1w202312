using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Unity1Week
{
    public interface ICustomDragEvent : IEventSystemHandler
    {
        void OnBeginDrag(Vector2 screenPos);
        void OnEndDrag(Vector2 screenPos);
        void OnDrag(Vector2 screenPos, Vector2 beginScreenPos);
        void OnDragCancel();
    }

    public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        private Vector2 _beginPos;

        void Update()
        {
            // IPointerDownHandler は画面内でしか反応しないため、
            // 画面内どこでもキャンセルできるようマウス入力を直接取得する。
            // ただしこの方法だとタッチ操作はキャンセルできない。
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                BroadcastExecuteEvents.Execute<ICustomDragEvent>(null,
                    (handler, _) => handler.OnDragCancel());
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log($"OnBeginDrag: {eventData.button}");

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _beginPos = eventData.position;

                BroadcastExecuteEvents.Execute<ICustomDragEvent>(null,
                    (handler, _) => handler.OnBeginDrag(eventData.position));
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                BroadcastExecuteEvents.Execute<ICustomDragEvent>(null,
                    (handler, _) => handler.OnDrag(eventData.position, _beginPos));
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log($"OnEndDrag: {eventData.button}");

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                BroadcastExecuteEvents.Execute<ICustomDragEvent>(null,
                    (handler, _) => handler.OnEndDrag(eventData.position));
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            /*
            Debug.Log($"OnPointerDown: {eventData.button}");

            // 右クリックでキャンセル
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                BroadcastExecuteEvents.Execute<ICustomDragEvent>(null,
                    (handler, _) => handler.OnDragCancel());
            }
            */
        }
    }
}
