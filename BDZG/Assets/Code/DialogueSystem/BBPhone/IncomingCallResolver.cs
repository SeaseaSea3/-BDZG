using UnityEngine;

/// <summary>
/// 根据监控接口与 ScriptableObject 配置解析来电起点。不在运行时生成任何资源。
/// </summary>
public class IncomingCallResolver : MonoBehaviour
{
    [SerializeField] private MonoBehaviour guestProviderComponent;
    [SerializeField] private GeneralIncomingPool generalPool;
    [SerializeField] private MonitorIncomingConfig monitorConfig;

    [Range(0f, 1f)]
    [Tooltip("随机判定为「监控相关来电」的概率；匹配失败时回退到 General Pool")]
    [SerializeField] private float monitorCallChance = 0.35f;

    [Tooltip("监控相关判定失败时是否仍强制尝试 Monitor Config 匹配")]
    [SerializeField] private bool alwaysTryMonitorMatchFirst;

    private IMonitorGuestProvider _provider;

    private void Awake()
    {
        ResolveProviderReference();
    }

    private void ResolveProviderReference()
    {
        if (guestProviderComponent is IMonitorGuestProvider p)
            _provider = p;
        else if (guestProviderComponent != null)
            Debug.LogWarning("[IncomingCallResolver] guestProviderComponent 未实现 IMonitorGuestProvider。");
    }

    public void SetGuestProvider(IMonitorGuestProvider provider)
    {
        _provider = provider;
    }

    public struct IncomingCallPick
    {
        public DialogueNode Node;
        public DialogueEntrySource Source;
    }

    public DialogueNode PickIncomingStartNode()
    {
        return PickIncoming().Node;
    }

    public IncomingCallPick PickIncoming()
    {
        if (_provider == null)
            ResolveProviderReference();

        bool tryMonitor = alwaysTryMonitorMatchFirst || Random.value < monitorCallChance;

        if (tryMonitor && monitorConfig != null && _provider != null)
        {
            var monitorNode = monitorConfig.Resolve(_provider);
            if (monitorNode != null)
                return new IncomingCallPick
                {
                    Node = monitorNode,
                    Source = DialogueEntrySource.IncomingMonitor
                };
        }

        if (generalPool != null)
        {
            var general = generalPool.PickRandom();
            if (general != null)
                return new IncomingCallPick
                {
                    Node = general,
                    Source = DialogueEntrySource.IncomingGeneral
                };
        }

        if (!tryMonitor && monitorConfig != null && _provider != null)
        {
            var fallback = monitorConfig.Resolve(_provider);
            if (fallback != null)
                return new IncomingCallPick
                {
                    Node = fallback,
                    Source = DialogueEntrySource.IncomingMonitor
                };
        }

        return default;
    }
}
