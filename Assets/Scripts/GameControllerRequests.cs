using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity1Week
{
    public interface IGameControllerRequests : IEventSystemHandler
    {
        void AddScore(float scoreToAdd);

        void OnLandedPlatform(Vector3 landingPosition, float distance, int landedCount);

        void OnLeftPlatform();

        void Retry();

        void BackToTitle();

        void Pause();

        void Resume();
    }
}
