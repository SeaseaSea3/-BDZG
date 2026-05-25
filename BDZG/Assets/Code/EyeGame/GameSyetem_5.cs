using UnityEngine;
using UnityEngine.Events;

public class GameSystem : MonoBehaviour
{
    [Header("预制体")]
    public GameObject leftEyePrefab;     // 左眼预制体
    public GameObject rightEyePrefab;    // 右眼预制体

    [Header("生成位置")]
    public Transform leftEyeSpawnPoint;   // 左眼生成位置
    public Transform rightEyeSpawnPoint;  // 右眼生成位置

    [Header("游戏参数")]
    public bool autoStart = true;         // 是否自动开始
    public float leftRadius = 2f;
    public float rightRadius = 2f;
    public float minSpeed = 40f;
    public float maxSpeed = 120f;
    public float successRange = 15f;

    [Header("顺序控制")]
    public bool requireOrder = true;      // 是否需要先左后右

    [Header("事件")]
    public UnityEvent onGameStart;
    public UnityEvent onLeftEyeSuccess;
    public UnityEvent onRightEyeSuccess;
    public UnityEvent onGameComplete;
    public UnityEvent onGameReset;

    // 运行时引用
    private GameObject currentLeftEye;
    private GameObject currentRightEye;
    private EyeQTE_Angle leftEyeScript;
    private EyeQTE_Angle rightEyeScript;

    private bool gameActive = false;
    private bool leftCompleted = false;
    private bool rightCompleted = false;

    // 公共属性
    public bool IsGameActive => gameActive;
    public bool IsLeftCompleted => leftCompleted;
    public bool IsRightCompleted => rightCompleted;

    void Start()
    {
        if (autoStart)
        {
            StartGame();
        }
    }

    // ========== 公共接口 ==========

    /// <summary>
    /// 开始游戏：生成眼睛并开始
    /// </summary>
    public void StartGame()
    {
        if (gameActive)
        {
            Debug.LogWarning("游戏已在进行中");
            return;
        }

        // 清除旧物体
        ClearEyes();

        // 重置状态
        ResetState();

        // 生成眼睛
        SpawnEyes();

        // 设置游戏激活
        gameActive = true;

        // 触发事件
        onGameStart?.Invoke();

        Debug.Log("游戏开始！");
    }

    /// <summary>
    /// 结束游戏
    /// </summary>
    public void EndGame()
    {
        gameActive = false;
        Debug.Log("游戏结束");
    }

    /// <summary>
    /// 重置游戏（清除眼睛，重置状态）
    /// </summary>
    public void ResetGame()
    {
        ClearEyes();
        ResetState();
        gameActive = false;
        onGameReset?.Invoke();
        Debug.Log("游戏已重置");
    }

    /// <summary>
    /// 重新开始（先重置再开始）
    /// </summary>
    public void RestartGame()
    {
        ResetGame();
        StartGame();
    }

    /// <summary>
    /// 设置左眼生成位置
    /// </summary>
    public void SetLeftSpawnPosition(Vector3 position)
    {
        if (leftEyeSpawnPoint == null)
        {
            GameObject go = new GameObject("LeftEyeSpawnPoint");
            leftEyeSpawnPoint = go.transform;
        }
        leftEyeSpawnPoint.position = position;
    }

    /// <summary>
    /// 设置右眼生成位置
    /// </summary>
    public void SetRightSpawnPosition(Vector3 position)
    {
        if (rightEyeSpawnPoint == null)
        {
            GameObject go = new GameObject("RightEyeSpawnPoint");
            rightEyeSpawnPoint = go.transform;
        }
        rightEyeSpawnPoint.position = position;
    }

