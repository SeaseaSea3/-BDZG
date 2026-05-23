using UnityEngine;
using UnityEngine.UI;

public class ArrowSelector : MonoBehaviour
{
    [Header("箭头设置")]
    public int currentIndex = 0;
    public float moveSpeed = 15f; // 调大一点，移动更跟手
    private Vector2 targetAnchoredPos;
    public Tooth[] teeth;
    private RectTransform arrowRect;

    void Start()
    {
        // 缓存箭头的RectTransform（必须是UI物体）
        arrowRect = GetComponent<RectTransform>();
        if (arrowRect == null)
        {
            Debug.LogError("箭头必须是UI物体，并且在Canvas下！");
            return;
        }

        if (teeth != null && teeth.Length > 0)
            UpdateArrowPosition();
    }

    void Update()
    {
        if (teeth != null && teeth.Length > 0 && arrowRect != null)
        {
            // 用UI专用的anchoredPosition平滑移动
            arrowRect.anchoredPosition = Vector2.Lerp(
                arrowRect.anchoredPosition,
                targetAnchoredPos,
                moveSpeed * Time.deltaTime
            );
        }
    }

    public void MoveLeft()
    {
        if (currentIndex > 0)
        {
            do
            {
                currentIndex--;
            } while (currentIndex > 0 && teeth[currentIndex].isMerged);

            UpdateArrowPosition();
        }
    }

    public void MoveRight()
    {
        if (currentIndex < teeth.Length - 1)
        {
            do
            {
                currentIndex++;
            } while (currentIndex < teeth.Length - 1 && teeth[currentIndex].isMerged);

            UpdateArrowPosition();
        }
    }

    public void SetCurrentIndex(int index)
    {
        if (index >= 0 && index < teeth.Length)
        {
            currentIndex = index;
            UpdateArrowPosition();
        }
    }

    // 【核心修复】直接获取牙齿的UI坐标，100%对齐
    void UpdateArrowPosition()
    {
        if (teeth[currentIndex] == null) return;

        // ✅ 这是 100% 正确获取牙齿图片位置的方法
        Image toothImage = teeth[currentIndex].toothImage;
        RectTransform toothRect = toothImage.rectTransform;

        Vector2 toothPos = toothRect.anchoredPosition;
        targetAnchoredPos = new Vector2(toothPos.x, toothPos.y - 30f); // 上下偏移自己调
    }
}