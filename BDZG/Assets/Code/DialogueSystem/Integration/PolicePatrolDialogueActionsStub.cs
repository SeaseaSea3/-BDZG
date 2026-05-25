using UnityEngine;

/// <summary>
/// 占位：巡逻程序未接入前仅打日志。接入后从场景中移除此组件，改为你们的实现类。
/// </summary>
public class PolicePatrolDialogueActionsStub : MonoBehaviour, IPolicePatrolDialogueActions
{
    public void OnContactDialogueOption(string commandId, int intValue, DialogueSessionContext context)
    {
        string contact = context?.Contact != null ? context.Contact.displayName : "?";
        Debug.Log($"[PolicePatrolStub] command={commandId} value={intValue} contact={contact}");
    }
}
