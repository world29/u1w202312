﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace u1w202312
{
    public class RailroadGameController : MonoBehaviour, IRailroadGameControllerRequests
    {
        [SerializeField]
        public PathFollower2D pathFollower;

        [SerializeField]
        public float initialFuel = 30f;

        // 残り時間の増加量
        [SerializeField]
        public float fuelIncreasing = 5f;

        [SerializeField]
        public float initialSpeed = 3f;

        [SerializeField]
        public float speedUpFactor = 0.1f;

        [SerializeField]
        public float maxSpeed = 8f;

        [HideInInspector]
        public float DistanceTravelled { get { return _distanceTravelled; } }

        [HideInInspector]
        public float Fuel { get { return _currentFuel; } }

        private float _distanceTravelled;

        private float _currentFuel;

        private void Start()
        {
            Debug.Assert(pathFollower != null);

            pathFollower.speed = initialSpeed;

            _distanceTravelled = 0;
            _currentFuel = initialFuel;
        }

        private void Update()
        {
            if (_currentFuel > 0f)
            {
                _currentFuel -= Time.deltaTime;

                if (_currentFuel < 0f)
                {
                    _currentFuel = 0f;
                }
            }

            // ゲームオーバー
            if (_currentFuel <= 0f)
            {
                pathFollower.enabled = false;
            }
        }

        void OnEnable()
        {
            //todo: 名前空間を変更する
            Unity1Week.BroadcastReceivers.RegisterBroadcastReceiver<IRailroadGameControllerRequests>(gameObject);
        }

        void OnDisable()
        {
            Unity1Week.BroadcastReceivers.UnregisterBroadcastReceiver<IRailroadGameControllerRequests>(gameObject);
        }

        // 走行距離更新
        // PathFollower2D から呼び出される
        public void OnUpdateDistanceTravelled(float distanceTravelled)
        {
            _distanceTravelled = distanceTravelled;
        }

        // アイテムを拾った
        public void OnItemPickup(Vector3 itemPosition, ItemType itemType)
        {
            if (itemType == ItemType.SpeedUp)
            {
                var newSpeed = pathFollower.speed * (1f + speedUpFactor);
                pathFollower.speed = Mathf.Min(newSpeed, maxSpeed);
            }
            else
            {
                _currentFuel += fuelIncreasing;

                if (_currentFuel > initialFuel)
                {
                    _currentFuel = initialFuel;
                }
            }
        }

        // リトライ
        public void Retry()
        {
            Debug.Log("Retry");
        }

        // タイトルへ戻る
        public void BackToTitle()
        {
            Debug.Log("Back to Title");
        }
    }
}