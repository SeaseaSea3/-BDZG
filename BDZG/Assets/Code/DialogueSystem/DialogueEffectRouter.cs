using System.Collections.Generic;
using UnityEngine;

/// <summary>按通道与会话来源执行 DialogueOption / DialogueNode 上的效果列表。</summary>
public static class DialogueEffectRouter
{
    public static void ApplyOptionEffects(
        DialogueOption option,
        DialogueSessionContext context,
        DialogueNode fromNode,
        int optionIndex)
    {
        if (option == null || option.onSelect == null || option.onSelect.Count == 0)
            return;

        ApplyBindings(option.onSelect, context, $"option[{optionIndex}]");
    }

    public static void ApplyNodeEnterEffects(DialogueNode node, DialogueSessionContext context)
    {
        if (node == null || node.onEnter == null || node.onEnter.Count == 0)
            return;

        ApplyBindings(node.onEnter, context, $"node:{node.name}");
    }

    public static void FlushAfterDialogue(DialogueSessionContext context)
    {
        var services = DialogueEffectServices.Instance;
        services?.Scene?.FlushPendingScene(context);
    }

    private static void ApplyBindings(
        List<DialogueEffectBinding> bindings,
        DialogueSessionContext context,
        string debugLabel)
    {
        var services = DialogueEffectServices.Instance;
        if (services == null)
        {
            Debug.LogWarning($"[DialogueEffectRouter] 缺少 DialogueEffectServices，跳过 {debugLabel}。");
            return;
        }

        context ??= new DialogueSessionContext();

        foreach (var binding in bindings)
        {
            if (binding == null || binding.effect == null)
                continue;

            if (!ShouldRunChannel(binding.channel, context))
                continue;

            binding.effect.Apply(context, services);
        }
    }

    private static bool ShouldRunChannel(DialogueEffectChannel channel, DialogueSessionContext context)
    {
        if (channel == DialogueEffectChannel.PolicePatrol)
            return context.Source == DialogueEntrySource.Contact;

        return true;
    }
}
