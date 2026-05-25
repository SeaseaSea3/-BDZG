/// <summary>选项/节点效果的通道。PolicePatrol 仅在联系人入口对话中执行。</summary>
public enum DialogueEffectChannel
{
    /// <summary>警察巡逻 / 监控侧，由外部程序实现 IPolicePatrolDialogueActions。</summary>
    PolicePatrol,

    /// <summary>结局、GameOver、剧情变量等。</summary>
    Narrative,

    /// <summary>切换场景（可立即或对话结束后）。</summary>
    Scene
}
