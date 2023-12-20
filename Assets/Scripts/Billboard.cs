using UnityEngine;

namespace u1w202312
{
    // Y 軸固定のビルボード
    public class Billboard : MonoBehaviour
    {
        private Transform _cameraTransform;

        private void Start()
        {
            _cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            Vector3 lookDir = _cameraTransform.position - transform.position;
            lookDir.y = 0f;

            if (lookDir != Vector3.zero)
            {
                transform.forward = lookDir.normalized;
            }
        }
    }
}
