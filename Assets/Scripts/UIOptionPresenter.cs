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
        private Slider bgmSlider;

        [SerializeField]
        private Slider seSlider;

        [SerializeField]
        private TMP_Dropdown frameRateDropdown;

        private GameController _gameController;

        public void CloseOptionDialog()
        {
            // 設定を保存してシーン終了
            _gameController.Save();

            UISceneLoader.UnloadUIScene("OptionScene");
        }

        void Start()
        {
            GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out _gameController);

            bgmSlider.value = _gameController.BgmVolume;
            seSlider.value = _gameController.SeVolume;

            var index = frameRateDropdown.options.FindIndex((opt) => opt.text == _gameController.FrameRate.ToString());
            frameRateDropdown.value = index;

            bgmSlider.onValueChanged.AddListener((volume) =>
            {
                _gameController.BgmVolume = volume;
                Debug.Log($"bgm volume changed. {volume}");
            });

            seSlider.onValueChanged.AddListener((volume) =>
            {
                _gameController.SeVolume = volume;
                Debug.Log($"se volume changed. {volume}");
            });

            frameRateDropdown.onValueChanged.AddListener((index) =>
            {
                int framerate = System.Int32.Parse(frameRateDropdown.options[index].text);
                _gameController.FrameRate = framerate;
                Debug.Log($"frameRate changed. {framerate}");
            });
        }
    }
}
