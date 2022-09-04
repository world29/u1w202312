using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity1Week
{
    public interface IGameControllerRequests : IEventSystemHandler
    {
        void AddScore(float scoreToAdd);
    }
}
