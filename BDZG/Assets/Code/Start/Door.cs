using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [Header("要进入的场景名")]
    public string targetSceneName = "Prepare";

    public void EnterDoor()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}