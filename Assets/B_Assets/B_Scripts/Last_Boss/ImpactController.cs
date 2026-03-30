using Takato;
using UnityEngine;

public class ImpactController : MonoBehaviour
{
    ParticleSystem particle;
    GameObject target;
    public int damage;

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        target = GameObject.FindWithTag("Player");
        
        if (target == null) { return; }
        particle.trigger.SetCollider(0, target.transform);
    }

    private void OnParticleTrigger()
    {
        Debug.Log("è’åÇîgÇ…ìñÇΩÇ¡ÇΩ");
        PlayerController player = target.GetComponent<PlayerController>();
        player.TakeDamage(damage);
    }
}
