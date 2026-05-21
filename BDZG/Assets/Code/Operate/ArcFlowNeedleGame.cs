using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ArcFlowNeedleGame : MonoBehaviour
{
    [Header("针筒")]
    public RectTransform needle;
    public float needleDownDistance = 120f;
    public float needleDownTime = 0.12f;
    public float needleUpTime = 0.12f;

    [Header("针尖判定点")]
    public RectTransform hitPoint;

    [Header("弧形轨道点父物体")]
    public RectTransform trackPointRoot;

    [Header("判定条小段")]
    public RectTransform segmentPrefab;

    [Header("判定条设置")]
    public int maxSegmentCount = 3;
    public float moveSpeed = 10f;
    public float hitRange = 45f;
    public float rotationOffset = 0f;

    [Header("完成事件")]
    public UnityEvent onGameComplete;

    private RectTransform[] trackPoints;
    private RectTransform[] segments;

    private int currentSegmentCount;
    private float headIndex;
    private int direction = 1;

    private bool isStabbing = false;
    private bool isFinished = false;

    private Vector2 needleStartPos;
    private Sequence stabSequence;

    void Start()
    {
        InitGame();
    }

    void Update()
    {
        if (isFinished) return;

        MoveBar();

        if (Input.GetKeyDown(KeyCode.S) && !isStabbing)
        {
            Stab();
        }
    }

    void InitGame()
    {
        needleStartPos = needle.anchoredPosition;

        currentSegmentCount = maxSegmentCount;
        headIndex = currentSegmentCount - 1;
        direction = 1;

        isStabbing = false;
        isFinished = false;

        LoadTrackPoints();
        CreateSegments();
        UpdateSegments();
    }

    void LoadTrackPoints()
    {
        int count = trackPointRoot.childCount;
        trackPoints = new RectTransform[count];

        for (int i = 0; i < count; i++)
        {
            trackPoints[i] = trackPointRoot.GetChild(i).GetComponent<RectTransform>();
        }
    }

    void CreateSegments()
    {
        if (segments != null)
        {
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] != null)
                {
                    Destroy(segments[i].gameObject);
                }
            }
        }

        segments = new RectTransform[maxSegmentCount];

        for (int i = 0; i < maxSegmentCount; i++)
        {
            RectTransform seg = Instantiate(segmentPrefab, segmentPrefab.parent);
            seg.gameObject.SetActive(true);
            segments[i] = seg;
        }

        segmentPrefab.gameObject.SetActive(false);
    }

    void MoveBar()
    {
        if (trackPoints == null || trackPoints.Length < 2) return;

        headIndex += direction * moveSpeed * Time.deltaTime;

        if (headIndex >= trackPoints.Length - 1)
        {
            headIndex = trackPoints.Length - 1;
            direction = -1;
        }

        if (headIndex <= currentSegmentCount - 1)
        {
            headIndex = currentSegmentCount - 1;
            direction = 1;
        }

        UpdateSegments();
    }

    void UpdateSegments()
    {
        for (int i = 0; i < maxSegmentCount; i++)
        {
            if (i >= currentSegmentCount)
            {
                segments[i].gameObject.SetActive(false);
                continue;
            }

            segments[i].gameObject.SetActive(true);

            float index = headIndex - i;

            Vector3 pos = GetWorldPointByIndex(index);
            Vector3 nextPos = GetWorldPointByIndex(index + 0.2f);

            segments[i].position = pos;

            Vector3 dir = nextPos - pos;

            if (dir.magnitude > 0.01f)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                segments[i].rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
            }
        }
    }

    Vector3 GetWorldPointByIndex(float index)
    {
        index = Mathf.Clamp(index, 0, trackPoints.Length - 1);

        int i0 = Mathf.FloorToInt(index);
        int i1 = Mathf.Clamp(i0 + 1, 0, trackPoints.Length - 1);

        float t = index - i0;

        return Vector3.Lerp(
            trackPoints[i0].position,
            trackPoints[i1].position,
            t
        );
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
        bool hit = false;

        for (int i = 0; i < currentSegmentCount; i++)
        {
            float distance = Vector3.Distance(
                segments[i].position,
                hitPoint.position
            );

            if (distance <= hitRange)
            {
                hit = true;
                break;
            }
        }

        if (hit)
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
        currentSegmentCount--;

        Debug.Log("戳中，剩余长度：" + currentSegmentCount);

        if (currentSegmentCount <= 0)
        {
            CompleteGame();
        }
    }

    void CompleteGame()
    {
        isFinished = true;

        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].gameObject.SetActive(false);
        }

        Debug.Log("小游戏完成");

        onGameComplete?.Invoke();

        // 延迟 1 秒后跳回主界面
        DOVirtual.DelayedCall(1f, () =>
        {
            SceneManager.LoadScene("Operate");
        });
    }

    public void RestartGame()
    {
        stabSequence?.Kill();

        if (segments != null)
        {
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] != null)
                    Destroy(segments[i].gameObject);
            }
        }

        needle.anchoredPosition = needleStartPos;
        segmentPrefab.gameObject.SetActive(true);

        InitGame();
    }

    void OnDisable()
    {
        stabSequence?.Kill();
    }
}