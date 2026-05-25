using UnityEngine;

/// <summary>选项/节点效果资源基类。Create 子类后在 DialogueOption.onSelect 里引用。</summary>
public abstract class DialogueEffect : ScriptableObject
{
    public abstract void Apply(DialogueSessionContext context, DialogueEffectServices services);
}
