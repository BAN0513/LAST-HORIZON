using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.GraphicsBuffer;

public class TornadoController : MonoBehaviour
{
    // ’†S“_
    [SerializeField] private Vector3 _center;

    // ‰ñ“]²
    [SerializeField] private Vector3 _axis = Vector3.forward;

    // ‰~‰^“®üŠú
    [SerializeField] private float _period = 2;

    private ParticleSystem particle;
    private GameObject target;
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
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        Debug.Log("—³Šª‚ª“–‚½‚Á‚½");
    //    }
    //}

    private void OnParticleTrigger()
    {
        Debug.Log("—³Šª‚ª“–‚½‚Á‚½");
    }
}
