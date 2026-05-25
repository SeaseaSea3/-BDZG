using System;

/// <summary>
/// 监控系统对外数据接口。BB 机来电解析只依赖此接口，不直接引用监控场景脚本。
/// 监控同事实现后，替换场景中的 Stub 组件即可。
/// </summary>
public interface IMonitorGuestProvider
{
    int TotalGuestCount { get; }
    int SpecialGuestCount { get; }
    /// <summary>当前监控路数 StateIndex，无监控时可恒为 0。</summary>
    int CurrentMonitorState { get; }

    event Action OnGuestDataChanged;
}
