using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private DialogueNode currentNode;
    private DialogueSessionContext sessionContext;

    /// <summary>是否有对话正在进行（含等待玩家点选选项）。</summary>
    public bool IsDialogueActive => currentNode != null;

    public DialogueSessionContext CurrentSession => sessionContext;

    public event Action<DialogueNode> OnDialogueStarted;
    public event Action<DialogueNode> OnDialogueUpdated;
    public event Action<List<DialogueOption>> OnOptionsReady;
    public event Action OnDialogueEnded;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartDialogue(DialogueNode startNode)
    {
        StartDialogue(startNode, new DialogueSessionContext());
    }

    public void StartDialogue(DialogueNode startNode, DialogueSessionContext context)
    {
        sessionContext = context ?? new DialogueSessionContext();
        currentNode = startNode;
        OnDialogueStarted?.Invoke(currentNode);
        ShowCurrentNode();
    }

    public void StopDialogue()
    {
        if (currentNode == null)
            return;
        EndDialogue();
    }

    private void ShowCurrentNode()
    {
        if (currentNode == null)
        {
            EndDialogue();
            return;
        }

        DialogueEffectRouter.ApplyNodeEnterEffects(currentNode, sessionContext);

        OnDialogueUpdated?.Invoke(currentNode);

        if (currentNode.options != null && currentNode.options.Count > 0)
            OnOptionsReady?.Invoke(currentNode.options);
    }

    public void Advance()
    {
        if (currentNode == null) return;
        if ((currentNode.options == null || currentNode.options.Count == 0) && currentNode.nextNode != null)
        {
            currentNode = currentNode.nextNode;
            ShowCurrentNode();
        }
        else if (currentNode.options != null && currentNode.options.Count > 0)
        {
            // 有选项，等待玩家选择
        }
        else
        {
            EndDialogue();
        }
    }

    public void SelectOption(int optionIndex)
    {
        if (currentNode == null || currentNode.options == null || optionIndex >= currentNode.options.Count)
            return;

        var option = currentNode.options[optionIndex];
        DialogueEffectRouter.ApplyOptionEffects(option, sessionContext, currentNode, optionIndex);

        var target = option.targetNode;
        if (target != null)
        {
            currentNode = target;
            ShowCurrentNode();
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        DialogueEffectRouter.FlushAfterDialogue(sessionContext);
        currentNode = null;
        sessionContext = null;
        OnDialogueEnded?.Invoke();
    }
}
