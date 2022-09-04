using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    // タイムラインから BGM を再生するのに使う
    public class BgmPlayer : MonoBehaviour
    {
        [SerializeField]
        private AudioClip bgm;

        [SerializeField]
        private float fadeTime = 0f;

        void Start()
        {
            PlayBgm(bgm);
        }

        public void PlayBgm(AudioClip clip)
        {
            SoundManager.PlayBgm(clip, fadeTime);
        }
    }
}
