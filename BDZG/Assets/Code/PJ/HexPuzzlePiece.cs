using UnityEngine;
using DG.Tweening;

public class HexPuzzlePiece : MonoBehaviour
{
    [Header("旋转根节点，拖 RotateRoot")]
    public Transform rotateRoot;

    [Header("高亮边框，拖 Square")]
    public GameObject highlightObj;

    [Header("每次旋转角度")]
    public float rotateAngle = 60f;

    [Header("旋转动画时间")]
    public float rotateDuration = 0.2f;

    private bool isRotating = false;

    // 当前偏离原始状态的步数：0~5
    // 0 = 和最开始一样
    // 1 = 转了60度
    // 2 = 转了120度
    private int currentStep = 0;

    void Awake()
    {
        currentStep = 0;
    }

    void Start()
    {
        if (highlightObj != null)
        {
            highlightObj.SetActive(false);
        }
    }

    public void SetSelected(bool selected)
    {
        if (highlightObj != null)
        {
            highlightObj.SetActive(selected);
        }
    }

    public bool IsRotating()
    {
        return isRotating;
    }

    // 开局随机旋转角度
    public void RandomRotation()
    {
        if (rotateRoot == null)
        {
            Debug.LogError(name + " 没有设置 RotateRoot");
            return;
        }

        // 随机 1~5，避免一开始就是正确的
        int randomStep = Random.Range(1, 6);

        currentStep = randomStep;

        rotateRoot.localRotation = Quaternion.Euler(0, 0, randomStep * rotateAngle);
    }

    // Q：逆时针旋转
    public void RotateLeft(System.Action onComplete = null)
    {
        if (isRotating || rotateRoot == null)
        {
            return;
        }

        currentStep++;

        if (currentStep >= 6)
        {
            currentStep = 0;
        }

        RotateByStep(rotateAngle, onComplete);
    }

    // E：顺时针旋转
    public void RotateRight(System.Action onComplete = null)
    {
        if (isRotating || rotateRoot == null)
        {
            return;
        }

        currentStep--;

        if (currentStep < 0)
        {
            currentStep = 5;
        }

        RotateByStep(-rotateAngle, onComplete);
    }

    private void RotateByStep(float angle, System.Action onComplete)
    {
        isRotating = true;

        // 重点：这里是“在当前角度基础上继续旋转60度”
        // 不是强制设置回某个角度
        rotateRoot
            .DOLocalRotate(
                new Vector3(0, 0, angle),
                rotateDuration,
                RotateMode.LocalAxisAdd
            )
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                isRotating = false;
                onComplete?.Invoke();
            });
    }

    // 判断是否回到原始角度
    public bool IsCorrect()
    {
        return currentStep == 0;
    }
}