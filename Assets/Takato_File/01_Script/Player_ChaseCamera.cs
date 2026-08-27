using UnityEngine;

/// <summary>
/// プレイヤーを追跡するカメラの挙動を管理するクラス
/// </summary>
public class Player_ChaseCamera : MonoBehaviour
{
    [Header("追跡対象")]
    [SerializeField] private Transform target; // 追跡対象のTransform
    [SerializeField] private Player_Input_New playerInput; // 入力クラスの参照

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
    }

    private void LateUpdate()
    {
        if (target == null || playerInput == null) return; // 追跡対象または入力クラスが設定されていない場合は処理を中断

        // マウス入力を取得
        Vector2 lookInput = playerInput.LookInput;

        //Time.deltaTime は掛けずに感度のみで計算
        currentYaw += lookInput.x * rotationSpeed * 0.1f;
        currentPitch -= lookInput.y * rotationSpeed * 0.1f;

        // 上下の首振り角度を制限
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

        // クォータニオン回転を作成
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

        // ターゲット位置を中心にオフセット分移動させた位置を計算
        Vector3 targetPosition = target.position + rotation * offset;

        // カメラの位置と向きを適用
        transform.position = targetPosition;
        transform.LookAt(target.position + Vector3.up * offset.y);
    }
}