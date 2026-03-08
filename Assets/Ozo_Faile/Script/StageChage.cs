using UnityEngine;
using UnityEngine.InputSystem;

///<summary>
/// ステージ移動の管理するクラス
/// (現在は仮でPキーを入力するとワープする)
/// </summary>
namespace Ozo
{
    public class StageChage : MonoBehaviour
    {
        [Header("ステージチェンジ")]
        public bool Chage = false;//ステージチェンジ中かどうか

        [Header("ステージナンバー（0～3）")]
        public int StageNumber = 0;//現在のステージナンバー

        private void Start()
        {
            if (StageNumber < 0 || StageNumber > 3)
            { 
                StageNumber = 0;
                transform.position = Vector3.zero;
            }
        }

        private void Update()
        {
            if(Chage)
            {
                StageNumber++;
                if (StageNumber > 3) StageNumber = 0;
                Warp();
                Chage = false;
            }
        }

        /// <summary>
        /// ステージチェンジ時のワープ関数
        /// </summary>
        private void Warp()
        {
            if (StageNumber == 1)
                transform.position = new Vector3(0, 1, 15);
            if (StageNumber == 2)
                transform.position = new Vector3(-15, 1, 15);
            if (StageNumber == 3)
                transform.position = new Vector3(20, 1, 15);
            if (StageNumber == 0)
                transform.position = new Vector3(0, 1, 0);
        }

        /// <summary>
        /// ステージチェンジ時のワープのデバッグ用
        /// </summary>
        /// <param name="var"></param>
        private void OnDv_Warp(InputValue var)
        {
            Chage = true;
        }
    }
}
