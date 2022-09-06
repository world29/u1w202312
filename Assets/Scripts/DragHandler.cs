using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity1Week
{
    public interface ICustomDragEvent : IEventSystemHandler
    {
        void OnBeginDrag(Vector2 screenPos);
        void OnEndDrag(Vector2 screenPos);
        void OnDrag(Vector2 screenPos, Vector2 beginScreenPos);
    }

    public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Vector2 _beginPos;

        public void OnBeginDrag(PointerEventData eventData)
        {
            _beginPos = eventData.position;

            BroadcastExecuteEvents.Execute<ICustomDragEvent>(null,
                (handler, _) => handler.OnBeginDrag(eventData.position));
        }

        public void OnDrag(PointerEventData eventData)
        {
            BroadcastExecuteEvents.Execute<ICustomDragEvent>(null,
                (handler, _) => handler.OnDrag(eventData.position, _beginPos));
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            BroadcastExecuteEvents.Execute<ICustomDragEvent>(null,
                (handler, _) => handler.OnEndDrag(eventData.position));
        }
    }
}
