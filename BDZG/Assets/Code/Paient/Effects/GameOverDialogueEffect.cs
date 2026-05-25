using UnityEngine;

[CreateAssetMenu(fileName = "GameOverEffect", menuName = "Dialogue/Effects/Game Over")]
public class GameOverDialogueEffect : DialogueEffect
{
    [Tooltip("结局标识，会写入 gameover 日志")]
    public string endingId = "default";

    public override void Apply(DialogueSessionContext context, DialogueEffectServices services)
    {
        if (services?.Narrative == null)
        {
            Debug.Log($"gameover ({endingId})");
            return;
        }

        services.Narrative.TriggerGameOver(endingId);
    }
}
