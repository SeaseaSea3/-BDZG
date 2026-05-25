/// <summary>场景切换。LoadSceneDialogueEffect 调用；未配置场景名时仅记录。</summary>
public interface ISceneDialogueActions
{
    void LoadSceneNow(string sceneName);
    void QueueSceneAfterDialogue(string sceneName, DialogueSessionContext context);
    void FlushPendingScene(DialogueSessionContext context);
}
