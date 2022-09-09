using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class DamageArea : MonoBehaviour
    {
        [SerializeField]
        private GameEvent damageEvent;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                damageEvent.Raise();
            }
        }
    }
}
