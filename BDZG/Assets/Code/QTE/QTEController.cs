using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QTEController : MonoBehaviour
{
    [Header("UI")]
    public Image arrowImage;
    public Image timerBar;
    public Text resultText;

    [Header("方向图片")]
    public Sprite up, down, left, right;

    [Header("时间")]
    public float maxTime = 2f;

    private float timer;
    private bool isActive;

    private Tween loopTween;

    // ⭐ 新增
    private KeyCode[] sequence;
    private int currentIndex;
    private RectTransform arrowRect;

    // ⭐ 一行位置（4个）
    private Vector2[] positions;
    public GameObject mainUI;

    void Start()
    {
        gameObject.SetActive(false);

        arrowRect = arrowImage.GetComponent<RectTransform>();

        // ⭐ 一行4个位置（你可以自己调）
        positions = new Vector2[]
        {
        new Vector2(-300, 0),
        new Vector2(-100, 0),
        new Vector2(100, 0),
        new Vector2(300, 0)
        };
    }

    public void StartQTE()
    {
        gameObject.SetActive(true);

        isActive = true;
        resultText.text = "";

        GenerateSequence();   // ⭐生成4个按键
        currentIndex = 0;

        ShowArrow();          // ⭐显示第一个
        ResetTimer();

        PlayShowAnim();
        PlayLoopAnim();
    }

    void Update()
    {
        if (!isActive) return;

        timer -= Time.deltaTime;

        if (timerBar != null)
            timerBar.fillAmount = timer / maxTime;

        // 输入检测
        if (Input.anyKeyDown)
        {
            PlayPressAnim();

            if (Input.GetKeyDown(sequence[currentIndex]))
            {
                NextStep();  // ⭐改这里
            }
            else
            {
                Fail();
            }
        }

        // 超时
        if (timer <= 0)
        {
            Fail();
        }

        // 快结束闪红（避免重复叠加）
        if (timer < 1f && arrowImage != null)
        {
            arrowImage.DOKill(); // ⭐防止叠加动画
            arrowImage.DOColor(Color.red, 0.2f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    // ⭐生成4个方向
    void GenerateSequence()
    {
        sequence = new KeyCode[4];

        KeyCode[] keys = {
            KeyCode.UpArrow,
            KeyCode.DownArrow,
            KeyCode.LeftArrow,
            KeyCode.RightArrow
        };

        for (int i = 0; i < 4; i++)
        {
            sequence[i] = keys[Random.Range(0, keys.Length)];
        }
    }

    // ⭐显示当前箭头
    void ShowArrow()
    {
        KeyCode key = sequence[currentIndex];

        switch (key)
        {
            case KeyCode.UpArrow: arrowImage.sprite = up; break;
            case KeyCode.DownArrow: arrowImage.sprite = down; break;
            case KeyCode.LeftArrow: arrowImage.sprite = left; break;
            case KeyCode.RightArrow: arrowImage.sprite = right; break;
        }

        arrowImage.color = Color.white;

        // ⭐ 核心：移动到对应位置（横排）
        arrowRect.DOAnchorPos(positions[currentIndex], 0.2f)
                 .SetEase(Ease.OutBack);
    }

    // ⭐进入下一步
    void NextStep()
    {
        currentIndex++;

        if (currentIndex >= sequence.Length)
        {
            Success();
            return;
        }

        ResetTimer();
        ShowArrow();
    }

    void ResetTimer()
    {
        timer = maxTime;
    }

    // ===== 动画 =====

    void PlayShowAnim()
    {
        arrowImage.transform.localScale = Vector3.zero;

        arrowImage.transform.DOScale(1.2f, 0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                arrowImage.transform.DOScale(1f, 0.1f);
            });
    }

    void PlayLoopAnim()
    {
        if (loopTween != null)
            loopTween.Kill();

        loopTween = arrowImage.transform
            .DOScale(1.1f, 0.6f)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void PlayPressAnim()
    {
        arrowImage.transform
            .DOScale(0.8f, 0.05f)
            .OnComplete(() =>
            {
                arrowImage.transform.DOScale(1f, 0.1f);
            });
    }

    // ===== 结果 =====

    void Success()
    {
        isActive = false;

        if (loopTween != null)
            loopTween.Kill();

        resultText.text = "手术成功！";
        arrowImage.color = Color.green;

        arrowImage.transform
            .DOScale(1.5f, 0.2f)
            .OnComplete(() =>
            {
                arrowImage.DOFade(0, 0.3f);
                Invoke("CloseQTE", 1f);
            });
    }

    void Fail()
    {
        if (!isActive) return;

        isActive = false;

        if (loopTween != null)
            loopTween.Kill();

        resultText.text = "手术失败！";
        arrowImage.color = Color.red;

        arrowImage.transform
            .DOShakePosition(0.3f, 20f, 10)
            .OnComplete(() =>
            {
                arrowImage.DOFade(0, 0.3f);
                Invoke("CloseQTE", 1f);
            });
    }

    void CloseQTE()
    {
        gameObject.SetActive(false);

        // ⭐恢复主界面
        if (mainUI != null)
            mainUI.SetActive(true);
    }
}