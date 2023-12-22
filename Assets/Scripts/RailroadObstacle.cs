using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace u1w202312
{
    public class RailroadObstacle : MonoBehaviour
    {
        [SerializeField]
        private AudioClip se;

        [SerializeField]
        private ParticleSystem particlePrefab;

        [SerializeField]
        private UnityEvent onHit;

        private bool _hitNotified;

        void Start()
        {
            _hitNotified = false;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (!_hitNotified)
                {
                    Unity1Week.BroadcastExecuteEvents.Execute<IRailroadGameControllerRequests>(null,
                        (handler, eventData) => handler.OnObstacleHit(transform.position));

                    _hitNotified = true;
                }

                if (se != null)
                {
                    Unity1Week.SoundManager.PlaySe(se);
                }

                if (particlePrefab != null)
                {
                    Instantiate<ParticleSystem>(particlePrefab, transform.position, Quaternion.identity);
                }

                onHit?.Invoke();

                Destroy(gameObject);
            }
        }
    }
}
