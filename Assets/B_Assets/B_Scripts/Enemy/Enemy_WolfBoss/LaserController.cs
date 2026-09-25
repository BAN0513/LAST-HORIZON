using Takato;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public GameObject Target { private get; set; }
    public int Damage {  private get; set; }

    private ParticleSystem particle;

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        particle.trigger.SetCollider(0, Target.transform);
    }

    private void OnParticleTrigger()
    {
        Debug.Log("ƒŒ[ƒU[‚ª“–‚½‚Á‚½");
        PlayerController player = Target.GetComponent<PlayerController>();
        player.TakeDamage(Damage);
    }
}
