using Takato;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    [Header("シールドのHP")]
    [SerializeField] private int shieldHP;

    [Header("シールドが大きくなるまでにかかる時間")]
    [SerializeField] private float duration;

    [Header("どこまで大きくするか")]
    [SerializeField] private Vector3 maxSize;

    private float elapsed = 0.0f;
    private bool isScaleUp = true;
    private float invincibilityTime = 0.5f;
    private float invincibilityTimer = 0;

    private Enemy_Wizard parentEnemyWizard;
    public Enemy_Wizard ParentEnemyWizard { set { parentEnemyWizard = value; } }

    private void Update()
    {
        if (isScaleUp)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            transform.localScale = Vector3.Lerp(Vector3.zero, maxSize, t);

            if (t >= 1.0f)
            {
                transform.localScale = maxSize;
                isScaleUp = false;
            }
        }

        if (invincibilityTimer > 0.0f)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (invincibilityTimer > 0.0f) { return; }
        if (other.TryGetComponent<Weapon>(out var weapon))
        {
            shieldHP -= (int)weapon.AttackDamage;
            invincibilityTimer = invincibilityTime;

            if (shieldHP <= 0)
            {
                parentEnemyWizard.IsShield = false;
                Destroy(gameObject);
            }
        }
    }
}
