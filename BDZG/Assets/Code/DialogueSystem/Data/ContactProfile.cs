using UnityEngine;

[CreateAssetMenu(fileName = "ContactProfile", menuName = "BBPhone/Contact Profile")]
public class ContactProfile : ScriptableObject
{
    [Tooltip("BB 机列表里显示的名字")]
    public string displayName;

    [Tooltip("联系人住所，在 ContactListView 的 TMP 上显示")]
    public string address;

    [Tooltip("联系人电话号码，在 ContactListView 的 TMP 上显示")]
    public string phoneNumber;

    [Tooltip("列表头像，在 ContactListView 的 Image 上显示；自行指定 Sprite")]
    public Sprite listIcon;

    [Tooltip("主动拨号时使用的对话根节点，拖入 DialogueNode 资源")]
    public DialogueNode defaultStartNode;
}
