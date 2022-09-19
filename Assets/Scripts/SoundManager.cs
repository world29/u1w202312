using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;

namespace Unity1Week
{
    public class SoundManager : MonoBehaviour
    {
        // static methods
        public static void PlaySe(AudioClip clip)
        {
            Instance.PlaySeImpl(clip, 1);
        }

        public static void PlaySeWithPitch(AudioClip clip, float pitch)
        {
            Instance.PlaySeImpl(clip, pitch);
        }

        /// <summary>
        /// BGM を再生する
        /// </summary>
        /// <param name="assetAddress"></param>
        /// <param name="fadeInTime"></param>
        public static void PlayBgm(AudioClip clip, float fadeInTime)
        {
            Instance.PlayBgmImpl(clip, fadeInTime);
        }

        public static void SetVolume(float volume, string group = "Master")
        {
            var dB = Mathf.Clamp(Mathf.Log10(volume) * 20f, -80f, 0f);
            Instance._mixer.SetFloat(group, dB);
        }

        /// <summary>
        /// 再生されている BGM を停止する
        /// </summary>
        /// <param name="fadeOutTime"></param>
        public static void StopBgm(float fadeOutTime)
        {
            Instance.StopBgmImpl(fadeOutTime);
        }

        private static SoundManager _instance;
        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var previous = FindObjectOfType<SoundManager>();
                    if (previous)
                    {
                        Debug.LogWarning("Initialized twice. Don't use SoundManager in scene hierarchy.");
                        _instance = (SoundManager)previous;
                    }
                    else
                    {
                        var go = new GameObject("__SoundManager (singleton)");
                        _instance = go.AddComponent<SoundManager>();
                        DontDestroyOnLoad(go);
                        go.hideFlags = HideFlags.HideInHierarchy;
                    }
                }

                return _instance;
            }
        }

        static readonly string k_AudioMixerPath = "AudioMixer";
        const int SE_CHANNELS = 8;

        private AudioMixer _mixer;
        private AudioSource _bgmSource;
        private AudioSource[] _seSources = new AudioSource[SE_CHANNELS];

        private void Awake()
        {
            _mixer = Resources.Load<AudioMixer>(k_AudioMixerPath);

            AudioMixerGroup[] mixerBgmGroups = _mixer.FindMatchingGroups("BGM");
            AudioMixerGroup[] mixerSeGroups = _mixer.FindMatchingGroups("SE");

            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.loop = true;
            _bgmSource.playOnAwake = false;
            _bgmSource.outputAudioMixerGroup = mixerBgmGroups[0];

            for (int i = 0; i < SE_CHANNELS; i++)
            {
                _seSources[i] = gameObject.AddComponent<AudioSource>();
                _seSources[i].loop = false;
                _seSources[i].playOnAwake = false;
                _seSources[i].outputAudioMixerGroup = mixerSeGroups[0];
            }
        }

        private void PlaySeImpl(AudioClip clip, float pitch)
        {
            var audioSource = _seSources.FirstOrDefault(x => !x.isPlaying);
            if (audioSource)
            {
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"cannot play sound. {clip.name}");
            }
        }

        private void PlayBgmImpl(AudioClip clip, float fadeInTime)
        {
            StartCoroutine(FadeInBgmCoroutine(clip, fadeInTime));
        }

        private void StopBgmImpl(float fadeOutTime)
        {
            if (!_bgmSource.isPlaying)
            {
                return;
            }

            StartCoroutine(FadeOutBgmCoroutine(fadeOutTime));
        }

        private IEnumerator FadeInBgmCoroutine(AudioClip clip, float fadeInTime)
        {
            Debug.Assert(_bgmSource != null);

            _bgmSource.volume = 0;
            _bgmSource.clip = clip;
            _bgmSource.Play();

            float timer = 0;
            while (timer < fadeInTime)
            {
                yield return null;

                _bgmSource.volume = timer / fadeInTime;
                timer += Time.deltaTime;
            }

            _bgmSource.volume = 1.0f;
        }

        private IEnumerator FadeOutBgmCoroutine(float fadeOutTime)
        {
            Debug.Assert(_bgmSource != null);

            float timer = 0;
            while (timer < fadeOutTime)
            {
                yield return null;

                _bgmSource.volume -= (timer / fadeOutTime);
                timer += Time.deltaTime;
            }

            _bgmSource.volume = 0;
            _bgmSource.Stop();
            _bgmSource.clip = null;
        }
    }
}
