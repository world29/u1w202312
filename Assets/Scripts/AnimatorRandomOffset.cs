using UnityEngine;

namespace Unity1Week
{
    public class AnimatorRandomOffset : MonoBehaviour
    {
        private Animator _animator;

        void Awake()
        {
            TryGetComponent(out _animator);
        }

        void Start()
        {
            _animator.Play(_animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, Random.Range(0f, 1f));
        }
    }
}
