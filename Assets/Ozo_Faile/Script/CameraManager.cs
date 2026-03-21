using UnityEngine;


/// <summary>
/// 確認用プレイヤー（仮）がワープした先を追跡する用
/// </summary>
namespace Ozo
{
    public class CameraManager : MonoBehaviour
    {
        [Header("プレイヤー追跡")]
        public bool Camera = false;

        void Update()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null && Camera)
            {
                float X = player.transform.position.x;
                float Y = player.transform.position.y;
                float Z = player.transform.position.z;

                Vector3 v3 = new Vector3(X, Y + 1.5f, Z - 5f);
                transform.position = v3;
            }
            if (!Camera)
            {
                transform.position = new Vector3 (0, 1, -10);
            }
        }
    }
}
