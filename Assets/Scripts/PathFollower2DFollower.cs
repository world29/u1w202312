using UnityEngine;
using UnityEngine.Events;

using PathCreation;

namespace u1w202312
{
    public class PathFollower2DFollower : MonoBehaviour
    {
        // 先頭車両
        [SerializeField]
        public PathFollower2D pathFollower2D;

        // 一つ前の車両
        [SerializeField]
        public PathFollower2DFollower previousFollower;

        private EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Stop;

        private float offset = 0f;

        [HideInInspector]
        public PathCreator pathCreator;

        private float _distanceTravelled;

        private PathCreator PreviousPath
        {
            get
            {
                return previousFollower ? previousFollower.pathCreator : pathFollower2D.pathCreator;
            }
        }

        public void SetOffset(float value)
        {
            offset = value;
            _distanceTravelled = offset;
        }

        void Start()
        {
            Debug.Log("PathFollower2DFollower.Start");
            //_distanceTravelled = offset;
        }

        void Update()
        {
            if (pathCreator == null)
            {
                pathCreator = PreviousPath;
            }

            if (pathCreator != null)
            {
                var distanceThisFrame = pathFollower2D.speed * Time.deltaTime;

                _distanceTravelled += distanceThisFrame;

                // パスの終端に達したら次パスに乗り移る
                var t = _distanceTravelled / pathCreator.path.length;
                if (t > 1)
                {
                    _distanceTravelled -= pathCreator.path.length;

                    // 次のパスに切り替える
                    pathCreator = PreviousPath;
                }

                transform.position = pathCreator.path.GetPointAtDistance(_distanceTravelled, endOfPathInstruction);
                // パスの法線は手前向きなので、オブジェクトのｚ軸が法線の逆方向を向くようにする
                var nml = pathCreator.path.GetNormalAtDistance(_distanceTravelled, endOfPathInstruction);
                transform.rotation = Quaternion.LookRotation(-nml);
            }
        }
    }
}