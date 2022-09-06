using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    //todo: プレイヤーに取得されず画面外に見切れたアイテムを削除する
    public class Item : MonoBehaviour
    {
        [SerializeField]
        private float score = 1f;

        private bool _scoreAdded;

        void Start()
        {
            _scoreAdded = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (!_scoreAdded)
                {
                    // スコアを加算する
                    BroadcastExecuteEvents.Execute<IGameControllerRequests>(null,
                        (handler, eventData) => handler.AddScore(score));

                    _scoreAdded = true;
                }

                Destroy(gameObject);
            }
        }
    }
}
