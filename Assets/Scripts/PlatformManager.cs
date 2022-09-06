using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity1Week
{
    public interface IPlatformEvent : IEventSystemHandler
    {
        void OnPlatformSpawned(PlatformNormal prevPlatform, PlatformNormal platform);
    }

    public class PlatformManager : MonoBehaviour
    {
        private PlatformNormal _prevPlatform;

        public void NotifyPlatformSpawned(PlatformNormal platform)
        {
            if (_prevPlatform != null)
            {
                Debug.Log("NotifyPlatformSpawned");

                BroadcastExecuteEvents.Execute<IPlatformEvent>(null,
                    (handler, _) => handler.OnPlatformSpawned(_prevPlatform, platform));
            }

            _prevPlatform = platform;
        }
    }
}
