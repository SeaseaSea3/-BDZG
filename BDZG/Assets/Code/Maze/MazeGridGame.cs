using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class ArcNeedleTrackGame : MonoBehaviour
{
    [Header("针筒")]
    public RectTransform needle;
    public float needleDownDistance = 120f;
    public float needleDownTime = 0.12f;
    public float needleUpTime = 0.12f;

    [Header("判定线")]
    public RectTransform hitBar;
    public Image hitBarImage;

    [Header("弧形框里面的三个轨道点")]
    public RectTransform leftPoint;
    public RectTransform middlePoint;
    public RectTransform rightPoint;

    [Header("轨道设置")]
    public float moveTime = 0.8f;
    public Vector2 trackOffset = Vector2.zero;

    [Header("自动贴合弧线方向")]
    public bool autoRotate = true;
    public float rotationOffset = 0f;

    [Header("命中判定")]
    public RectTransform hitPoint;
    public float hitRange = 70f;

    [Header("次数")]
    public int maxHitCount = 3;

    [Header("完成事件")]
    public UnityEvent onGameComplete;

    private Vector2 needleStartPos;
    private Vector2 lastHitBarPos;

    private int currentHitCount = 0;
    private bool isStabbing = false;
    private bool isFinished = false;

    private Sequence moveSequence;
    private Sequence stabSequence;

    void Start()
    {
        InitGame();
    }

    void Update()
    {
        if (isFinished) return;

        if (Input.GetKeyDown(KeyCode.S) && !isStabbing)
        {
            Stab();
        }
    }

    void LateUpdate()
    {
        if (isFinished) return;

        if (autoRotate)
        {
            RotateAlongTrack();
        }
    }

    void InitGame()
    {
        needleStartPos = needle.anchoredPosition;

        currentHitCount = 0;
        isStabbing = false;
        isFinished = false;

        hitBarImage.fillAmount = 1f;

        hitBar.anchoredPosition = leftPoint.anchoredPosition + trackOffset;
        lastHitBarPos = hitBar.anchoredPosition;

        StartMove();
    }

    void StartMove()
    {
        moveSequence?.Kill();

        Vector2 leftPos = leftPoint.anchoredPosition + trackOffset;
        Vector2 middlePos = middlePoint.anchoredPosition + trackOffset;
        Vector2 rightPos = rightPoint.anchoredPosition + trackOffset;

        hitBar.anchoredPosition = leftPos;
        lastHitBarPos = leftPos;

        moveSequence = DOTween.Sequence();

        moveSequence.Append(
            hitBar.DOAnchorPos(middlePos, moveTime)
                .SetEase(Ease.InOutSine)
        );

        moveSequence.Append(
            hitBar.DOAnchorPos(rightPos, moveTime)
                .SetEase(Ease.InOutSine)
        );

        moveSequence.Append(
            hitBar.DOAnchorPos(middlePos, moveTime)
                .SetEase(Ease.InOutSine)
        );

        moveSequence.Append(
            hitBar.DOAnchorPos(leftPos, moveTime)
                .SetEase(Ease.InOutSine)
        );

        moveSequence.SetLoops(-1);
    }

    void RotateAlongTrack()
    {
        Vector2 currentPos = hitBar.anchoredPosition;
        Vector2 direction = currentPos - lastHitBarPos;

        if (direction.magnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            hitBar.localRotation = Quaternion.Euler(0, 0, angle + rotationOffset);
        }

        lastHitBarPos = currentPos;
    }

    void Stab()
    {
        isStabbing = true;

        stabSequence?.Kill();

        stabSequence = DOTween.Sequence();

        stabSequence.Append(
            needle.DOAnchorPosY(needleStartPos.y - needleDownDistance, needleDownTime)
                .SetEase(Ease.OutQuad)
        );

        stabSequence.AppendCallback(CheckHit);

        stabSequence.Append(
            needle.DOAnchorPosY(needleStartPos.y, needleUpTime)
                .SetEase(Ease.OutQuad)
        );

        stabSequence.OnComplete(() =>
        {
            isStabbing = false;
        });
    }

    void CheckHit()
    {
        float distance = Vector2.Distance(hitBar.anchoredPosition, hitPoint.anchoredPosition);

        if (distance <= hitRange)
        {
            HitSuccess();
        }
        else
        {
            Debug.Log("没戳中");
        }
    }

    void HitSuccess()
    {
        currentHitCount++;

        float remain = 1f - (float)currentHitCount / maxHitCount;
        remain = Mathf.Clamp01(remain);

        hitBarImage.DOFillAmount(remain, 0.15f)
            .SetEase(Ease.OutQuad);

        Debug.Log("戳中：" + currentHitCount);

        if (currentHitCount >= maxHitCount)
        {
            CompleteGame();
        }
    }

    void CompleteGame()
    {
        isFinished = true;

        moveSequence?.Kill();
        stabSequence?.Kill();

        hitBarImage.fillAmount = 0f;

        Debug.Log("小游戏完成");

        onGameComplete?.Invoke();
    }

    public void RestartGame()
    {
        moveSequence?.Kill();
        stabSequence?.Kill();

        needle.anchoredPosition = needleStartPos;
        hitBar.localRotation = Quaternion.identity;

        InitGame();
    }

    void OnDisable()
    {
        moveSequence?.Kill();
        stabSequence?.Kill();
    }
}