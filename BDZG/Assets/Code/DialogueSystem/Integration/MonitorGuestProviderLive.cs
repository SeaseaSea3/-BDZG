using System;
using UnityEngine;

/// <summary>
/// 监控正式接入点：在此读取同事提供的 GuestManager / 队列脚本，并映射到 IMonitorGuestProvider。
/// 当前为空壳，接好监控 API 后只改本文件内部实现。
/// </summary>
public class MonitorGuestProviderLive : MonoBehaviour, IMonitorGuestProvider
{
    public int TotalGuestCount => 0;
    public int SpecialGuestCount => 0;
    public int CurrentMonitorState => 0;

    public event Action OnGuestDataChanged;

    // 示例：监控侧有变化时调用 NotifyChanged();
    public void NotifyChanged()
    {
        OnGuestDataChanged?.Invoke();
    }
}
