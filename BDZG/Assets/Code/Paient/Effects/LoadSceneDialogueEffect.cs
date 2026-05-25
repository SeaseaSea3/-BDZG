using UnityEngine;

public enum SceneTransitionTiming
{
    Immediate,
    OnDialogueEnd
}

[CreateAssetMenu(fileName = "LoadSceneEffect", menuName = "Dialogue/Effects/Load Scene")]
public class LoadSceneDialogueEffect : DialogueEffect
{
    [Tooltip("Build Settings 中的场景名；留空则只打日志不加载")]
    public string sceneName;

    public SceneTransitionTiming timing = SceneTransitionTiming.OnDialogueEnd;

    public override void Apply(DialogueSessionContext context, DialogueEffectServices services)
    {
        if (services?.Scene == null)
        {
            Debug.LogWarning($"[LoadSceneDialogueEffect] 无 ISceneDialogueActions，scene={sceneName}");
            return;
        }

        if (timing == SceneTransitionTiming.Immediate)
            services.Scene.LoadSceneNow(sceneName);
        else
            services.Scene.QueueSceneAfterDialogue(sceneName, context);
    }
}
