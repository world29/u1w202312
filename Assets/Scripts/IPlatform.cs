using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public interface IPlatform
    {
        /// <summary>
        /// 上に載っているものを通知する。フレーム毎にリセットされるため、載っているフレームで毎回呼ぶ必要がある
        /// </summary>
        /// <param name="passenger"></param>
        /// <param name="velocity">接地したときの速度</param>
        void OnLandingPlatform(Transform passenger, Vector2 velocity);
    }
}
