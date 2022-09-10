using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class ParticleSystemPlayer : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem particle1;
        [SerializeField]
        private ParticleSystem particle2;

        private void PlayParticle(ParticleSystem particle)
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
        public void PlayParticle1()
        {
            PlayParticle(particle1);

        }
        public void PlayParticle2()
        {
            PlayParticle(particle2);

        }
    }
}