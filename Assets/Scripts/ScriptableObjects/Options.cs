using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity1Week
{
    [CreateAssetMenu]
    public class Options : ScriptableObject
    {
        [SerializeField]
        private float bgmVolume = 1.0f;

        [SerializeField]
        private float seVolume = 1.0f;

        [SerializeField]
        private int defaultFrameRate = 60;

        public float BgmVolume
        {
            get { return bgmVolume; }
            set
            {
                bgmVolume = value;

                SoundManager.SetVolume(value, "BGM");
            }
        }

        public float SeVolume
        {
            get { return seVolume; }
            set
            {
                seVolume = value;

                SoundManager.SetVolume(value, "SE");
            }
        }

        public int FrameRate
        {
            get { return Application.targetFrameRate; }
            set
            {
                Application.targetFrameRate = value;
            }
        }

        void OnEnable()
        {
            BgmVolume = bgmVolume;
            SeVolume = seVolume;

            Application.targetFrameRate = defaultFrameRate;
        }
    }
}
