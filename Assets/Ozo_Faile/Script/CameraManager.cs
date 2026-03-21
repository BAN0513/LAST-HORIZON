using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// 確認用プレイヤー（仮）がワープした先を追跡する用
/// </summary>
namespace Ozo
{
    public class CameraManager : MonoBehaviour
    {
        [Header("プレイヤー追跡")]
        public bool Camera = false;

        [Header("追跡カメラ座標")]
        public float x = 0;
        public float y = 0;
        public float z = 0;

        void Update()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null && Camera)
            {
                float X = player.transform.position.x;
                float Y = player.transform.position.y;
                float Z = player.transform.position.z;

                Vector3 v3 = new Vector3(X + x, Y + y, Z + z);
                transform.position = v3;
            }
            if (!Camera)
            {
                transform.position = new Vector3 (0, 1, -10);
            }
        }
    }
}
