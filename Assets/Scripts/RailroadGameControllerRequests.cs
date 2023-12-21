using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace u1w202312
{
    // 走行時間延長
    // 走行速度アップ
    public enum ItemType { FuelUp, SpeedUp }

    public interface IRailroadGameControllerRequests : IEventSystemHandler
    {
        // 走行距離更新
        // PathFollower2D から呼び出される
        void OnUpdateDistanceTravelled(float distanceTravelled);

        // アイテムを拾った
        void OnItemPickup(Vector3 itemPosition, ItemType itemType);

        // リトライ
        void Retry();

        // タイトルへ戻る
        void BackToTitle();
    }
}
