using UnityEngine;

using PathCreation;

using System.Collections;
using System.Collections.Generic;

namespace u1w202312
{
    public class RailroadTrainSmoke : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem particle;

        [SerializeField]
        private float rateOverTimePerSpeed = 10f / 5f;

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
        }

        public void OnTrainSpeedChanged(float newSpeed)
        {
            var em = particle.emission;

            em.rateOverTime = newSpeed * rateOverTimePerSpeed;
        }
    }
}