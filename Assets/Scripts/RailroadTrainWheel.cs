using UnityEngine;

using PathCreation;

using System.Collections;
using System.Collections.Generic;

namespace u1w202312
{
    public class RailroadTrainWheel : MonoBehaviour
    {
        [SerializeField]
        private List<SpriteRenderer> wheels;

        [SerializeField]
        private float angularVelocity = 1f;

        private float _speed = 3f;
        private PathFollower2D _pathFollower;

        private void Start()
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            Debug.Assert(go != null);
            if (!go.TryGetComponent<PathFollower2D>(out _pathFollower))
            {
                Debug.Assert(false);
            }

            _pathFollower.onSpeedChanged += OnTrainSpeedChanged;
        }

        private void Update()
        {
            // 車輪の回転
            foreach (var wheel in wheels)
            {
                wheel.transform.Rotate(0, 0, -_speed * angularVelocity);
            }
        }

        public void OnTrainSpeedChanged(float newSpeed)
        {
            _speed = newSpeed;
        }
    }
}