using UnityEngine;

using PathCreation;

using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

namespace u1w202312
{
    public enum HumanType { Young, Old }

    public class RailroadEventObject : MonoBehaviour
    {
        [SerializeField]
        private HumanType humanType;
        public GameObject Audio_Object;

        [SerializeField]
        private Vector2 forceOnHit;

        [SerializeField]
        private float torqueOnHit;

        private bool _isAlive = true;
        private AudioSource audioSource;

        private RailroadGameController _controller;

        private void Start()
        {
            var go = GameObject.FindGameObjectWithTag("GameController");
            Debug.Assert(go != null);
            go.TryGetComponent<RailroadGameController>(out _controller);
            Debug.Assert(_controller != null);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Instantiate(Audio_Object, transform.position, transform.rotation);
                _controller.SelectHuman(humanType, _controller.SelectedHumanCount + 1);

                _isAlive = false;

                var rb = GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.AddForce(forceOnHit, ForceMode.Impulse);
                rb.AddTorque(new Vector3(0, 0, torqueOnHit), ForceMode.Impulse);

                GetComponent<Collider>().enabled = false;


                DOVirtual.DelayedCall(5f, () => Destroy(gameObject));
            }
        }
    }
}