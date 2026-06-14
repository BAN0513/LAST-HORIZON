using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "EnemySO/EnemySO")]
public class EnemySO : ScriptableObject
{
    [Header("“G‘S‘Ì‚ª‚Â’l‚Ì‰Šúİ’èB\n" +
        "“G‚ª‚»‚ê‚¼‚ê‚Â’l‚Í“G‚Ìinspector‚Åİ’è")]

    [Header("“G‚ÌUŒ‚‚Ìs“®")]
    public EnemyActionSO[] action;

    [Header("“G‚ÌUŒ‚‚µ‚È‚©‚Á‚½‚Æ‚«‚Ìs“®")]
    public EnemyActionSO[] doNotAttack_Action;

    [Header("“G‚ÌHP")]
    public int maxHP;

    [Header("“G‚ÌUŒ‚—Í")]
    public int damage;

    [Header("“G‚Ì‘–‚éƒXƒs[ƒh")]
    public float dashMoveSpeed;

    [Header("“G‚Ì•à‚­ƒXƒs[ƒh")]
    public float walkMoveSpeed;

    [Header("“G‚Ì–hŒä—Í")]
    public int def;

    [Header("“G‚ª‚±‚Ì’lˆÈã‚Í‹ß‚Ã‚©‚È‚¢")]
    public float stoopingDis; 

    [Header("“G‚ÌU‚èŒü‚«‚Ì‘¬“x")]
    public float lookRotationSpeed;

    [Header("Ú“G‹——£i‚±‚Ì’lˆÈ‰º‚É‚È‚é‚ÆUŒ‚‚Ì’Š‘I‚ğŠJn‚·‚éj")]
    public float engageDis;

    [Header("UŒ‚‚ÌŠm—¦")]
    public float attackInitProbability;

    [Header("’Š‘I‚ÅUŒ‚ˆÈŠO‚É‚È‚Á‚½‚ÉUŒ‚Šm—¦‚ğã‚°‚é‚½‚ß‚Ì’l")]
    public float attackUpProbability;

    [Header("UŒ‚ŒãA‚±‚Ì’l‚Ì•ª‚Ì•b”‚ÍUŒ‚‚Ì’Š‘I‚Ís‚í‚È‚¢")]
    public float attackCoolDown;
}
