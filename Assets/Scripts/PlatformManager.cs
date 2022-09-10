using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity1Week
{
    public interface IPlatformEvent : IEventSystemHandler
    {
        void OnPlatformSpawned(Transform prevPlatform, Transform platform);
    }

    public class PlatformManager : MonoBehaviour
    {
        [SerializeField]
        private PlatformSetting setting;

        private Transform _prevPlatform;

        private GameController _gameController;

        void Start()
        {
            GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out _gameController);
        }

        public void GetPlatformSpawnParams(out float size, out bool isMovingPlatform)
        {
            var phaseTable = setting.phaseTable;

            int idx = phaseTable.Count - 1;
            for (; 0 < idx; idx--)
            {
                if (_gameController.Score > phaseTable[idx].score)
                {
                    break;
                }
            }

            var sizeMin = phaseTable[idx].size - phaseTable[idx].sizeRange * 0.5f;
            var sizeMax = phaseTable[idx].size + phaseTable[idx].sizeRange * 0.5f;

            var t = NormDist01();
            size = Mathf.Lerp(sizeMin, sizeMax, t);

            isMovingPlatform = (Random.Range(0, 100) < phaseTable[idx].percentMove);
        }

        public float GetPlatformSpawnInterval()
        {
            var phaseTable = setting.phaseTable;

            int idx = phaseTable.Count - 1;
            for (; 0 < idx; idx--)
            {
                if (_gameController.Score > phaseTable[idx].score)
                {
                    break;
                }
            }

            return phaseTable[idx].interval;
        }

        public void NotifyPlatformSpawned(Transform platform)
        {
            if (_prevPlatform != null)
            {
                BroadcastExecuteEvents.Execute<IPlatformEvent>(null,
                    (handler, _) => handler.OnPlatformSpawned(_prevPlatform, platform));
            }

            _prevPlatform = platform;
        }

        private static float NormDist01()
        {
            return Mathf.Clamp01((NormalDistribution() + 3f) / 6f);
        }

        private static float NormalDistribution()
        {
            var ret = 0f;
            for (int i = 0; i < 12; i++)
            {
                ret += Random.value;
            }
            return ret - 6f;
        }
    }
}
