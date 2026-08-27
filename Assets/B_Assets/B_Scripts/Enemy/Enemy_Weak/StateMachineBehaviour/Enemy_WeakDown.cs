using UnityEngine;

public class Enemy_WeakDown : StateMachineBehaviour
{
    Enemy_Weak enemy;
    Enemy_WeakAnimatorController animatorController;
    [SerializeField] private float downTime = 5;
    [SerializeField] private float duration = 0.5f;
    private float elapsed = 0.0f;
    private float downTimer;
    private Vector3 startPos;

    [SerializeField] float distance = 5.0f;
    Vector3 endPos;
    bool isKnockback = true;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy_Weak>();
        animatorController = animator.GetComponent<Enemy_WeakAnimatorController>();
        enemy.IsActionAnimation = true;
        enemy.SetLookPlayerAndEnemyStop(false, true);

        downTimer = downTime;

        startPos = enemy.transform.position;
        Vector3 dir = enemy.transform.position - enemy.Target.position;
        dir.Normalize();
        enemy.transform.rotation = Quaternion.LookRotation(-dir);
        endPos = enemy.transform.position + distance * dir;

        isKnockback = true;
        elapsed = 0.0f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isKnockback)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            enemy.transform.position = Vector3.Lerp(startPos, endPos, t);

            if (t >= 1.0f)
            {
                isKnockback = false;
            }
        }
        else
        {
            downTimer -= Time.deltaTime;
            if (downTimer <= 0)
            {
                animatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Weak_Down, true);
            }
        }
    }
}
