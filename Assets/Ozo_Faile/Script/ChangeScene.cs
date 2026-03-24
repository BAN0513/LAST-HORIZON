using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

    public string SeneName;

    public void OnDecision (InputValue var)
    {
        Load();
    }

    public void Load()
    {
        SceneManager.LoadScene(SeneName);
    }
}
