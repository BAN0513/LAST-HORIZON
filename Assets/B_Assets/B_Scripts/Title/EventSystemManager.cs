using UnityEngine;

public class EventSystemManager : MonoBehaviour
{
    public static EventSystemManager Instance;
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


}
