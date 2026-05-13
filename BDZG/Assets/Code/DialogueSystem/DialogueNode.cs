using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueNode", menuName = "Dialogue/DialogueNode")]
public class DialogueNode : ScriptableObject
{
    public string speakerName;

    [Tooltip("立绘查表用角色 ID；留空则使用 speakerName")]
    public string characterId;

    [TextArea(3, 5)] public string dialogueText;
    public List<DialogueOption> options;
    public DialogueNode nextNode; // 线性跳转（无选项时使用）
}

[System.Serializable]
public class DialogueOption
{
    [TextArea(2, 4)]
    public string optionText;

    [Tooltip("此选项专用按钮预制体；留空则使用 DialogueUI 的默认选项按钮")]
    public GameObject optionButtonPrefab;

    public DialogueNode targetNode;
    // 可扩展条件变量（比如需要某物品）
    // public string requiredItemID;
}
