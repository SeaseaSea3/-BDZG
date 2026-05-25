using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// BB 机对话界面：正文与选项合并到同一 TMP；按钮由 BBPhoneController 统一分发。
/// </summary>
public class BBDialogueView : MonoBehaviour
{
    [SerializeField] private BBPhoneUITheme theme;

    [Header("对话 Panel（进对话时显示）")]
    [SerializeField] private GameObject dialoguePanelRoot;

    [Header("文本（场景中已有 TMP）")]
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueLineText;

    [Header("显示方式")]
    [Tooltip("开启后选项追加在同一段 dialogueLineText 里。")]
    [SerializeField] private bool combineOptionsInDialogueText = true;

    [Header("独立选项槽（combineOptionsInDialogueText 关闭时使用）")]
    [SerializeField] private GameObject optionsRowRoot;
    [SerializeField] private TMP_Text optionText0;
    [SerializeField] private TMP_Text optionText1;
    [SerializeField] private SpriteStateImage optionBackground0;
    [SerializeField] private SpriteStateImage optionBackground1;

    [Header("立绘（可选）")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private CharacterPortraitDatabase portraitDatabase;

    private List<DialogueOption> _currentOptions;
    private int _selectedOptionIndex;
    private bool _hasOptions;
    private DialogueNode _currentNode;

    public bool IsConfigured => dialogueLineText != null;

    private Transform BindRoot => dialoguePanelRoot != null ? dialoguePanelRoot.transform : transform;

    private void Awake()
    {
        TryAutoBindDisplayFields();
    }

    private void Start()
    {
        Hide();
        SubscribeDialogueEvents();
    }

    private void OnEnable()
    {
        SubscribeDialogueEvents();
    }

    public void Show()
    {
        TryAutoBindDisplayFields();
        SetDialoguePanelActive(true);
    }

    public void Hide()
    {
        SetDialoguePanelActive(false);
    }

    private void SetDialoguePanelActive(bool active)
    {
        if (dialoguePanelRoot != null)
            dialoguePanelRoot.SetActive(active);
        else
            gameObject.SetActive(active);
    }

    private void SubscribeDialogueEvents()
    {
        if (DialogueManager.Instance == null)
            return;

        DialogueManager.Instance.OnDialogueStarted -= OnDialogueStarted;
        DialogueManager.Instance.OnDialogueUpdated -= OnDialogueUpdated;
        DialogueManager.Instance.OnOptionsReady -= OnOptionsReady;
        DialogueManager.Instance.OnDialogueEnded -= OnDialogueEnded;

        DialogueManager.Instance.OnDialogueStarted += OnDialogueStarted;
        DialogueManager.Instance.OnDialogueUpdated += OnDialogueUpdated;
        DialogueManager.Instance.OnOptionsReady += OnOptionsReady;
        DialogueManager.Instance.OnDialogueEnded += OnDialogueEnded;
    }

    private void OnDialogueStarted(DialogueNode node)
    {
        Show();
    }

    private void OnDialogueUpdated(DialogueNode node)
    {
        _currentNode = node;

        if (speakerNameText != null)
            speakerNameText.text = node.speakerName ?? string.Empty;

        ApplyPortrait(node);

        _hasOptions = node.options != null && node.options.Count > 0;
        _currentOptions = _hasOptions ? node.options : null;
        _selectedOptionIndex = 0;

        RefreshDialogueDisplay();
    }

    private void OnOptionsReady(List<DialogueOption> options)
    {
        _currentOptions = options;
        _selectedOptionIndex = 0;
        _hasOptions = options != null && options.Count > 0;
        RefreshDialogueDisplay();
    }

    public void NavigateUp()
    {
        if (!_hasOptions || _currentOptions == null || _currentOptions.Count <= 1)
            return;
        _selectedOptionIndex = 0;
        RefreshDialogueDisplay();
    }

    public void NavigateDown()
    {
        if (!_hasOptions || _currentOptions == null || _currentOptions.Count <= 1)
            return;
        _selectedOptionIndex = 1;
        RefreshDialogueDisplay();
    }

    public void ConfirmChoice()
    {
        if (DialogueManager.Instance == null)
            return;

        if (_hasOptions && _currentOptions != null && _selectedOptionIndex < _currentOptions.Count)
            DialogueManager.Instance.SelectOption(_selectedOptionIndex);
        else
            DialogueManager.Instance.Advance();
    }

    private void RefreshDialogueDisplay()
    {
        if (combineOptionsInDialogueText)
        {
            if (optionsRowRoot != null && optionsRowRoot != dialoguePanelRoot)
                optionsRowRoot.SetActive(false);

            RefreshCombinedDialogueText();
            return;
        }

        if (_currentNode != null && dialogueLineText != null)
            dialogueLineText.text = _currentNode.dialogueText ?? string.Empty;

        if (optionsRowRoot != null)
            optionsRowRoot.SetActive(_hasOptions);

        if (!_hasOptions)
        {
            ClearOptionSlots();
            return;
        }

        if (_currentOptions != null && _currentOptions.Count > 2)
            Debug.LogWarning("[BBDialogueView] 当前节点选项超过 2 条，仅显示前 2 条。");

        RefreshOptionSlots();
    }

    private void RefreshCombinedDialogueText()
    {
        if (dialogueLineText == null)
            return;

        var sb = new StringBuilder();
        string body = _currentNode != null ? _currentNode.dialogueText : string.Empty;
        if (!string.IsNullOrEmpty(body))
            sb.Append(body);

        if (_hasOptions && _currentOptions != null)
        {
            if (sb.Length > 0)
                sb.AppendLine().AppendLine();

            int count = Mathf.Min(_currentOptions.Count, 2);
            for (int i = 0; i < count; i++)
            {
                string prefix = i == _selectedOptionIndex ? "> " : "  ";
                sb.AppendLine(prefix + _currentOptions[i].optionText);
            }
        }

        dialogueLineText.text = sb.ToString();
    }

    private void RefreshOptionSlots()
    {
        ApplyOptionSlot(optionText0, optionBackground0, 0);
        ApplyOptionSlot(optionText1, optionBackground1, 1);
    }

    private void ApplyOptionSlot(TMP_Text label, SpriteStateImage bg, int optionIndex)
    {
        bool hasEntry = optionIndex >= 0 && _currentOptions != null && optionIndex < _currentOptions.Count;
        if (label != null)
        {
            label.gameObject.SetActive(hasEntry);
            if (hasEntry)
                label.text = _currentOptions[optionIndex].optionText;
        }

        if (bg != null)
        {
            bg.gameObject.SetActive(hasEntry);
            if (hasEntry && theme != null)
                bg.SetSprites(theme.optionSlotNormal, theme.optionSlotSelected);
            if (hasEntry)
                bg.SetSelected(optionIndex == _selectedOptionIndex);
        }
    }

    private void ClearOptionSlots()
    {
        _currentOptions = null;
        ApplyOptionSlot(optionText0, optionBackground0, -1);
        ApplyOptionSlot(optionText1, optionBackground1, -1);
    }

    private void ApplyPortrait(DialogueNode node)
    {
        if (portraitImage == null)
            return;
        string key = string.IsNullOrWhiteSpace(node.characterId) ? node.speakerName : node.characterId;
        Sprite sprite = portraitDatabase != null ? portraitDatabase.GetPortrait(key) : null;
        portraitImage.sprite = sprite;
        portraitImage.enabled = sprite != null;
    }

    private void OnDialogueEnded()
    {
        Hide();
        ClearOptionSlots();
        _currentNode = null;
    }

    private void TryAutoBindDisplayFields()
    {
        var root = BindRoot;
        if (root == null)
            return;

        foreach (var tmp in root.GetComponentsInChildren<TMP_Text>(true))
        {
            if (tmp.GetComponentInParent<Button>() != null)
                continue;

            string n = tmp.gameObject.name.ToLowerInvariant();
            if (speakerNameText == null && (n.Contains("speaker") || (n.Contains("name") && !n.Contains("contact"))))
                speakerNameText = tmp;
            else if (dialogueLineText == null && (n.Contains("dialogue") || n.Contains("line") || n.Contains("content")))
                dialogueLineText = tmp;
        }

        if (dialogueLineText == null)
        {
            foreach (var tmp in root.GetComponentsInChildren<TMP_Text>(true))
            {
                if (tmp.GetComponentInParent<Button>() != null || tmp == speakerNameText)
                    continue;
                dialogueLineText = tmp;
                break;
            }
        }
    }

    private void OnDestroy()
    {
        if (DialogueManager.Instance == null)
            return;
        DialogueManager.Instance.OnDialogueStarted -= OnDialogueStarted;
        DialogueManager.Instance.OnDialogueUpdated -= OnDialogueUpdated;
        DialogueManager.Instance.OnOptionsReady -= OnOptionsReady;
        DialogueManager.Instance.OnDialogueEnded -= OnDialogueEnded;
    }
}
