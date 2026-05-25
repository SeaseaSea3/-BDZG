using UnityEngine;

[CreateAssetMenu(fileName = "SetStoryFlagEffect", menuName = "Dialogue/Effects/Set Story Flag")]
public class SetStoryFlagDialogueEffect : DialogueEffect
{
    public string flagId = "flag";
    public int value = 1;

    public override void Apply(DialogueSessionContext context, DialogueEffectServices services)
    {
        services?.Narrative?.SetStoryFlag(flagId, value);
    }
}
