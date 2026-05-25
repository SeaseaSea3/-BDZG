using System;
using UnityEngine;

/// <summary>
/// 占位实现：在 Inspector 填测试用人数。监控脚本就绪后换为 MonitorGuestProviderLive。
/// </summary>
public class MonitorGuestProviderStub : MonoBehaviour, IMonitorGuestProvider
{
    [Header("测试用假数据（监控接口接好后可删此组件）")]
    [SerializeField] private int stubTotalGuests;
    [SerializeField] private int stubSpecialGuests;
    [SerializeField] private int stubMonitorState;

    public int TotalGuestCount => stubTotalGuests;
    public int SpecialGuestCount => stubSpecialGuests;
    public int CurrentMonitorState => stubMonitorState;

    public event Action OnGuestDataChanged;

    /// <summary>运行时改假数据并通知订阅者（测试用）。</summary>
    public void SetStubCounts(int total, int special, int monitorState = 0)
    {
        stubTotalGuests = total;
        stubSpecialGuests = special;
        stubMonitorState = monitorState;
        OnGuestDataChanged?.Invoke();
    }

    private void OnValidate()
    {
        OnGuestDataChanged?.Invoke();
    }
}
