using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HookStretchController : MonoBehaviour
{
    [Header("钩子杆")]
    public RectTransform hookBody;
    public float minLength = 60f;
    public float maxLength = 320f;
    public float stretchDuration = 0.25f;

    [Header("钩子头")]
    public RectTransform hookHead;
    public float headOffsetY = 0f; // 如果头和杆子有缝，就调这个

    [Header("细胞生成器")]
    public CellSpawner cellSpawner;

    [Header("结果文字")]
    public Text resultText;

    private float currentLength;
    private Tween stretchTween;
    private bool gameOver = false;

    void Start()
    {
        CellMove.stopAll = false;

        currentLength = minLength;
        SetHookLength(minLength);
    }

    void Update()
    {
        if (gameOver) return;

        if (Input.GetKeyDown(KeyCode.S))
        {
            StretchDown();
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            StretchUp();
        }
    }

    void StretchDown()
    {
        stretchTween?.Kill();

        stretchTween = DOTween.To(
            () => currentLength,
            x =>
            {
                currentLength = x;
                SetHookLength(x);
            },
            maxLength,
            stretchDuration
        ).SetEase(Ease.OutQuad);
    }

    void StretchUp()
    {
        stretchTween?.Kill();

        stretchTween = DOTween.To(
            () => currentLength,
            x =>
            {
                currentLength = x;
                SetHookLength(x);
            },
            minLength,
            stretchDuration
        ).SetEase(Ease.InQuad);
    }

    void SetHookLength(float length)
    {
        // 杆子变长
        hookBody.sizeDelta = new Vector2(hookBody.sizeDelta.x, length);

        // 钩子头跟着杆子底部移动
        hookHead.anchoredPosition = new Vector2(
            hookHead.anchoredPosition.x,
            -length + headOffsetY
        );
    }

    public void FailGame()
    {
        if (gameOver) return;

        gameOver = true;
        StopWholeGame();

        if (resultText != null)
            resultText.text = "失败！碰到细胞";

        hookHead.DOShakeAnchorPos(0.3f, 20f, 20);
    }

    public void ClearGame()
    {
        if (gameOver) return;

        gameOver = true;
        StopWholeGame();

        if (resultText != null)
            resultText.text = "完成！取到子弹";

        hookHead.DOPunchScale(Vector3.one * 0.3f, 0.3f, 5);

    }

    void StopWholeGame()
    {
        stretchTween?.Kill();

        CellMove.stopAll = true;

        if (cellSpawner != null)
        {
            cellSpawner.StopSpawn();
        }
    }
}