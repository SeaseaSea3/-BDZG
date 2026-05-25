using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 对外调用入口：由按钮或其它脚本调用 Begin，对话正常结束后触发结束回调。
/// 挂到任意常驻物体上，在 Inspector 里把按钮 OnClick 指向 Begin() 或带默认起点节点。
/// </summary>
public class DialogueLauncher : MonoBehaviour
{
    [Tooltip("供无参 Begin() 使用；也可在代码里传入其它节点")]
    [SerializeField] private DialogueNode defaultStartNode;

    /// <summary>无参 Begin() 使用的起点；可在运行时赋值。</summary>
    public DialogueNode DefaultStartNode
    {
        get => defaultStartNode;
        set => defaultStartNode = value;
    }

    [SerializeField] private UnityEvent onDialogueBegan;
    [SerializeField] private UnityEvent onDialogueEnded;

    private bool _awaitingOurSession;

    /// <summary>由本 Launcher 发起且尚未收到结束回调时为 true。</summary>
    public bool IsSessionPending => _awaitingOurSession;

    /// <summary>对话完全结束（与 UnityEvent 同时触发）。</summary>
    public event Action DialogueEnded;

    /// <summary>对话已开始展示第一句（与 UnityEvent 同时触发）。</summary>
    public event Action DialogueBegan;

    /// <summary>使用 Inspector 中配置的 defaultStartNode 开始对话。</summary>
    public void Begin()
    {
        Begin(defaultStartNode);
    }

    /// <summary>从指定节点开始一段对话。</summary>
    public void Begin(DialogueNode startNode)
    {
        if (startNode == null)
        {
            Debug.LogError("[DialogueLauncher] startNode 为空。");
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[DialogueLauncher] 场景中缺少 DialogueManager。");
            return;
        }

        if (_awaitingOurSession)
        {
            Debug.LogWarning("[DialogueLauncher] 上一轮尚未结束，忽略本次 Begin。");
            return;
        }

        if (DialogueManager.Instance.IsDialogueActive)
        {
            Debug.LogWarning("[DialogueLauncher] 已有对话在进行，忽略本次 Begin。");
            return;
        }

        _awaitingOurSession = true;
        DialogueManager.Instance.OnDialogueEnded += OnManagerDialogueEnded;

        DialogueBegan?.Invoke();
        onDialogueBegan?.Invoke();

        DialogueManager.Instance.StartDialogue(startNode, DialogueSessionContext.ForLauncher());
    }

    /// <summary>打断当前对话（若由本 Launcher 开启，仍会收到一次结束回调）。</summary>
    public void Stop()
    {
        if (DialogueManager.Instance == null)
            return;
        DialogueManager.Instance.StopDialogue();
    }

    private void OnManagerDialogueEnded()
    {
        if (!_awaitingOurSession)
            return;

        _awaitingOurSession = false;
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.OnDialogueEnded -= OnManagerDialogueEnded;

        onDialogueEnded?.Invoke();
        DialogueEnded?.Invoke();
    }

    private void OnDestroy()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.OnDialogueEnded -= OnManagerDialogueEnded;
        _awaitingOurSession = false;
    }
}
