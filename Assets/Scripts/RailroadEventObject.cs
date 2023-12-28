using UnityEngine;

using PathCreation;

using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

namespace u1w202312
{
    public class RailroadEventObject : MonoBehaviour
    {
        [SerializeField]
        private Vector2 forceOnHit;

        [SerializeField]
        private float torqueOnHit;

        private bool _isAlive = true;
        private AudioSource audioSource;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isAlive = false;

                var rb = GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.AddForce(forceOnHit, ForceMode.Impulse);
                rb.AddTorque(new Vector3(0, 0, torqueOnHit), ForceMode.Impulse);

                GetComponent<Collider>().enabled = false;

                audioSource = gameObject.GetComponent<AudioSource>();
                audioSource.Play();
                DOVirtual.DelayedCall(5f, () => Destroy(gameObject));
            }
        }
    }
}