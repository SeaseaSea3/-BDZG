using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 默认场景效果：立即加载或对话结束后加载。sceneName 为空时只打日志，便于策划先配资源。
/// </summary>
public class SceneDialogueActionsDefault : MonoBehaviour, ISceneDialogueActions
{
    [Tooltip("为 false 时只 Log，不真正 LoadScene（防止误配空场景名）")]
    [SerializeField] private bool allowRealLoad = true;

    public void LoadSceneNow(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[SceneDialogue] LoadSceneNow: sceneName 为空。");
            return;
        }

        if (!allowRealLoad)
        {
            Debug.Log($"[SceneDialogue] LoadSceneNow (dry-run): {sceneName}");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void QueueSceneAfterDialogue(string sceneName, DialogueSessionContext context)
    {
        if (context == null)
            return;

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[SceneDialogue] QueueSceneAfterDialogue: sceneName 为空。");
            return;
        }

        context.PendingSceneName = sceneName;
        Debug.Log($"[SceneDialogue] 已排队，对话结束后加载: {sceneName}");
    }

    public void FlushPendingScene(DialogueSessionContext context)
    {
        if (context == null || !context.HasPendingScene)
            return;

        string name = context.PendingSceneName;
        context.ClearPendingScene();
        LoadSceneNow(name);
    }
}
