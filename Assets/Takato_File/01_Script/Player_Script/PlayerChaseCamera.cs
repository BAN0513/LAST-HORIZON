using UnityEngine;

namespace Takato
{
    /// <summary>
    /// プレイヤーを追従し、マウス入力で回転するカメラを管理するクラス
    /// </summary>
    public class PlayerChaseCamera : MonoBehaviour
    {
        [System.Serializable]
        public struct CameraSettings
        {
            [Tooltip("マウスの回転感度")] public float mouseSensitivity;
            [Tooltip("上下回転の最小角度")] public float minVerticalAngle;
            [Tooltip("上下回転の最大角度")] public float maxVerticalAngle;
            [Tooltip("プレイヤーの回転追従速度")] public float playerRotationSpeed;
            [Tooltip("マウスY軸（上下）の反転")] public bool invertY;
            [Tooltip("角度のスムース時間（SmoothDampAngle 用）")] public float angleSmoothTime;
        }

        [Header("--- 追従対象の設定 ---")]
        [SerializeField] private Transform player;
        [SerializeField] private Vector3 cameraOffset;
        [SerializeField] private float cameraHeight;

        [Header("--- カメラの挙動設定 ---")]
        [SerializeField] private CameraSettings settings;

        // 自動検索オプション
        [Header("--- 自動プレイヤー検索（オプション） ---")]
        [SerializeField] private bool autoFindPlayerByTag = true;
        [SerializeField] private float autoFindInterval;


        private float autoFindCooldown = 0f; // 自動検索のクールダウンタイマー

        // キャッシュ
        private Transform camTransform;
        private PlayerInputController inputController;

        // 角度およびスムース用の速度
        private float targetHorizontalAngle;
        private float targetVerticalAngle;
        private float currentHorizontalAngle;
        private float currentVerticalAngle;
        private float horizontalVelocity;
        private float verticalVelocity;

        private void Awake()
        {
            camTransform = transform;
        }

        private void Start()
        {
            // 最初のプレイヤー参照と入力取得
            InitializeForPlayer(player);

            // 初期角度をカメラの現状から設定（ジャンプを防ぐ）
            var euler = camTransform.eulerAngles;
            currentHorizontalAngle = targetHorizontalAngle = euler.y;
            float vx = euler.x;
            if (vx > 180f) vx -= 360f;
            currentVerticalAngle = targetVerticalAngle = Mathf.Clamp(vx, settings.minVerticalAngle, settings.maxVerticalAngle);
        }

        private void LateUpdate()
        {
            // player が null のときのみ、間隔を置いてタグ検索（毎フレーム検索はしない）
            if (player == null && autoFindPlayerByTag)
            {
                autoFindCooldown -= Time.deltaTime;
                if (autoFindCooldown <= 0f)
                {
                    var go = GameObject.FindWithTag("Player");
                    if (go != null) InitializeForPlayer(go.transform);
                    autoFindCooldown = Mathf.Max(0.01f, autoFindInterval);
                }
            }

            if (player == null) return;

            //入力取得
            Vector2 look = Vector2.zero;
            if (inputController != null)
            {
                look = inputController.LookInput;
            }
            else
            {
                look.x = Input.GetAxis("Mouse X");
                look.y = Input.GetAxis("Mouse Y");
            }

            //目標角度を更新（behavior の互換を保つため Time.deltaTime を感度調整に導入）
            float sens = settings.mouseSensitivity;
            targetHorizontalAngle += look.x * sens;
            float invert = settings.invertY ? 1f : -1f;
            targetVerticalAngle += look.y * sens * invert;
            targetVerticalAngle = Mathf.Clamp(targetVerticalAngle, settings.minVerticalAngle, settings.maxVerticalAngle);

            //スムースに現在角度へ近づける
            float smooth = Mathf.Max(0.0001f, settings.angleSmoothTime);
            currentHorizontalAngle = Mathf.SmoothDampAngle(currentHorizontalAngle, targetHorizontalAngle, ref horizontalVelocity, smooth);
            currentVerticalAngle = Mathf.SmoothDampAngle(currentVerticalAngle, targetVerticalAngle, ref verticalVelocity, smooth);

            //カメラ位置・回転の適用
            Quaternion cameraRotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0f);
            Vector3 targetLookAt = player.position + Vector3.up * cameraHeight;
            Vector3 targetPosition = targetLookAt + cameraRotation * cameraOffset;

            camTransform.position = targetPosition;
            camTransform.rotation = cameraRotation;

            //プレイヤー回転（平面のみ）: 角度差がある場合だけ計算する
            RotatePlayerTowardCamera(targetLookAt);
        }

        private void InitializeForPlayer(Transform targetPlayer)
        {
            player = targetPlayer ?? transform.parent;
            if (player != null)
            {
                // TryGetComponent を使って余計な GC/lookup を避ける
                player.TryGetComponent(out inputController);
            }
        }

        private void RotatePlayerTowardCamera(Vector3 lookAtTarget)
        {
            if (player == null) return;

            // カメラの正面ベクトルを水平面に投影
            Vector3 camForward = (lookAtTarget - camTransform.position).normalized;
            Vector3 planarForward = Vector3.ProjectOnPlane(camForward, Vector3.up);
            if (planarForward.sqrMagnitude <= Mathf.Epsilon) return;

            Quaternion targetRot = Quaternion.LookRotation(planarForward.normalized);
            float t = Mathf.Clamp01(settings.playerRotationSpeed * Time.deltaTime);
            player.rotation = Quaternion.Slerp(player.rotation, targetRot, t);
        }

        // --- プロパティ / セッター ---
        public Vector3 CameraOffset { get => cameraOffset; set => cameraOffset = value; }
        public float CameraHeight { get => cameraHeight; set => cameraHeight = value; }
        public CameraSettings Settings { get => settings; set => settings = value; }
    }
}

