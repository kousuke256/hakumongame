using UnityEngine;

public class AutoDestroyAnimation : MonoBehaviour

{

    [SerializeField] private float extraDelay = 0.05f;

    private void Start()

    {

        Animator animator = GetComponent<Animator>();

        if (animator == null || animator.runtimeAnimatorController == null)

        {

            Destroy(gameObject);

            return;

        }

        // 最初のクリップの長さを取得して削除予約

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        float duration = clips.Length > 0 ? clips[0].length : 0.5f;

        Destroy(gameObject, duration + extraDelay);

    }

}
