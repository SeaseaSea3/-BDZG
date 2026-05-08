using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("界面")]
    public GameObject startPanel;
    public GameObject puzzlePanel;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ShowStart();
    }

    // 显示开始界面
    public void ShowStart()
    {
        startPanel.SetActive(true);
        puzzlePanel.SetActive(false);
    }

    // 进入拼图
    public void StartPuzzle()
    {
        startPanel.SetActive(false);
        puzzlePanel.SetActive(true);

    }

    // 拼图完成退出
    public void ExitPuzzle()
    {
        ShowStart();
    }
}