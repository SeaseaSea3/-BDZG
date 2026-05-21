using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [Header("拼图块")]
    public Piece[] pieces;

    [Header("目标位置")]
    public Transform[] positions;

    [Header("胜利UI（可选）")]
    public GameObject winPanel;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Shuffle();
    }
    void OnEnable()
    {
        Shuffle();
    }

    // 打乱拼图
    void Shuffle()
    {
        // 生成随机位置索引
        int[] indexArr = new int[] { 0, 1, 2, 3 };

        // Fisher-Yates 洗牌
        for (int i = 0; i < indexArr.Length; i++)
        {
            int rand = Random.Range(i, indexArr.Length);
            int temp = indexArr[i];
            indexArr[i] = indexArr[rand];
            indexArr[rand] = temp;
        }

        // 分配位置 + 随机旋转
        for (int i = 0; i < pieces.Length; i++)
        {
            int posIndex = indexArr[i];

            pieces[i].SetPosition(posIndex, positions[posIndex].position);

            int rot = Random.Range(0, 4) * 90;
            pieces[i].SetRotation(rot);
        }

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    // 检查胜利
    public void CheckWin()
    {
<<<<<<< Updated upstream
        foreach (Piece p in pieces)
        {
            if (!p.IsCorrect())
                return;
=======
        if (gameOver)
        {
            return;
        }

        if (AllCorrect())
        {
            gameOver = true;

            Debug.Log("游戏结束：所有拼图都旋转回原来的角度！");

            for (int i = 0; i < pieces.Length; i++)
            {
                pieces[i].SetSelected(false);
            }

            MiniGameFinish finish = FindObjectOfType<MiniGameFinish>();

            if (finish != null)
            {
                finish.FinishMiniGame();
            }
            else
            {
                Debug.LogError("场景中没有 MiniGameFinish，无法完成小游戏跳转！");
            }
>>>>>>> Stashed changes
        }

        Win();
    }

    void Win()
    {
        Debug.Log("拼图完成！");

        if (winPanel != null)
            winPanel.SetActive(true);

        // 延迟退出（更自然）
        Invoke("ExitGame", 1.5f);
    }

    void ExitGame()
    {
        UIManager.Instance.ExitPuzzle();
    }
}