using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public class GameController : MonoBehaviour
    {
        private int _score;

        void Start()
        {
            _score = 0;
        }

        public void AddScore(int scoreToAdd)
        {
            _score += scoreToAdd;
        }

        public void DamageToPlayer(int damageAmount)
        {
            // game over

        }
    }
}
