using UnityEngine;

public class ImpactController : MonoBehaviour
{
    ParticleSystem particle;
    GameObject target;

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        target = GameObject.FindWithTag("Player");
        
        particle.trigger.SetCollider(0, target.transform);
    }

    private void OnParticleTrigger()
    {
        Debug.Log("è’åÇîgÇ…ìñÇΩÇ¡ÇΩ");
    }
}
