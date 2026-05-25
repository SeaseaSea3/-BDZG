using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogText;

    [Header("立绘（与角色 ID 绑定）")]
    public Image portraitImage;
    public CharacterPortraitDatabase portraitDatabase;

    public Transform optionsParent;
    [Tooltip("当选项未在 DialogueOption 上指定 optionButtonPrefab 时使用")]
    public GameObject optionButtonPrefab;
    public Button continueButton;

    private List<GameObject> currentOptionButtons = new List<GameObject>();

    private void Start()
    {
        gameObject.SetActive(false);
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
        else
            Debug.LogWarning("[DialogueUI] Continue Button 未绑定，无选项节点将无法继续。");

        if (DialogueManager.Instance == null)
            return;

        DialogueManager.Instance.OnDialogueStarted += OnDialogueStarted;
        DialogueManager.Instance.OnDialogueUpdated += OnDialogueUpdated;
        DialogueManager.Instance.OnOptionsReady += OnOptionsReady;
        DialogueManager.Instance.OnDialogueEnded += OnDialogueEnded;
    }

    private void OnDialogueStarted(DialogueNode node)
    {
        gameObject.SetActive(true);
        ClearOptions();
        if (continueButton != null)
            continueButton.gameObject.SetActive(false);
    }

    private void OnDialogueUpdated(DialogueNode node)
    {
        if (speakerNameText != null)
            speakerNameText.text = node.speakerName;
        if (dialogText != null)
            dialogText.text = node.dialogueText;
        ApplyPortrait(node);

        ClearOptions();
        if (continueButton == null)
            return;

        if (node.options == null || node.options.Count == 0)
            continueButton.gameObject.SetActive(true);
        else
            continueButton.gameObject.SetActive(false);
    }

    private void OnOptionsReady(List<DialogueOption> options)
    {
        ClearOptions();
        foreach (var opt in options)
        {
            GameObject prefab = opt.optionButtonPrefab != null ? opt.optionButtonPrefab : optionButtonPrefab;
            if (prefab == null)
            {
                Debug.LogError("选项缺少按钮预制体，且 DialogueUI 未设置默认 optionButtonPrefab。");
                continue;
            }

            GameObject btnObj = Instantiate(prefab, optionsParent);
            var btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
                btnText.text = opt.optionText;
            else
                Debug.LogWarning($"选项按钮预制体 {prefab.name} 下未找到 TextMeshProUGUI，无法写入 optionText。");

            var btn = btnObj.GetComponentInChildren<Button>();
            if (btn == null)
            {
                Debug.LogError($"选项按钮预制体 {prefab.name} 上未找到 Button。");
                Destroy(btnObj);
                continue;
            }

            int idx = currentOptionButtons.Count;
            btn.onClick.AddListener(() => DialogueManager.Instance.SelectOption(idx));
            currentOptionButtons.Add(btnObj);
        }
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

    private void OnContinueClicked()
    {
        DialogueManager.Instance.Advance();
    }

    private void ClearOptions()
    {
        foreach (var btn in currentOptionButtons)
            Destroy(btn);
        currentOptionButtons.Clear();
    }

    private void OnDialogueEnded()
    {
        gameObject.SetActive(false);
        ClearOptions();
    }

    private void OnDestroy()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueStarted -= OnDialogueStarted;
            DialogueManager.Instance.OnDialogueUpdated -= OnDialogueUpdated;
            DialogueManager.Instance.OnOptionsReady -= OnOptionsReady;
            DialogueManager.Instance.OnDialogueEnded -= OnDialogueEnded;
        }
    }
}
