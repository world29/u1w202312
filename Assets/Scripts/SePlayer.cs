using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    // アニメーションイベントから SE を再生するのに使う
    public class SePlayer : MonoBehaviour
    {
        public void PlaySe(AudioClip clip)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            SoundManager.PlaySe(clip);
        }
    }
}
