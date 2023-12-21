using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace u1w202312
{
    public class UIRailroadFuelPresenter : MonoBehaviour
    {
        [SerializeField]
        public Slider fuelSlider;

        private RailroadGameController _controller;

        void Start()
        {
            var go = GameObject.FindGameObjectWithTag("GameController");
            Debug.Assert(go != null);
            go.TryGetComponent<RailroadGameController>(out _controller);
            Debug.Assert(_controller != null);
        }

        private void LateUpdate()
        {
            if (_controller != null)
            {
                fuelSlider.value = (_controller.Fuel / _controller.initialFuel);
            }
        }
    }
}
