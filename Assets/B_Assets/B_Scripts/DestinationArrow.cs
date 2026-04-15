using Takato;
using UnityEngine;

public class DestinationArrow : MonoBehaviour
{
    private PlayerController player;
    public GameObject destinetionObj;
    [SerializeField] private GameObject testDestinetion;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (destinetionObj == null) return;

        Vector3 dir = destinetionObj.transform.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
        Debug.Log(player.transform.position);
    }

    public void SetDestinetion(GameObject destinetion)
    {
        gameObject.SetActive(true);
        destinetionObj = destinetion;
    }

    public void DestroyDestinetion()
    {
        gameObject.SetActive(false);
        destinetionObj = null;
    }

    [ContextMenu("ワープポイントセット")]
    public void Set()
    {
        SetDestinetion(testDestinetion);
    }
}
