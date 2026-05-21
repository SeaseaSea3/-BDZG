using UnityEngine;
using UnityEngine.Events;

public class EyeQTE_Angle : MonoBehaviour
{
    [Header("组件引用")]
    public Transform eyeCenter;        // 旋转中心
    public Transform qteZone;          // 判定区域

    [Header("轨道参数")]
    public float radius = 2f;
    public float minSpeed = 40f;
    public float maxSpeed = 120f;

    [Header("QTE参数")]
    public float successRange = 15f;    // 成功角度范围（度）
    public KeyCode qteKey = KeyCode.Q;
    public bool randomizeZonePosition = true;  // 是否随机判定区域位置

    [Header("事件")]
    public UnityEvent onSuccess;
    public UnityEvent onFail;

    [Header("视觉反馈")]
    public Color successColor = Color.green;
    public Color failColor = Color.gray;
    public Color readyColor = Color.yellow;

    // 私有变量
    private float currentAngle;
    private float currentSpeed;
    private bool hasTriggered = false;
    private bool isSuccess = false;
    private bool isInZone = false;
    private SpriteRenderer pupilRenderer;
    private Color originalColor;

    // 判定区域显示控制
    private SpriteRenderer qteZoneRenderer;
    private Collider2D qteZoneCollider;
    private bool isQTEZoneActive = true;

    // 公共属性
    public bool IsSuccess => isSuccess;
    public SpriteRenderer PupilRenderer => pupilRenderer;
    public Color OriginalColor => originalColor;

    void Start()
    {
        // 获取眼珠渲染器
        pupilRenderer = GetComponent<SpriteRenderer>();
        if (pupilRenderer != null)
        {
            originalColor = pupilRenderer.color;
        }

        // 获取判定区域的组件
        if (qteZone != null)
        {
            qteZoneRenderer = qteZone.GetComponent<SpriteRenderer>();
            qteZoneCollider = qteZone.GetComponent<Collider2D>();
        }

        // 随机初始角度和速度
        currentAngle = Random.Range(0f, 360f);
        currentSpeed = Random.Range(minSpeed, maxSpeed);

        // 随机判定区域位置（在圆周上）
        if (randomizeZonePosition && qteZone != null && eyeCenter != null)
        {
            RandomizeQTEPosition();
        }
    }

    // 随机移动判定区域到圆周上的新位置
    public void RandomizeQTEPosition()
    {
        if (qteZone == null || eyeCenter == null) return;

        float newAngle = Random.Range(0f, 360f);
        float rad = newAngle * Mathf.Deg2Rad;

        qteZone.position = eyeCenter.position + new Vector3(
            Mathf.Cos(rad) * radius,
            Mathf.Sin(rad) * radius,
            0
        );

        Debug.Log($"{gameObject.name} 判定区域移动到角度: {newAngle:F0}°");
    }

    // 设置判定区域到指定角度
    public void SetQTEPosition(float angle)
    {
        if (qteZone == null || eyeCenter == null) return;

        float rad = angle * Mathf.Deg2Rad;
        qteZone.position = eyeCenter.position + new Vector3(
            Mathf.Cos(rad) * radius,
            Mathf.Sin(rad) * radius,
            0
        );
    }

    void Update()
    {
        // 如果已经成功，停止运动
        if (isSuccess)
        {
            return;
        }

        // 安全检查
        if (eyeCenter == null || qteZone == null)
        {
            return;
        }

        // 随机速度变化
        currentSpeed += Random.Range(-10f, 10f) * Time.deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);

        // 更新角度
        currentAngle += currentSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;

        // 更新眼珠位置
        float rad = currentAngle * Mathf.Deg2Rad;
        transform.position = eyeCenter.position + new Vector3(
            Mathf.Cos(rad) * radius,
            Mathf.Sin(rad) * radius,
            0
        );

        // ========== 角度判定 ==========

