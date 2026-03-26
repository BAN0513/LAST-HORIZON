using Takato;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.GraphicsBuffer;

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

    private ParticleSystem particle;
    private GameObject target;
    private int damage;
    public int Damage
    {
        set
        {
            damage = value;
        }
    }
    private void Start()
    {
        transform.Rotate(-90, 0, 0);
        _center = transform.position;
        _center.y = 0;

        particle = GetComponent<ParticleSystem>();
        target = GameObject.FindWithTag("Player");
        particle.trigger.SetCollider(0, target.transform);
    }

    private void Update()
    {
        // ’†S“_center‚Ìü‚è‚ğA²axis‚ÅAperiodüŠú‚Å‰~‰^“®
        transform.RotateAround(
            _center,
            _axis,
            360 / _period * Time.deltaTime
        );

        distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= attractionDis)
        {
            Debug.Log("ˆø‚«Šñ‚¹‚ç‚ê‚éII");
            target.transform.position = Vector3.MoveTowards(target.transform.position,transform.position, attractionPower * Time.deltaTime);
        }
    }

    private void OnParticleTrigger()
    {
        Debug.Log("—³Šª‚ª“–‚½‚Á‚½");
        PlayerController player = target.GetComponent<PlayerController>();
        player.TakeDamage(damage);
    }
}
