using UnityEngine;

using PathCreation;

using System.Collections;
using System.Collections.Generic;

namespace u1w202312
{
    // カメラとの距離を固定する
    public class RailroadFixCameraDistance : MonoBehaviour
    {
        private Camera _mainCamera;

        private float _distanceFromCamera;

        private void OnEnable()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            // ここでカメラとの距離を保存する
            _distanceFromCamera = transform.position.z - _mainCamera.transform.position.z;
        }

        private void Update()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, _mainCamera.transform.position.z + _distanceFromCamera);
        }
    }
}