using UnityEngine;

[CreateAssetMenu(fileName = "BBPhoneUITheme", menuName = "BBPhone/UI Theme")]
public class BBPhoneUITheme : ScriptableObject
{
    [Header("对话选项槽背景（仅换 Sprite，不生成 UI）")]
    public Sprite optionSlotNormal;
    public Sprite optionSlotSelected;

    [Header("可选：联系人列表选中高亮背景")]
    public Sprite contactEntryNormal;
    public Sprite contactEntrySelected;
}
