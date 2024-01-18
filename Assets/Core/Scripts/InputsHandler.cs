using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Scripts
{
    public class InputsHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private static InputsHandler _instance;
        public static InputsHandler Instance => _instance;
        private Vector2 _swipeStartPosition;
        public Action<Vector2Int> OnDirectionSwiped;
        
        private void Awake()
        {
            _instance = this;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _swipeStartPosition = eventData.pressPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            var direction = (eventData.position - _swipeStartPosition).normalized;
            Vector2Int convertedDirection;
            if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
                convertedDirection = direction.y > 0 ? Vector2Int.up : Vector2Int.down;
            else
                convertedDirection = direction.x > 0 ? Vector2Int.right : Vector2Int.left;
            OnDirectionSwiped?.Invoke(convertedDirection);
        }
    }
}