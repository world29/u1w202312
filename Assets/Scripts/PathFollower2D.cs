using UnityEngine;
using UnityEngine.Events;

using PathCreation;

namespace u1w202312
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower2D : MonoBehaviour
    {
        [SerializeField]
        public RailroadSwitch railroadSwitch;

        private EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Stop;

        [SerializeField]
        private float offset = 0f;

        [HideInInspector]
        public float speed = 5;

        [HideInInspector]
        public PathCreator pathCreator;

        [HideInInspector]
        public UnityAction<PathCreator> onEnterPath;

        [HideInInspector]
        public Railroad CurrentRailroad { get { return pathCreator?.GetComponent<Railroad>(); } }

        private float _distanceTravelled;

        void Start()
        {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;

                _distanceTravelled = offset;

            }
        }

        void Update()
        {
            if (pathCreator != null)
            {
                var distanceThisFrame = speed * Time.deltaTime;

                NotifyDistanceTravelled(distanceThisFrame);

                _distanceTravelled += distanceThisFrame;

                // パスの終端に達したら次パスに乗り移る
                var t = _distanceTravelled / pathCreator.path.length;
                if (t > 1)
                {
                    _distanceTravelled -= pathCreator.path.length;

                    // 次のパスに切り替える
                    // 分岐がある場合は、RailroadSwitch に問い合わせる
                    if (CurrentRailroad.next2 != null)
                    {
                        pathCreator = railroadSwitch.GetNextPath(pathCreator);
                    }
                    else
                    {
                        pathCreator = CurrentRailroad.next1.path;
                    }

                    onEnterPath?.Invoke(pathCreator);
                }

                transform.position = pathCreator.path.GetPointAtDistance(_distanceTravelled, endOfPathInstruction);
                // パスの法線は手前向きなので、オブジェクトのｚ軸が法線の逆方向を向くようにする
                var nml = pathCreator.path.GetNormalAtDistance(_distanceTravelled, endOfPathInstruction);
                transform.rotation = Quaternion.LookRotation(-nml);
            }
        }

        void NotifyDistanceTravelled(float distanceThisFrame)
        {
            Unity1Week.BroadcastExecuteEvents.Execute<IRailroadGameControllerRequests>(null,
                (handler, eventData) => handler.OnTravelled(distanceThisFrame));
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged()
        {
            _distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}