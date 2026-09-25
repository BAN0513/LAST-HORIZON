using Takato;
using UnityEngine;

public class TornadoController : MonoBehaviour
{
    // ’†S“_
    private Vector3 _center;

    [Header("‰ñ“]²")]
    [SerializeField] private Vector3 _axis;

    // ‰~‰^“®üŠú
    private float _period = 2;

    private float distance;
    [Header("ˆø‚«Šñ‚¹‚ÉŠª‚«‚Ü‚ê‚é‹——£")]
    [SerializeField] private float attractionDis = 10;

    [Header("ˆø‚«Šñ‚¹‚é—Í")]
    [SerializeField] private float attractionPower = 1;

    [Header("‰½•bŠÔc‚è‘±‚¯‚é‚©")]
    [SerializeField] private float activeTimer = 3.0f;

    private ParticleSystem particle;
    public GameObject Target { private get; set; }

    public int Damage { private get; set; }

    private void Start()
    {
        transform.Rotate(-90, 0, 0);
        _center = transform.position;
        _center.y = 0;

        particle = GetComponent<ParticleSystem>();
        particle.trigger.SetCollider(0, Target.transform);
    }

    private void Update()
    {
        // ’†S“_center‚Ìü‚è‚ğA²axis‚ÅAperiodüŠú‚Å‰~‰^“®
        transform.RotateAround(
            _center,
            _axis,
            360 / _period * Time.deltaTime
        );

        distance = Vector3.Distance(transform.position, Target.transform.position);
        if (distance <= attractionDis)
        {
            Target.transform.position = Vector3.MoveTowards(Target.transform.position,transform.position, attractionPower * Time.deltaTime);
        }

        activeTimer -= Time.deltaTime;
        if (activeTimer <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnParticleTrigger()
    {
        Debug.Log("—³Šª‚ª“–‚½‚Á‚½");
        PlayerController player = Target.GetComponent<PlayerController>();
        player.TakeDamage(Damage);
    }
}
