using UnityEngine;

using PathCreation;

namespace u1w202312
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower2D : MonoBehaviour
    {
        public RailroadSwitch railoadSwitch;

        private EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Stop;
        private float offset = 0f;

        [HideInInspector]
        public float speed = 5;

        [HideInInspector]
        public PathCreator pathCreator;

        [HideInInspector]
        public System.Action<PathCreator> onEnterPath;

        private float distanceTravelled;

        private float _totalDistance;

        void Start()
        {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;

                distanceTravelled = offset;

                _totalDistance = 0;
            }
        }

        void Update()
        {
            if (pathCreator != null)
            {
                var distanceThisFrame = speed * Time.deltaTime;

                AddDistanceToTotal(distanceThisFrame);

                distanceTravelled += distanceThisFrame;

                // パスの終端に達したら次パスに乗り移る
                var t = distanceTravelled / pathCreator.path.length;
                if (t > 1)
                {
                    distanceTravelled -= pathCreator.path.length;

                    // 次のパスに切り替える
                    pathCreator = railoadSwitch.GetNextPath(pathCreator);
                    onEnterPath(pathCreator);
                }

                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                // パスの法線は手前向きなので、オブジェクトのｚ軸が法線の逆方向を向くようにする
                var nml = pathCreator.path.GetNormalAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = Quaternion.LookRotation(-nml);
            }
        }

        void AddDistanceToTotal(float distance)
        {
            _totalDistance += distance;

            Unity1Week.BroadcastExecuteEvents.Execute<IRailroadGameControllerRequests>(null,
                (handler, eventData) => handler.OnUpdateDistanceTravelled(_totalDistance));
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged()
        {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}