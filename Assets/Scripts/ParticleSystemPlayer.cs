using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class ParticleSystemPlayer : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem particle;

        public void PlayParticle()
        {
            if (particle == null)
            {
                particle = GetComponent<ParticleSystem>();
            }

            if (particle)
            {
                particle.Play();
            }

        }
    }
}