using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class Cloud : MonoBehaviour
    {
        [SerializeField]
        private float speed = 1f;

        [SerializeField]
        private float yMin = 2;

        [SerializeField]
        private float yMax = 3;

        [SerializeField]
        private Camera cam;

        private float startPosY;
        private float camStartPosY;

        private SpriteRenderer _spriteRenderer;

        void Awake()
        {
            TryGetComponent(out _spriteRenderer);

            startPosY = transform.position.y;
            camStartPosY = cam.transform.position.y;
        }

        void Update()
        {
            var offsetY = cam.transform.position.y - camStartPosY;
            transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, startPosY + offsetY, transform.position.z);

            var cameraLeft = cam.transform.position.x - cam.aspect * cam.orthographicSize;

            if (_spriteRenderer.bounds.max.x < cameraLeft)
            {
                var cameraRight = cam.transform.position.x + cam.aspect * cam.orthographicSize;
                var newPosX = cameraRight + _spriteRenderer.bounds.extents.x;
                var newPosY = Random.Range(yMin, yMax);

                transform.position = new Vector3(newPosX, newPosY, transform.position.z);
            }
        }
    }
}