        // 只有判定区域激活时才进行判定
        if (isQTEZoneActive)
        {
            float zoneAngle = GetAngleFromPosition(qteZone.position);
            float angleDiff = GetAngleDifference(currentAngle, zoneAngle);

            bool wasInZone = isInZone;
            isInZone = Mathf.Abs(angleDiff) <= successRange;

            // 离开区域时重置 hasTriggered
            if (wasInZone && !isInZone)
            {
                hasTriggered = false;
                Debug.Log($"{gameObject.name} 离开判定区域");
            }

            // 进入区域时提示
            if (!wasInZone && isInZone && !hasTriggered && !isSuccess)
            {
                Debug.Log($"{gameObject.name} 进入判定区域！按 {qteKey} 键");
            }

            // 视觉反馈：在区域内且未成功时变黄
            if (isInZone && !hasTriggered && !isSuccess)
            {
                if (pupilRenderer != null && pupilRenderer.color != readyColor)
                {
                    pupilRenderer.color = readyColor;
                }
            }
            else if (!isInZone && pupilRenderer != null && pupilRenderer.color == readyColor)
            {
                pupilRenderer.color = originalColor;
            }

            // QTE判定
            if (isInZone && !hasTriggered && !isSuccess && Input.GetKeyDown(qteKey))
            {
                OnQTESuccess();
            }
            else if (!isInZone && !isSuccess && Input.GetKeyDown(qteKey))
            {
                OnQTEFail();
            }
        }
        else
        {
            // 判定区域未激活时，确保颜色正常
            if (pupilRenderer != null && pupilRenderer.color != originalColor && !isSuccess)
            {
                pupilRenderer.color = originalColor;
            }
        }
    }

    // 从世界坐标获取角度（相对于eyeCenter）
    float GetAngleFromPosition(Vector3 position)
    {
        Vector3 relativePos = position - eyeCenter.position;
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;
        return angle;
    }

    // 计算两个角度的最小差值
    float GetAngleDifference(float from, float to)
    {
        float diff = (to - from + 360) % 360;
        if (diff > 180) diff -= 360;
        return diff;
    }

    void OnQTESuccess()
    {
        isSuccess = true;
        hasTriggered = true;

        // 眼珠移动到判定区域位置（重合）
        transform.position = qteZone.position;

        if (pupilRenderer != null) pupilRenderer.color = successColor;

        Debug.Log($"{gameObject.name} QTE成功！");
        onSuccess?.Invoke();
    }

    void OnQTEFail()
    {
        Debug.Log($"{gameObject.name} QTE失败！");

        if (pupilRenderer != null) pupilRenderer.color = failColor;
        onFail?.Invoke();
        Invoke(nameof(ResetFailColor), 0.3f);
    }

    void ResetFailColor()
    {
        if (pupilRenderer != null && !isSuccess)
        {
            if (isInZone)
                pupilRenderer.color = readyColor;
            else
                pupilRenderer.color = originalColor;
        }
    }

    // 显示判定区域
    public void ShowQTEZone()
    {
        if (qteZone == null) return;

        isQTEZoneActive = true;

        // 显示视觉图片
        if (qteZoneRenderer != null)
            qteZoneRenderer.enabled = true;

        // 启用碰撞体（如果用碰撞检测）
        if (qteZoneCollider != null)
            qteZoneCollider.enabled = true;

        Debug.Log($"{gameObject.name} 判定区域已显示");
    }

    // 隐藏判定区域
    public void HideQTEZone()
    {
        if (qteZone == null) return;

        isQTEZoneActive = false;
        isInZone = false;
        hasTriggered = false;

        // 隐藏视觉图片
        if (qteZoneRenderer != null)
            qteZoneRenderer.enabled = false;

        // 禁用碰撞体
        if (qteZoneCollider != null)
            qteZoneCollider.enabled = false;

        // 恢复颜色
        if (pupilRenderer != null && !isSuccess)
        {
            pupilRenderer.color = originalColor;
        }

        Debug.Log($"{gameObject.name} 判定区域已隐藏");
    }

    // 重置眼睛（保留随机位置）
    public void ResetEye()
    {
        isSuccess = false;
        hasTriggered = false;
        isInZone = false;
        currentAngle = Random.Range(0f, 360f);
        currentSpeed = Random.Range(minSpeed, maxSpeed);

        if (pupilRenderer != null)
            pupilRenderer.color = originalColor;

        // 可选：重置时重新随机判定区域位置
        if (randomizeZonePosition)
        {
            RandomizeQTEPosition();
        }

        Debug.Log($"{gameObject.name} 已重置");
    }

    // 重置眼睛并随机判定区域位置
    public void ResetEyeWithRandomZone()
    {
        ResetEye();
        RandomizeQTEPosition();
    }

    // 设置QTE激活状态
    public void SetQTEEnalbed(bool enabled)
    {
        isQTEZoneActive = enabled;
        if (!enabled)
        {
            isInZone = false;
            hasTriggered = false;
            if (pupilRenderer != null && !isSuccess)
                pupilRenderer.color = originalColor;
        }
    }

    // 可视化调试
    void OnDrawGizmosSelected()
    {
        if (eyeCenter != null)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(eyeCenter.position, radius);

            if (qteZone != null)
            {
                float zoneAngle = GetAngleFromPosition(qteZone.position);
                Vector3 leftBound = eyeCenter.position + AngleToVector(zoneAngle - successRange) * radius;
                Vector3 rightBound = eyeCenter.position + AngleToVector(zoneAngle + successRange) * radius;

                Gizmos.color = Color.green;
                Gizmos.DrawLine(eyeCenter.position, leftBound);
                Gizmos.DrawLine(eyeCenter.position, rightBound);

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(qteZone.position, 0.2f);
            }
        }
    }

    Vector3 AngleToVector(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
    }
}