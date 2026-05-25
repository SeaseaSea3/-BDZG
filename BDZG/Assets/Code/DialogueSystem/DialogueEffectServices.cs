using UnityEngine;

/// <summary>聚合各通道的实现，挂在与 DialogueManager 同场景即可。</summary>
public class DialogueEffectServices : MonoBehaviour
{
    public static DialogueEffectServices Instance { get; private set; }

    [SerializeField] private MonoBehaviour policePatrolComponent;
    [SerializeField] private MonoBehaviour narrativeComponent;
    [SerializeField] private MonoBehaviour sceneComponent;

    public IPolicePatrolDialogueActions PolicePatrol { get; private set; }
    public INarrativeDialogueActions Narrative { get; private set; }
    public ISceneDialogueActions Scene { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[DialogueEffectServices] 场景中存在多个实例，保留先创建的。");
            return;
        }

        Instance = this;
        ResolveReferences();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void ResolveReferences()
    {
        PolicePatrol = policePatrolComponent as IPolicePatrolDialogueActions;
        if (policePatrolComponent != null && PolicePatrol == null)
            Debug.LogWarning("[DialogueEffectServices] policePatrolComponent 未实现 IPolicePatrolDialogueActions。");

        Narrative = narrativeComponent as INarrativeDialogueActions;
        if (narrativeComponent != null && Narrative == null)
            Debug.LogWarning("[DialogueEffectServices] narrativeComponent 未实现 INarrativeDialogueActions。");

        Scene = sceneComponent as ISceneDialogueActions;
        if (sceneComponent != null && Scene == null)
            Debug.LogWarning("[DialogueEffectServices] sceneComponent 未实现 ISceneDialogueActions。");

        if (Narrative == null)
        {
            var fallback = GetComponent<NarrativeDialogueActionsDefault>();
            if (fallback != null)
                Narrative = fallback;
        }

        if (Scene == null)
        {
            var fallback = GetComponent<SceneDialogueActionsDefault>();
            if (fallback != null)
                Scene = fallback;
        }
    }
}
