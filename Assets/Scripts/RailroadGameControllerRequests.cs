﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace u1w202312
{
    public enum ItemType { Small, Large }

    public interface IRailroadGameControllerRequests : IEventSystemHandler
    {
        // このフレームで走行した距離
        void OnTravelled(float distanceTravelled);

        // アイテムを拾った
        void OnItemPickup(Vector3 itemPosition, ItemType itemType);

        // 障害物に当たった
        void OnObstacleHit(Vector3 obstaclePosition);

        // リトライ
        void Retry();

        // タイトルへ戻る
        void BackToTitle();
    }
}
