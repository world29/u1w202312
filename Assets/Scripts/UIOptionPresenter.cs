using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Unity1Week
{
    public class UIOptionPresenter : MonoBehaviour
    {
        [SerializeField]
        private Options options;

        [SerializeField]
        private Slider bgmSlider;

        [SerializeField]
        private Slider seSlider;

        [SerializeField]
        private TMP_Dropdown frameRateDropdown;

        public void UnloadUIScene(string sceneName)
        {
            UISceneLoader.UnloadUIScene(sceneName);
        }

        void Start()
        {
            bgmSlider.value = options.BgmVolume;
            seSlider.value = options.SeVolume;

            var index = frameRateDropdown.options.FindIndex((opt) => opt.text == options.FrameRate.ToString());
            frameRateDropdown.value = index;

            bgmSlider.onValueChanged.AddListener((volume) =>
            {
                Debug.Log($"bgm volume changed. {volume}");
                options.BgmVolume = volume;
            });

            seSlider.onValueChanged.AddListener((volume) =>
            {
                Debug.Log($"se volume changed. {volume}");
                options.SeVolume = volume;
            });

            frameRateDropdown.onValueChanged.AddListener((index) =>
            {
                int framerate = int.Parse(frameRateDropdown.options[index].text);
                Debug.Log($"frameRate changed. {framerate}");
                options.FrameRate = framerate;
            });
        }
    }
}
