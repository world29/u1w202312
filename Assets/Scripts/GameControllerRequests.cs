using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity1Week
{
    public interface IGameControllerRequests : IEventSystemHandler
    {
        void AddScore(float scoreToAdd);

        void OnItemPickup(Vector3 itemPosition, float score);

        void OnLandedPlatform(Vector3 landingPosition, bool isGood, int landedCount);

        void OnLeftPlatform();

        void Retry();

        void BackToTitle();

        void Pause();

        void Resume();

        void Save();

        void Load();
    }
}
