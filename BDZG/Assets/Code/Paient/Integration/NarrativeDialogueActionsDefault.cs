using UnityEngine;

/// <summary>默认叙事效果：GameOver 打日志，便于后续替换为真实结局流程。</summary>
public class NarrativeDialogueActionsDefault : MonoBehaviour, INarrativeDialogueActions
{
    public void TriggerGameOver(string endingId)
    {
        string id = string.IsNullOrEmpty(endingId) ? "default" : endingId;
        Debug.Log($"gameover ({id})");
    }

    public void SetStoryFlag(string flagId, int value)
    {
        Debug.Log($"[Narrative] SetStoryFlag {flagId}={value}");
    }
}
