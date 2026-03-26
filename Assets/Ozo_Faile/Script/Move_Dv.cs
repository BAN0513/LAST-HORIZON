using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move_Dv : MonoBehaviour
{
    [SerializeField] private GameObject Player;

    [Header("移動速度")]
    public float MoveSpeed = 5f;

    private Vector3 InputMoveValue = Vector3.zero;
    private Rigidbody Rd;

    void Start()
    {
        Rd = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 PlayerPos = Player.transform.position;
        PlayerPos += (InputMoveValue * MoveSpeed * Time.deltaTime);
        Player.transform.position = PlayerPos;
    }

    public void OnWASD(InputValue val)
    {
        InputMoveValue = val.Get<Vector3>();
    }
}
