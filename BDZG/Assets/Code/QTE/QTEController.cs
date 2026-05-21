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

    private KeyCode[] sequence;
    private int currentIndex;
    private RectTransform arrowRect;

    private Vector2[] positions;

    [Header("主界面，可不填")]
    public GameObject mainUI;

    void Start()
    {
        arrowRect = arrowImage.GetComponent<RectTransform>();

        positions = new Vector2[]
        {
            new Vector2(-300, 0),
            new Vector2(-100, 0),
            new Vector2(100, 0),
            new Vector2(300, 0)
        };

        // 不需要点击开始，进入场景后自动开始
        StartQTE();
    }

    public void StartQTE()
    {
        gameObject.SetActive(true);

        if (mainUI != null)
        {
            mainUI.SetActive(false);
        }

        isActive = true;

        if (resultText != null)
        {
            resultText.text = "";
        }

        if (arrowImage != null)
        {
            arrowImage.color = Color.white;
            arrowImage.DOKill();
            arrowImage.transform.DOKill();
            arrowImage.transform.localScale = Vector3.one;
        }

        GenerateSequence();
        currentIndex = 0;

        ShowArrow();
        ResetTimer();

        PlayShowAnim();
        PlayLoopAnim();
    }

    void Update()
    {
        if (!isActive) return;

        timer -= Time.deltaTime;

        if (timerBar != null)
        {
            timerBar.fillAmount = timer / maxTime;
        }

        if (Input.anyKeyDown)
        {
            PlayPressAnim();

            if (Input.GetKeyDown(sequence[currentIndex]))
            {
                NextStep();
            }
            else
            {
                Fail();
            }
        }

        if (timer <= 0)
        {
            Fail();
        }

        if (timer < 1f && arrowImage != null)
        {
            arrowImage.DOKill();
            arrowImage.DOColor(Color.red, 0.2f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    void GenerateSequence()
    {
        sequence = new KeyCode[4];

        KeyCode[] keys =
        {
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

    void ShowArrow()
    {
        KeyCode key = sequence[currentIndex];

        switch (key)
        {
            case KeyCode.UpArrow:
                arrowImage.sprite = up;
                break;

            case KeyCode.DownArrow:
                arrowImage.sprite = down;
                break;

            case KeyCode.LeftArrow:
                arrowImage.sprite = left;
                break;

            case KeyCode.RightArrow:
                arrowImage.sprite = right;
                break;
        }

        arrowImage.color = Color.white;

        arrowRect.DOAnchorPos(positions[currentIndex], 0.2f)
                 .SetEase(Ease.OutBack);
    }

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

        if (timerBar != null)
        {
            timerBar.fillAmount = 1f;
        }
    }

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
        {
            loopTween.Kill();
        }

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

    void Success()
    {
        isActive = false;

        if (loopTween != null)
        {
            loopTween.Kill();
        }

        resultText.text = "手术成功！";
        arrowImage.color = Color.green;

        arrowImage.transform
            .DOScale(1.5f, 0.2f)
            .OnComplete(() =>
            {
                arrowImage.DOFade(0, 0.3f);

            // 等 1 秒后自动完成小游戏，回血并返回主界面
            Invoke(nameof(FinishMiniGame), 1f);
            });
    }

    void FinishMiniGame()
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
    }


    void Fail()
    {
        if (!isActive) return;

        isActive = false;

        if (loopTween != null)
        {
            loopTween.Kill();
        }

        resultText.text = "手术失败！";
        arrowImage.color = Color.red;

        arrowImage.transform
            .DOShakePosition(0.3f, 20f, 10)
            .OnComplete(() =>
            {
                arrowImage.DOFade(0, 0.3f);
                Invoke(nameof(CloseQTE), 1f);
            });
    }

    void CloseQTE()
    {
        gameObject.SetActive(false);
    }
}