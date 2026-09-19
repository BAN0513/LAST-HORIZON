using UnityEngine;

public class Enemy_WolfBoss_Jump : StateMachineBehaviour
{
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private float jumpDistance = 20.0f;
    float elapsed = 0.0f;
    Vector3 startPos;
    Vector3 endPos;
    private Enemy enemy;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();

        startPos = enemy.transform.position;
        endPos = enemy.transform.position + (enemy.Target.position - enemy.transform.position).normalized * jumpDistance;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);
        enemy.transform.position = Vector3.Lerp(startPos, endPos, t);
    }
}
