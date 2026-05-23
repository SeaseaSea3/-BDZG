using UnityEngine;

[System.Serializable]
public class StateObjectProbability
{
    public int stateIndex;          // 对应状态 0=A 1=B 2=C 3=D 4=E 5=猫眼
    public GameObject[] objects;    // 这个状态下要控制的物体
    public float[] probabilities;   // 每个物体的出现概率（长度必须和objects一样）
    [Range(0, 100)] public float nothingProb; // 都不出现概率
}

/// <summary>
/// 仅监听状态变化，自动按概率显示/隐藏物体
/// 不影响你现有的任何切换逻辑
/// </summary>
public class StateProbabilityDisplay : MonoBehaviour
{
    [Header("你的动画状态机")]
    public Animator animator;

    [Header("状态概率配置（每个状态独立）")]
    public StateObjectProbability[] stateSettings;

    private int lastState = -1;

    void Update()
    {
        int currentState = animator.GetInteger("StateIndex");

        // 只有状态变化时才执行
        if (currentState != lastState)
        {
            lastState = currentState;
            OnStateChanged(currentState);
        }
    }

    /// <summary>
    /// 状态切换时触发：概率显示物体
    /// </summary>
    void OnStateChanged(int state)
    {
        // 先隐藏所有物体
        HideAllObjects();

        // 找到当前状态的配置
        foreach (var setting in stateSettings)
        {
            if (setting.stateIndex != state) continue;

            // 总概率
            float total = setting.nothingProb;
            foreach (float p in setting.probabilities) total += p;

            float random = Random.Range(0, total);
            float current = 0;

            // 按概率显示物体
            for (int i = 0; i < setting.objects.Length; i++)
            {
                current += setting.probabilities[i];
                if (random < current)
                {
                    if (setting.objects[i] != null)
                        setting.objects[i].SetActive(true);
                    return;
                }
            }
            return;
        }
    }

    /// <summary>
    /// 隐藏所有配置过的物体
    /// </summary>
    void HideAllObjects()
    {
        foreach (var s in stateSettings)
            foreach (var obj in s.objects)
                if (obj != null) obj.SetActive(false);
    }
}