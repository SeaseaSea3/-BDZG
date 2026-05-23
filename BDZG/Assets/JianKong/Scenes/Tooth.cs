using UnityEngine;
using UnityEngine.UI;

public class Tooth : MonoBehaviour
{
    [Header("牙齿设置")]
    public int point = 1; // 当前点数
    public bool isMerged = false; // 是否已被合成（无法选中）
    public Text pointText; // 显示点数的文本（可选）
    public Sprite[] pointSprites; // 对应点数1~6的Sprite
    public Image toothImage; // 牙齿的Image组件

    // 更新牙齿显示
    public void UpdateDisplay()
    {
        if (pointText != null)
            pointText.text = point.ToString();

        // 合成后点数可能超过5，这里做个限制，防止越界
        if (toothImage != null && point >= 1 && point <= 5)
            toothImage.sprite = pointSprites[point - 1];
        else if (toothImage != null && point > 5)
        {
            // 如果合成后超过5点，显示第5张图，或者你可以再加更高点数的图
            toothImage.sprite = pointSprites[4];
        }
    }

    // 初始化
    void Start()
    {
        UpdateDisplay();
    }
}