    /// <summary>
    /// 手动生成左眼
    /// </summary>
    public GameObject SpawnLeftEyeOnly()
    {
        if (leftEyePrefab == null)
        {
            Debug.LogError("左眼预制体未设置");
            return null;
        }

        Vector3 spawnPos = leftEyeSpawnPoint != null ? leftEyeSpawnPoint.position : Vector3.zero;
        currentLeftEye = Instantiate(leftEyePrefab, spawnPos, Quaternion.identity);
        leftEyeScript = currentLeftEye.GetComponentInChildren<EyeQTE_Angle>();

        // 设置参数
        if (leftEyeScript != null)
        {
            leftEyeScript.radius = leftRadius;
            leftEyeScript.minSpeed = minSpeed;
            leftEyeScript.maxSpeed = maxSpeed;
            leftEyeScript.successRange = successRange;
            leftEyeScript.onSuccess.AddListener(OnLeftEyeSuccess);
        }

        return currentLeftEye;
    }

    /// <summary>
    /// 手动生成右眼（不自动开始）
    /// </summary>
    public GameObject SpawnRightEyeOnly()
    {
        if (rightEyePrefab == null)
        {
            Debug.LogError("右眼预制体未设置");
            return null;
        }

        Vector3 spawnPos = rightEyeSpawnPoint != null ? rightEyeSpawnPoint.position : Vector3.zero;
        currentRightEye = Instantiate(rightEyePrefab, spawnPos, Quaternion.identity);
        rightEyeScript = currentRightEye.GetComponentInChildren<EyeQTE_Angle>();

        // 设置参数
        if (rightEyeScript != null)
        {
            rightEyeScript.radius = rightRadius;
            rightEyeScript.minSpeed = minSpeed;
            rightEyeScript.maxSpeed = maxSpeed;
            rightEyeScript.successRange = successRange;
            rightEyeScript.onSuccess.AddListener(OnRightEyeSuccess);

            // 如果需要顺序控制，开始时隐藏右眼判定区域
            if (requireOrder)
            {
                rightEyeScript.HideQTEZone();
            }
        }

        return currentRightEye;
    }

    /// <summary>
    /// 获取左眼脚本
    /// </summary>
    public EyeQTE_Angle GetLeftEye()
    {
        return leftEyeScript;
    }

    /// <summary>
    /// 获取右眼脚本
    /// </summary>
    public EyeQTE_Angle GetRightEye()
    {
        return rightEyeScript;
    }

    /// <summary>
    /// 检查游戏是否完成
    /// </summary>
    public bool IsGameComplete()
    {
        return leftCompleted && rightCompleted;
    }

    // ========== 私有方法 ==========

    private void SpawnEyes()
    {
        SpawnLeftEyeOnly();
        SpawnRightEyeOnly();
    }

    private void ClearEyes()
    {
        if (currentLeftEye != null) Destroy(currentLeftEye);
        if (currentRightEye != null) Destroy(currentRightEye);
        leftEyeScript = null;
        rightEyeScript = null;
    }

    private void ResetState()
    {
        leftCompleted = false;
        rightCompleted = false;
    }

    private void OnLeftEyeSuccess()
    {
        if (!gameActive) return;

        leftCompleted = true;
        Debug.Log("左眼成功！");
        onLeftEyeSuccess?.Invoke();

        // 如果需要顺序控制，解锁右眼
        if (requireOrder && rightEyeScript != null)
        {
            rightEyeScript.ShowQTEZone();
            Debug.Log("右眼判定区域已解锁！");
        }

    }

    private void OnRightEyeSuccess()
    {
        if (!gameActive) return;

        // 顺序控制：如果要求先左后右，但左眼未完成
        if (requireOrder && !leftCompleted)
        {
            Debug.Log("请先完成左眼判定！");
            return;
        }

        rightCompleted = true;
        Debug.Log("右眼成功！");
        onRightEyeSuccess?.Invoke();

    }


    void OnDestroy()
    {
        // 清理监听器
        if (leftEyeScript != null)
            leftEyeScript.onSuccess.RemoveListener(OnLeftEyeSuccess);
        if (rightEyeScript != null)
            rightEyeScript.onSuccess.RemoveListener(OnRightEyeSuccess);
    }
}