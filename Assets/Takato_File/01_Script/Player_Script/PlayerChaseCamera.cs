using UnityEngine;

namespace Takato
{
    /// <summary>
    /// プレイヤーを追従し、マウス入力で回転するカメラを管理するクラス
    /// </summary>
    public class PlayerChaseCamera : MonoBehaviour
    {
        [Header("カメラの設定")]
        [SerializeField] private Transform player; // プレイヤーのTransform
        [Header("カメラのオフセット（水平成分のみ使用）")]
        [SerializeField] private Vector3 cameraOffset; // プレイヤーからの水平オフセット (Yは無視)
        [Header("カメラの高さ")]
        [SerializeField] private float cameraHeight; // プレイヤーのどの高さを狙うか（LookAt高さ）
        [Header("マウスの感度")]
        [SerializeField] private float mouseSensitivity; // マウス感度
        [Header("回転の制限")]
        [SerializeField] private float minVerticalAngle; // 上下回転の最小角度
        [SerializeField] private float maxVerticalAngle; // 上下回転の最大角度
        [Header("プレイヤーの回転速度")]
        [SerializeField] private float playerRotationSpeed; // プレイヤーの回転速度
        [Header("Y軸反転")]
        [SerializeField] private bool invertY; // 上下反転のオン/オフ

        private PlayerInputController inputController; // プレイヤー入力コントローラー
        private float horizontalAngle = 0f; // 水平回転角度
        private float verticalAngle = 0f; // 垂直回転角度

        private void Start()
        {
            if (player == null)
            {
                player = transform.parent; // 親オブジェクトがプレイヤーの場合
            }

            if (player != null)
            {
                inputController = player.GetComponent<PlayerInputController>();
                if (inputController != null)
                {
                    inputController.IsLookEnabled = true; // Look入力を有効化
                }
            }
        }

        private void LateUpdate()
        {
            if (player == null || inputController == null)
            {
                return;
            }

            // マウス入力（LookInput）を取得
            Vector2 lookInput = inputController.LookInput;

            // Y軸の反転設定を考慮して垂直入力を決定
            float lookY = invertY ? -lookInput.y : lookInput.y;

            // カメラの回転を更新（上下の符号は invertY で制御）
            horizontalAngle += lookInput.x * mouseSensitivity;
            verticalAngle += lookY * mouseSensitivity;
            verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);

            // 水平オフセットだけを使う（cameraOffset.y は無視）
            Vector3 horizontalOffset = new Vector3(cameraOffset.x, 0f, cameraOffset.z);

            // カメラの位置と回転を計算
            Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
            Vector3 newCameraPosition = player.position + Vector3.up * cameraHeight + rotation * horizontalOffset;

            // カメラの位置と回転を適用
            transform.position = newCameraPosition;
            transform.LookAt(player.position + Vector3.up * cameraHeight);

            // プレイヤーを常にカメラの方向に向ける
            RotatePlayerTowardCamera();
        }

        /// <summary>
        /// プレイヤーを常にカメラの方向に向けるメソッド
        /// </summary>
        private void RotatePlayerTowardCamera()
        {
            if (player == null)
            {
                return;
            }

            // カメラの前方向（Y軸を無視）をプレイヤーの向きとする
            Vector3 cameraForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

            if (cameraForward.sqrMagnitude > 0.0001f)
            {
                // カメラが指す方向にプレイヤーを滑らかに回転させる
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
                player.rotation = Quaternion.Slerp(player.rotation, targetRotation, playerRotationSpeed * Time.deltaTime);
            }
        }

        // --- Getter / Setter ---
        public Vector3 GetCameraOffset() => cameraOffset;
        public void SetCameraOffset(Vector3 offset) => cameraOffset = offset;

        public float GetCameraHeight() => cameraHeight;
        public void SetCameraHeight(float height) => cameraHeight = height;

        public void SetMouseSensitivity(float sensitivity) => mouseSensitivity = Mathf.Max(0f, sensitivity);
        public void SetPlayerRotationSpeed(float speed) => playerRotationSpeed = Mathf.Max(0f, speed);
        public void SetInvertY(bool invert) => invertY = invert;
    }
}

