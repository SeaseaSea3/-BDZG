using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CleanStainGame : MonoBehaviour
{
    [Header("棉球")]
    public RectTransform cottonBall;
    public float moveSpeed = 300f;
    public float cleanRadius = 45f;

    [Header("污渍")]
    public Image stainImage;

    [Header("UI")]
    public Text resultText;

    private Texture2D stainTexture;
    private Color32[] pixels;

    private bool gameFinished = false;
    private bool cottonTweening = false;

    void Start()
    {
        InitGame();
    }

    void Update()
    {
        if (gameFinished) return;

        MoveCottonBall();
        CleanAtCottonBall();
        CheckFinish();
    }

    void InitGame()
    {
        gameFinished = false;

        if (resultText != null)
            resultText.text = "";

        Texture2D source = stainImage.sprite.texture;

        stainTexture = new Texture2D(
            source.width,
            source.height,
            TextureFormat.RGBA32,
            false
        );

        stainTexture.SetPixels32(source.GetPixels32());
        stainTexture.Apply();

        stainImage.sprite = Sprite.Create(
            stainTexture,
            new Rect(0, 0, stainTexture.width, stainTexture.height),
            new Vector2(0.5f, 0.5f)
        );

        pixels = stainTexture.GetPixels32();
    }

    void MoveCottonBall()
    {
        Vector2 input = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) input.y += 1;
        if (Input.GetKey(KeyCode.S)) input.y -= 1;
        if (Input.GetKey(KeyCode.A)) input.x -= 1;
        if (Input.GetKey(KeyCode.D)) input.x += 1;

        if (input == Vector2.zero) return;

        Vector2 targetPos = cottonBall.anchoredPosition +
                            input.normalized * moveSpeed * Time.deltaTime;

        cottonBall.DOAnchorPos(targetPos, 0.05f).SetEase(Ease.Linear);

        if (!cottonTweening)
        {
            cottonTweening = true;

            cottonBall.DOScale(1.08f, 0.08f).OnComplete(() =>
            {
                cottonBall.DOScale(1f, 0.08f).OnComplete(() =>
                {
                    cottonTweening = false;
                });
            });
        }
    }

    void CleanAtCottonBall()
    {
        RectTransform stainRect = stainImage.rectTransform;

        Vector2 localPoint;

        bool inside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            stainRect,
            RectTransformUtility.WorldToScreenPoint(null, cottonBall.position),
            null,
            out localPoint
        );

        if (!inside) return;

        Rect rect = stainRect.rect;

        float normalizedX = (localPoint.x - rect.x) / rect.width;
        float normalizedY = (localPoint.y - rect.y) / rect.height;

        int centerX = Mathf.RoundToInt(normalizedX * stainTexture.width);
        int centerY = Mathf.RoundToInt(normalizedY * stainTexture.height);

        float radiusInPixel = cleanRadius * (stainTexture.width / rect.width);
        int r = Mathf.RoundToInt(radiusInPixel);

        bool changed = false;

        for (int y = -r; y <= r; y++)
        {
            for (int x = -r; x <= r; x++)
            {
                if (x * x + y * y > r * r) continue;

                int px = centerX + x;
                int py = centerY + y;

                if (px < 0 || px >= stainTexture.width || py < 0 || py >= stainTexture.height)
                    continue;

                int index = py * stainTexture.width + px;

                if (pixels[index].a > 1)
                {
                    pixels[index].a = 0;
                    changed = true;
                }
            }
        }

        if (changed)
        {
            stainTexture.SetPixels32(pixels);
            stainTexture.Apply();
        }
    }

    void CheckFinish()
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a > 1)
            {
                return;
            }
        }

        GameClear();
    }

    void GameClear()
    {
        if (gameFinished)
        {
            return;
        }

        gameFinished = true;

        if (resultText != null)
        {
            resultText.text = "清理完成！";
            resultText.transform.localScale = Vector3.zero;
            resultText.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }

        cottonBall.DOPunchScale(Vector3.one * 0.25f, 0.3f, 6);

        Debug.Log("污渍全部清理完成");

        // 等 1 秒，让玩家看到“清理完成！”后再跳转
        DOVirtual.DelayedCall(1f, () =>
        {
            MiniGameFinish finish = FindObjectOfType<MiniGameFinish>();

            if (finish != null)
            {
                finish.FinishMiniGame();
            }
            else
            {
                Debug.LogError("场景中没有 MiniGameFinish，无法完成小游戏跳转！");
            }
        });
    }
}