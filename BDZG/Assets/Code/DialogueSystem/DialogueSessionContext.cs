using UnityEngine;

/// <summary>本次对话的入口来源，用于过滤警察巡逻（联系人）类效果。</summary>
public enum DialogueEntrySource
{
    Unknown,
    Contact,
    IncomingGeneral,
    IncomingMonitor,
    Launcher
}

/// <summary>当前对话会话上下文，由 BBPhone / Launcher 在 StartDialogue 时传入。</summary>
public class DialogueSessionContext
{
    public DialogueEntrySource Source = DialogueEntrySource.Unknown;
    public ContactProfile Contact;
    public int ContactIndex = -1;

    /// <summary>对话结束后待加载的场景名（由 Scene 通道效果写入）。</summary>
    public string PendingSceneName;

    public bool HasPendingScene => !string.IsNullOrEmpty(PendingSceneName);

    public void ClearPendingScene()
    {
        PendingSceneName = null;
    }

    public static DialogueSessionContext ForContact(ContactProfile profile, int index)
    {
        return new DialogueSessionContext
        {
            Source = DialogueEntrySource.Contact,
            Contact = profile,
            ContactIndex = index
        };
    }

    public static DialogueSessionContext ForIncoming(DialogueEntrySource source)
    {
        return new DialogueSessionContext
        {
            Source = source == DialogueEntrySource.IncomingMonitor
                ? DialogueEntrySource.IncomingMonitor
                : DialogueEntrySource.IncomingGeneral
        };
    }

    public static DialogueSessionContext ForLauncher()
    {
        return new DialogueSessionContext { Source = DialogueEntrySource.Launcher };
    }
}
