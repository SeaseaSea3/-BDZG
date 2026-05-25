using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonitorIncomingEntry
{
    [Header("客人总数区间（含边界）")]
    public int minTotalGuests;
    public int maxTotalGuests = 99;

    [Header("特殊客人数区间（含边界，-1 表示不限制）")]
    public int minSpecialGuests = -1;
    public int maxSpecialGuests = -1;

    [Tooltip("匹配后使用的对话根节点，拖入预制 DialogueNode")]
    public DialogueNode startNode;
}

[CreateAssetMenu(fileName = "MonitorIncomingConfig", menuName = "BBPhone/Monitor Incoming Config")]
public class MonitorIncomingConfig : ScriptableObject
{
    [Tooltip("按监控客人数据匹配；每条拖入对应该情况的 DialogueNode 根节点")]
    public List<MonitorIncomingEntry> entries = new List<MonitorIncomingEntry>();

    public DialogueNode Resolve(IMonitorGuestProvider provider)
    {
        if (provider == null || entries == null)
            return null;

        int total = provider.TotalGuestCount;
        int special = provider.SpecialGuestCount;

        foreach (var e in entries)
        {
            if (e.startNode == null)
                continue;
            if (total < e.minTotalGuests || total > e.maxTotalGuests)
                continue;
            if (e.minSpecialGuests >= 0 && special < e.minSpecialGuests)
                continue;
            if (e.maxSpecialGuests >= 0 && special > e.maxSpecialGuests)
                continue;
            return e.startNode;
        }

        return null;
    }
}
