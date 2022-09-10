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

    [System.Serializable]
    public struct PlatformPhase
    {
        // このスコア以上なら、以下の設定値が採用される
        public float score;

        public float widthMin;

        public float widthMax;

        public float interval;
    }

    public class PlatformManager : MonoBehaviour
    {
        [SerializeField]
        private List<PlatformPhase> phaseTable = new List<PlatformPhase>();

        private PlatformNormal _prevPlatform;

        private GameController _gameController;

        void Start()
        {
            GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out _gameController);
        }

        public void GetPlatformWidth(out float widthMin, out float widthMax)
        {
            int idx = phaseTable.Count - 1;
            for (; 0 < idx; idx--)
            {
                if (_gameController.Score > phaseTable[idx].score)
                {
                    break;
                }
            }

            widthMin = phaseTable[idx].widthMin;
            widthMax = phaseTable[idx].widthMax;
        }

        public float GetPlatformInterval()
        {
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

        public void NotifyPlatformSpawned(PlatformNormal platform)
        {
            if (_prevPlatform != null)
            {
                BroadcastExecuteEvents.Execute<IPlatformEvent>(null,
                    (handler, _) => handler.OnPlatformSpawned(_prevPlatform, platform));
            }

            _prevPlatform = platform;
        }
    }
}
