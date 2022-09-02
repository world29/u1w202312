using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity1Week.TopDown
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 5f;

        [SerializeField]
        private Transform aimTransform;

        private Vector3 _move;

        public void OnMove(InputAction.CallbackContext context)
        {
            _move = context.ReadValue<Vector2>();
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                Debug.Log($"Fire");
            }
        }

        void Update()
        {
            // 移動
            transform.Translate(_move * moveSpeed * Time.deltaTime);

            // マウス
            var mousePosition = Mouse.current.position.ReadValue();
            var worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -Camera.main.transform.position.z));

            Debug.Log($"{worldPoint}");

            var toDirection = (worldPoint - transform.position);
            //aimTransform.rotation = Quaternion.LookRotation(aimDir, Vector3.toDirection);
            aimTransform.rotation = Quaternion.FromToRotation(Vector3.up, toDirection);
        }
    }
}
