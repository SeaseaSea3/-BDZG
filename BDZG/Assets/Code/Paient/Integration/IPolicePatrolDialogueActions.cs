/// <summary>
/// 警察巡逻 / 监控对话效果接入点。由另一套程序（巡逻、监控逻辑）实现并挂到场景；
/// BB 机联系人对话里选项的 PolicePatrol 通道会调用此接口。
/// </summary>
public interface IPolicePatrolDialogueActions
{
    /// <summary>联系人对话中玩家确认某选项时触发。</summary>
    /// <param name="commandId">策划在 DialogueEffect 资源里配置的指令 ID。</param>
    /// <param name="intValue">可选数值参数。</param>
    /// <param name="context">当前会话（含联系人信息）。</param>
    void OnContactDialogueOption(string commandId, int intValue, DialogueSessionContext context);
}
