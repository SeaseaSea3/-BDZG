using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TreatmentGameManager : MonoBehaviour
{
    public static TreatmentGameManager Instance;

    [Header("当前病人任务")]
    public PatientTaskData currentPatientTask;

    [Header("主界面场景名")]
    public string mainSceneName = "Operate";

    [Header("生命值")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("每秒损失百分比")]
    public float losePercentPerSecond = 1f;

    [Header("血条")]
    public Image healthFill;

    [Header("心率仪")]
    public ECGMonitorLine ecgLine;

    [Header("失败结束面板")]
    public GameObject endPanel;

    [Header("成功结束面板")]
    public GameObject successPanel;

    private bool isGameOver = false;
    private bool isTreatmentFinished = false;

    private HashSet<int> completedMiniGames = new HashSet<int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject.transform.root.gameObject);
            return;
        }

        Instance = this;

        // 让整个 Canvas 或 GameManager 在切换小游戏场景时不销毁
        DontDestroyOnLoad(gameObject.transform.root.gameObject);
    }

    private void Start()
    {
        InitTreatment();
    }

    private void Update()
    {
        if (isGameOver || isTreatmentFinished)
        {
            return;
        }

        currentHealth -= losePercentPerSecond * Time.deltaTime;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            UpdateHealthUI();
            GameOver();
            return;
        }

        UpdateHealthUI();
    }

    public void InitTreatment()
    {
        Time.timeScale = 1f;

        currentHealth = maxHealth;
        isGameOver = false;
        isTreatmentFinished = false;

        completedMiniGames.Clear();

        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }

        if (successPanel != null)
        {
            successPanel.SetActive(false);
        }

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        float rate = currentHealth / maxHealth;

        if (healthFill != null)
        {
            healthFill.fillAmount = rate;
        }

        if (ecgLine != null)
        {
            ecgLine.SetHealth(currentHealth);
        }
    }

    public bool IsRequiredMiniGame(int miniGameId)
    {
        if (currentPatientTask == null)
        {
            Debug.LogError("没有设置 Current Patient Task");
            return false;
        }

        for (int i = 0; i < currentPatientTask.requiredMiniGameIds.Length; i++)
        {
            if (currentPatientTask.requiredMiniGameIds[i] == miniGameId)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsMiniGameCompleted(int miniGameId)
    {
        return completedMiniGames.Contains(miniGameId);
    }

    public void CompleteMiniGame(int miniGameId)
    {
        if (isGameOver || isTreatmentFinished)
        {
            return;
        }

        if (!IsRequiredMiniGame(miniGameId))
        {
            Debug.Log("这个小游戏不是当前病人需要的：" + miniGameId);
            return;
        }

        if (completedMiniGames.Contains(miniGameId))
        {
            Debug.Log("这个小游戏已经完成过了：" + miniGameId);
            SceneManager.LoadScene(mainSceneName);
            return;
        }

        completedMiniGames.Add(miniGameId);

        float healValue = GetHealAmount(miniGameId);
        Heal(healValue);

        Debug.Log("完成小游戏：" + miniGameId + "，回血：" + healValue);

        if (CheckAllRequiredMiniGamesFinished())
        {
            TreatmentSuccess();
        }
        else
        {
            SceneManager.LoadScene(mainSceneName);
        }
    }

    private float GetHealAmount(int miniGameId)
    {
        if (currentPatientTask == null)
        {
            return 0;
        }

        for (int i = 0; i < currentPatientTask.requiredMiniGameIds.Length; i++)
        {
            if (currentPatientTask.requiredMiniGameIds[i] == miniGameId)
            {
                if (i < currentPatientTask.healAmounts.Length)
                {
                    return currentPatientTask.healAmounts[i];
                }
            }
        }

        return 0;
    }

    private bool CheckAllRequiredMiniGamesFinished()
    {
        if (currentPatientTask == null)
        {
            return false;
        }

        for (int i = 0; i < currentPatientTask.requiredMiniGameIds.Length; i++)
        {
            int id = currentPatientTask.requiredMiniGameIds[i];

            if (!completedMiniGames.Contains(id))
            {
                return false;
            }
        }

        return true;
    }

    public void Damage(float value)
    {
        if (isGameOver || isTreatmentFinished)
        {
            return;
        }

        currentHealth -= value;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            UpdateHealthUI();
            GameOver();
            return;
        }

        UpdateHealthUI();
    }

    public void Heal(float value)
    {
        if (isGameOver || isTreatmentFinished)
        {
            return;
        }

        currentHealth += value;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthUI();
    }

    private void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }

        Debug.Log("生命值为 0，游戏失败");
    }

    private void TreatmentSuccess()
    {
        isTreatmentFinished = true;
        Time.timeScale = 0f;

        if (successPanel != null)
        {
            successPanel.SetActive(true);
        }
        else if (endPanel != null)
        {
            endPanel.SetActive(true);
        }

        Debug.Log("三个小游戏全部完成，治疗成功");
    }

    public void RestartTreatment()
    {
        Time.timeScale = 1f;
        InitTreatment();
        SceneManager.LoadScene(mainSceneName);
    }

    public void BackToMain()
    {
        Time.timeScale = 1f;
        InitTreatment();
        SceneManager.LoadScene(mainSceneName);
    }
}