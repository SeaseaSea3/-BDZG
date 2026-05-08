using UnityEngine;

public class Piece : MonoBehaviour
{
    [Header("正确数据")]
    public int correctIndex;   // 正确位置（0~3）

    [Header("当前状态")]
    public int currentIndex;   // 当前所在位置
    public int currentRotation; // 当前旋转角度（0,90,180,270）

    private void OnMouseDown()
    {
        RotatePiece();
    }

    // 点击旋转
    void RotatePiece()
    {
        currentRotation += 90;
        if (currentRotation >= 360)
            currentRotation = 0;

        transform.Rotate(0, 0, -90);

        PuzzleManager.Instance.CheckWin();
    }

    // 判断是否正确
    public bool IsCorrect()
    {
        return currentIndex == correctIndex && currentRotation == 0;
    }

    // 设置位置
    public void SetPosition(int index, Vector3 pos)
    {
        currentIndex = index;
        transform.position = pos;
    }

    // 设置旋转
    public void SetRotation(int angle)
    {
        currentRotation = angle;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}