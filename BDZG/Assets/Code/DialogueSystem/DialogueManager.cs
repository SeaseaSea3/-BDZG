using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private DialogueNode currentNode;

    /// <summary>�Ƿ��жԻ����ڽ��У����ȴ���ҵ������ѡ���</summary>
    public bool IsDialogueActive => currentNode != null;

    // �¼����� UI ������������ʾ
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
        currentNode = startNode;
        OnDialogueStarted?.Invoke(currentNode);
        ShowCurrentNode();
    }

    /// <summary>����������ǰ�Ի������� OnDialogueEnded��������ǰ�жԻ�ʱ����</summary>
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

        OnDialogueUpdated?.Invoke(currentNode);

        if (currentNode.options != null && currentNode.options.Count > 0)
        {
            OnOptionsReady?.Invoke(currentNode.options);
        }
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
            // ����ʾѡ��ȴ����ѡ��
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

        var target = currentNode.options[optionIndex].targetNode;
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
        currentNode = null;
        OnDialogueEnded?.Invoke();
    }
}
