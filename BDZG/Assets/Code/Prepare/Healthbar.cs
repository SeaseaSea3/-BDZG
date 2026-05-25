using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [Header("红色血条 Image，也就是满血")]
    public Image hpFillImage;

    [Header("血条抖动 Animator")]
    public Animator hpAnimator;

    [Header("抖动 Trigger 名")]
    public string shakeTriggerName = "Shake";

    [Header("最大血量")]
    public float maxHp = 100f;

    [Header("当前血量")]
    public float currentHp = 100f;

    [Header("每秒自动扣血")]
    public float timeDamagePerSecond = 1f;

    [Header("是否开启时间扣血")]
    public bool timeDamageRunning = true;

    void Start()
    {
        currentHp = maxHp;
        UpdateHpBar();
    }

    void Update()
    {
        if (timeDamageRunning)
        {
            ReduceHpByTime();
        }
    }

    // 按时间扣血：不会抖
    void ReduceHpByTime()
    {
        if (currentHp <= 0) return;

        currentHp -= timeDamagePerSecond * Time.deltaTime;

        if (currentHp < 0)
        {
            currentHp = 0;
        }

        UpdateHpBar();
    }

    // 游戏内扣血：会抖
    public void TakeDamage(float damage)
    {
        if (currentHp <= 0) return;

        currentHp -= damage;

        if (currentHp < 0)
        {
            currentHp = 0;
        }

        UpdateHpBar();
        PlayShake();
    }

    // 加血：不会抖
    public void AddHp(float value)
    {
        currentHp += value;

        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }

        UpdateHpBar();
    }

    void UpdateHpBar()
    {
        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = currentHp / maxHp;
        }
    }

    void PlayShake()
    {
        if (hpAnimator != null)
        {
            hpAnimator.ResetTrigger(shakeTriggerName);
            hpAnimator.SetTrigger(shakeTriggerName);
        }
    }
}