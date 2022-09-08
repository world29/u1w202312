﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity1Week
{
    public interface IGameControllerRequests : IEventSystemHandler
    {
        void AddScore(float scoreToAdd);

        void OnLandingPlatform(Vector3 landingPosition, float distance);

        void Retry();

        void BackToTitle();

        void Pause();

        void Resume();
    }
}
