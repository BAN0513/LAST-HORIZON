using UnityEngine;

public class SystemManager : MonoBehaviour
{
    public static SystemManager instance { get; private set; }

    public float volueSE;
    public float volueBGM;
    public float valueLight;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

}
