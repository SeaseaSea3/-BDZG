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
        foreach (Piece p in pieces)
        {
            if (!p.IsCorrect())
                return;
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