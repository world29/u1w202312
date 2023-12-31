using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity1Week
{
    //todo: プレイヤーに取得されず画面外に見切れたアイテムを削除する
    public class Item : MonoBehaviour
    {
        [SerializeField]
        private u1w202312.ItemType itemType;

        [SerializeField]
        private float score = 1f;

        [SerializeField]
        private AudioClip se;

        [SerializeField]
        private float pitch = 1;

        [SerializeField]
        private ParticleSystem particlePrefab;

        [SerializeField]
        private UnityEvent onPickup;

        private bool _scoreAdded;

        void Start()
        {
            _scoreAdded = false;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (!_scoreAdded)
                {
                    // スコアを加算する
                    BroadcastExecuteEvents.Execute<IGameControllerRequests>(null,
                        (handler, eventData) => handler.AddScore(score));

                    BroadcastExecuteEvents.Execute<IGameControllerRequests>(null,
                        (handler, eventData) => handler.OnItemPickup(transform.position, score));

                    BroadcastExecuteEvents.Execute<u1w202312.IRailroadGameControllerRequests>(null,
                        (handler, eventData) => handler.OnItemPickup(transform.position, itemType));

                    _scoreAdded = true;
                }

                if (se != null)
                {
                    SoundManager.PlaySeWithPitch(se, pitch);
                }

                if (particlePrefab != null)
                {
                    Instantiate<ParticleSystem>(particlePrefab, transform.position, Quaternion.identity);
                }

                onPickup?.Invoke();

                Destroy(gameObject);
            }
        }
    }
}
