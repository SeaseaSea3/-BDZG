using UnityEngine;

[CreateAssetMenu(fileName = "PolicePatrolEffect", menuName = "Dialogue/Effects/Police Patrol")]
public class PolicePatrolDialogueEffect : DialogueEffect
{
    [Tooltip("交给 IPolicePatrolDialogueActions 的指令 ID")]
    public string commandId = "default";

    public int intValue;

    public override void Apply(DialogueSessionContext context, DialogueEffectServices services)
    {
        if (services?.PolicePatrol == null)
        {
            Debug.LogWarning("[PolicePatrolDialogueEffect] 场景中未找到 IPolicePatrolDialogueActions 实现。");
            return;
        }

        services.PolicePatrol.OnContactDialogueOption(commandId, intValue, context);
    }
}
