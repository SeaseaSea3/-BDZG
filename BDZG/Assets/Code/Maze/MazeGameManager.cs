using UnityEngine;
using UnityEngine.SceneManagement;

public class MazeGameManager : MonoBehaviour
{
    public static MazeGameManager Instance;

    [Header("主界面场景名")]
    public string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        Instance = this;
    }

    public void GameWin()
    {
        Debug.Log("迷宫小游戏完成，返回主界面");
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void RestartGame()
    {
        Debug.Log("撞到墙，重新开始迷宫");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}