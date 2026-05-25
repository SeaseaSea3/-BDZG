/// <summary>结局 / GameOver / 剧情标记。对话选项 Narrative 通道调用。</summary>
public interface INarrativeDialogueActions
{
    void TriggerGameOver(string endingId);
    void SetStoryFlag(string flagId, int value);
}
