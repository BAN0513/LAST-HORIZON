using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

    public string SeneName;

    private void OnDecision (InputValue var)
    {
        Load();
    }

    private void Load()
    {
        SceneManager.LoadScene(SeneName);
    }
}
