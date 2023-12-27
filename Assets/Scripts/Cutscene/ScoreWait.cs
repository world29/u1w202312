using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


namespace u1w202312
{
    // スコアが規定値を超えるまで待機する
    public class ScoreWait : MonoBehaviour
    {
        [SerializeField]
        private float score = 100f;

        [HideInInspector]
        public bool IsWaiting => !_controller.IsEventReady;

        private RailroadGameController _controller;

        void Start()
        {
            var go = GameObject.FindGameObjectWithTag("GameController");
            Debug.Assert(go != null);
            go.TryGetComponent<RailroadGameController>(out _controller);
            Debug.Assert(_controller != null);
        }
    }
}
