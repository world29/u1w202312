using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace u1w202312
{
    public class RailroadGameController : MonoBehaviour, IRailroadGameControllerRequests
    {
        [HideInInspector]
        public float DistanceTravelled { get { return _distanceTravelled; } }

        private float _distanceTravelled;

        private void Start()
        {
            _distanceTravelled = 0;
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
            Debug.Log("On Item Pickup");
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
