using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    // PlatformBehaviour は Unity の実行順序設定で DefaultTime より後に設定する。
    // プレイヤーや敵など動くアクターが DefaultTime で OnPassengerStay を呼び出した後に、Update, LateUpdate を処理する。
    public abstract class PlatformBehaviour : MonoBehaviour, IPlatform
    {
        private HashSet<Transform> _passengers;
        private HashSet<Transform> _passengersPrevFrame;
        private Dictionary<Transform, Vector2> _passengerVelocityMap;

        protected abstract void OnPassengerEnter(Transform passenger, Vector2 velocity);
        protected abstract void OnPassengerExit(Transform passenger);
        protected abstract void OnPassengerStay(Transform passenger);

        // IPlatform
        public void OnLandingPlatform(Transform passenger, Vector2 velocity)
        {
            _passengers.Add(passenger);
            _passengerVelocityMap[passenger] = velocity;
        }

        protected virtual void Awake()
        {
            _passengers = new HashSet<Transform>();
            _passengersPrevFrame = new HashSet<Transform>();
            _passengerVelocityMap = new Dictionary<Transform, Vector2>();
        }

        protected virtual void Update()
        {
            // 前フレームでは乗っていたが今フレームでは乗っていない (降りた)オブジェクトに対する処理
            foreach (var passenger in _passengersPrevFrame)
            {
                if (!_passengers.Contains(passenger))
                {
                    OnPassengerExit(passenger);
                }
            }

            // 上に乗っているものに対して処理を行う
            foreach (var passenger in _passengers)
            {
                if (!_passengersPrevFrame.Contains(passenger))
                {
                    OnPassengerEnter(passenger, _passengerVelocityMap[passenger]);
                }
                else
                {
                    OnPassengerStay(passenger);
                }
            }
        }

        protected virtual void LateUpdate()
        {
            // 上に乗っているオブジェクトリストをクリアする
            _passengersPrevFrame = new HashSet<Transform>(_passengers);
            _passengers.Clear();
        }
    }
}
