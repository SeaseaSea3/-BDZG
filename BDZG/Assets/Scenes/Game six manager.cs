using UnityEngine;
using TMPro;

public class GameSixManager : MonoBehaviour
{
    [Header("牙齿与箭头")]
    public Tooth[] teeth;
    public ArrowSelector arrowSelector;

    [Header("目标点数UI(TMP)")]
    public TMP_Text targetText;

    private int currentTarget;

    // 选牙逻辑
    private bool isFirstSelect = false;
    private int firstSelectIndex = -1;

    void Start()
    {
        // 随机目标 2~6
        currentTarget = Random.Range(2, 7);

        // 初始化牙齿
        foreach (var t in teeth)
        {
            t.point = Random.Range(1, 6);
            t.isMerged = false;
            t.toothImage.color = Color.white;
            t.UpdateDisplay();
        }

        // 显示目标
        if (targetText != null)
        {
            targetText.text = "目标点数：" + currentTarget;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            arrowSelector.MoveLeft();

        if (Input.GetKeyDown(KeyCode.D))
            arrowSelector.MoveRight();

        if (Input.GetKeyDown(KeyCode.S))
        {
            SelectOrMerge();
        }
    }

    // 按S：选第一颗 → 再选相邻第二颗 → 合成
    void SelectOrMerge()
    {
        int curIdx = arrowSelector.currentIndex;

        // 第一下按S：选中第一颗
        if (!isFirstSelect)
        {
            // 已被合成的不能选
            if (teeth[curIdx].isMerged)
            {
                Debug.Log("该牙齿已无法选择");
                return;
            }

            isFirstSelect = true;
            firstSelectIndex = curIdx;
            Debug.Log("已选中第一颗牙齿：" + curIdx);
            return;
        }

        // 第二下按S：选第二颗，准备合成
        // 必须和第一颗相邻
        int diff = Mathf.Abs(curIdx - firstSelectIndex);
        if (diff != 1)
        {
            Debug.Log("必须选择相邻的牙齿！重新选第一颗");
            ResetSelect();
            return;
        }

        // 第二颗也不能是已合成的
        if (teeth[curIdx].isMerged)
        {
            Debug.Log("第二颗牙齿已无法选择");
            ResetSelect();
            return;
        }

        // 可以合成了
        DoMerge(firstSelectIndex, curIdx);

        // 清空选择状态，下一轮重新选两颗
        ResetSelect();
    }

    // 合成：两颗都变成相加后的点数
    void DoMerge(int idx1, int idx2)
    {
        Tooth t1 = teeth[idx1];
        Tooth t2 = teeth[idx2];

        int sum = t1.point + t2.point;

        t1.point = sum;
        t2.point = sum;

        t1.UpdateDisplay();
        t2.UpdateDisplay();

        Debug.Log("两颗合成成功，点数：" + sum);

        CheckWin();
    }

    // 清空选择状态
    void ResetSelect()
    {
        isFirstSelect = false;
        firstSelectIndex = -1;
    }

    // 判断胜利
    void CheckWin()
    {
        bool win = true;
        foreach (var t in teeth)
        {
            if (t.point != currentTarget)
            {
                win = false;
                break;
            }
        }

        if (win)
        {
            Debug.Log("游戏胜利！目标点数：" + currentTarget);
        }
    }
}