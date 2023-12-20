using UnityEngine;

using PathCreation;

using System.Collections;
using System.Collections.Generic;

namespace u1w202312
{
    public class Railroad : MonoBehaviour
    {
        // 現在のパス
        [SerializeField] public PathCreator path;
        // 線路の位置
        [SerializeField] public float offsetZ = 2f;
        // 次のパス候補１
        [SerializeField] public Railroad next1;
        // 次のパス候補２
        [SerializeField] public Railroad next2;
    }
}