using UnityEngine;

/// <summary>
/// プレイヤーを追跡するカメラの挙動を管理するクラス
/// </summary>
public class Player_ChaseCamera : MonoBehaviour
{
    [Header("追跡対象")]
    [SerializeField] private Transform target; // 追跡対象のTransform
    [SerializeField] private Player_Input_New playerInput; // 入力クラスの参照
    [SerializeField] private Player_Script_New playerScript;// プレイヤーのスクリプト参照

    [Header("カメラ設定")]
    [SerializeField] private Vector3 offset; // カメラの位置オフセット
    [SerializeField] private float rotationSpeed; // カメラの回転感度

    [Header("角度制限設定")]
    [SerializeField] private float minPitch; // 下向きの上限
    [SerializeField] private float maxPitch; // 上向きの上限

    private float currentYaw = 0f;   // Y軸周りの回転（左右）
    private float currentPitch = 0f; // X軸周りの回転（上下）

    private void Start()
    {
        // マウスカーソルを画面中央にロックして非表示にする
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 初期角度をカメラの現在向きから取得
        Vector3 angles = transform.eulerAngles;
        currentYaw = angles.y;
        currentPitch = angles.x;

        // 追跡対象のTransformが設定されていない場合、プレイヤーオブジェクトを探して設定
        if (playerScript == null && target != null)
        {
            playerScript = target.GetComponent<Player_Script_New>();
        }
    }

    private void LateUpdate()
    {
        if (target == null || playerInput == null) return;

        //ポーズ中、またはプレイヤー死亡中はカメラ回転操作を行わない
        bool isPaused = PausManeger.ToPause;
        bool isDead = (playerScript != null && playerScript.IsDead);

        if (!isPaused && !isDead)
        {
            // マウス入力を取得
            Vector2 lookInput = playerInput.LookInput;

            // 回転値を更新
            currentYaw += lookInput.x * rotationSpeed * 0.1f;
            currentPitch -= lookInput.y * rotationSpeed * 0.1f;

            // 上下の首振り角度を制限
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        }

        // カメラの位置・向きの追跡更新処理
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 targetPosition = target.position + rotation * offset;

        transform.position = targetPosition;
        transform.LookAt(target.position + Vector3.up * offset.y);
    }
}