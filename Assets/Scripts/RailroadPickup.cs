using UnityEngine;

using PathCreation;

using System.Collections;
using System.Collections.Generic;

namespace u1w202312
{
    public class RailroadPickup : MonoBehaviour
    {
        [SerializeField] float amplitude = 0.1f; // 振幅
        [SerializeField] float period = 1f; // 周期

        private Vector3 _initialPosition;

        private void Start()
        {
            _initialPosition = transform.position;
        }

        private void Update()
        {
            // 上下に揺れる
            var offset = Mathf.Cos((Mathf.PI * 2 / period) * Time.timeSinceLevelLoad) * amplitude;

            transform.position = new Vector3(_initialPosition.x, _initialPosition.y + offset, _initialPosition.z);
        }
    }
}