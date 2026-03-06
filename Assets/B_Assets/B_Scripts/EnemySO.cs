using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/EnemySO")]
public class EnemySO : ScriptableObject
{
    [Header("“G‘S‘Ì‚ª‚Â’l‚Ì‰Šúİ’èB\n" +
        "“G‚ª‚»‚ê‚¼‚ê‚Â’l‚Í“G‚Ìinspector‚Åİ’è")]

    [Header("“G‚ÌHP")]
    public int maxHP;

    [Header("“G‚ÌUŒ‚—Í")]
    public float damage;

    [Header("“G‚Ì“®‚­ƒXƒs[ƒh")]
    public float moveSpeed;

    [Header("“G‚ª‚±‚Ì’lˆÈã‚Í‹ß‚Ã‚©‚È‚¢")]
    public float stoopingDis; 

    [Header("“G‚ÌU‚èŒü‚«‚Ì‘¬“x")]
    public float lookRotationSpeed;

    [Header("“G‚Ì’T’m”ÍˆÍ")]
    public float searchDistance;

    [Header("“G‚Ì’ÇÕ”ÍˆÍ")]
    public float contactDistance;
}
