using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BBPhoneState
{
    Idle,
    Ringing,
    ContactList,
    Dialogue
}

/// <summary>
/// BB 机总控。BB 机面板与 Ring 分开开关：打开 BB 机（Tab/对话/接听）后隐藏 Ring，直至下次 TriggerIncomingCall 响铃再显示。
/// </summary>
public class BBPhoneController : MonoBehaviour
{
    [Header("数据")]
    [SerializeField] private ContactDatabase contactDatabase;
    [SerializeField] private IncomingCallResolver incomingCallResolver;

    [Header("子视图")]
    [Tooltip("仅 BB 机面板（联系人、对话、Up/Down/Choose/Exit）。不要拖整颗 Canvas，不要包含 Ring。")]
    [SerializeField] private GameObject bbMachineRoot;

    [Tooltip("响铃按钮根物体。来电响铃时显示；打开 BB 机后隐藏，直至下次来电。")]
    [SerializeField] private GameObject ringButtonRoot;

    [SerializeField] private ContactListView contactListView;
    [SerializeField] private BBDialogueView bbDialogueView;
    [SerializeField] private RingButtonView ringButtonView;

    [Header("共用操作按钮（Contact / Dialogue 同一组，只在这里绑一次）")]
    [SerializeField] private UnityEngine.UI.Button btnUp;
    [SerializeField] private UnityEngine.UI.Button btnDown;
    [SerializeField] private UnityEngine.UI.Button btnChoose;
    [SerializeField] private UnityEngine.UI.Button btnExit;

    [Header("输入")]
    [SerializeField] private KeyCode openKey = KeyCode.Tab;
    [SerializeField] private KeyCode confirmKey = KeyCode.Return;
    [SerializeField] private KeyCode exitKey = KeyCode.Escape;

    [Header("若 BBDialogueView 未配置，可回退到旧版 DialogueUI")]
    [SerializeField] private DialogueUI legacyDialogueUI;

    private BBPhoneState _state = BBPhoneState.Idle;
    private DialogueNode _pendingIncomingNode;
    private DialogueEntrySource _pendingIncomingSource = DialogueEntrySource.IncomingGeneral;

    public BBPhoneState State => _state;

    private Transform RingHierarchyRoot
    {
        get
        {
            if (ringButtonRoot != null)
                return ringButtonRoot.transform;
            if (ringButtonView != null)
                return ringButtonView.transform;
            return null;
        }
    }

    private void Start()
    {
        if (legacyDialogueUI == null)
            legacyDialogueUI = FindObjectOfType<DialogueUI>();

        if (bbDialogueView != null && bbDialogueView.IsConfigured && legacyDialogueUI != null)
            legacyDialogueUI.enabled = false;

        ValidateHierarchy();
        SetBBMachineActive(false);
        ShowRingButton();

        if (contactListView != null)
        {
            contactListView.BindDatabase(contactDatabase);
            contactListView.OnConfirmContact += OnContactConfirmed;
        }

        WireSharedButtons();

        if (ringButtonView != null)
            ringButtonView.OnClicked += OnRingButtonClicked;

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.OnDialogueEnded += OnDialogueEnded;
    }

    private void ValidateHierarchy()
    {
        if (bbMachineRoot == null || ringButtonView == null)
            return;

        if (ringButtonView.transform.IsChildOf(bbMachineRoot.transform))
            Debug.LogWarning("[BBPhoneController] RingButtonView 不应放在 bbMachineRoot 子层级下，否则关闭 BB 机时会一并隐藏响铃按钮。");
    }

    private void SetBBMachineActive(bool active)
    {
        if (bbMachineRoot != null)
            bbMachineRoot.SetActive(active);
    }

    private void ShowRingButton()
    {
        if (ringButtonRoot != null)
            ringButtonRoot.SetActive(true);
        else if (ringButtonView != null)
            ringButtonView.gameObject.SetActive(true);
    }

    private void HideRingButton()
    {
        if (ringButtonRoot != null)
            ringButtonRoot.SetActive(false);
        else if (ringButtonView != null)
            ringButtonView.gameObject.SetActive(false);
    }

    private bool IsUnderRingHierarchy(Transform t)
    {
        Transform ringRoot = RingHierarchyRoot;
        if (ringRoot == null || t == null)
            return false;
        return t == ringRoot || t.IsChildOf(ringRoot);
    }

    private void Update()
    {
        if (Input.GetKeyDown(openKey))
            TryOpenOrAccept();

        if (_state == BBPhoneState.ContactList && Input.GetKeyDown(confirmKey))
            OnChoosePressed();

        if (Input.GetKeyDown(exitKey))
            CloseBBMachine();
    }

    private void WireSharedButtons()
    {
        TryAutoBindSharedButtons();
        PrepareSharedButtons();

        BindSharedButton(btnUp, OnUpPressed);
        BindSharedButton(btnDown, OnDownPressed);
        BindSharedButton(btnChoose, OnChoosePressed);
        BindSharedButton(btnExit, CloseBBMachine);
    }

    private void BindSharedButton(Button btn, UnityEngine.Events.UnityAction handler)
    {
        if (btn == null)
            return;
        btn.onClick.RemoveListener(handler);
        btn.onClick.AddListener(handler);
    }

    private void PrepareSharedButtons()
    {
        PrepareSharedButton(btnUp);
        PrepareSharedButton(btnDown);
        PrepareSharedButton(btnChoose);
        PrepareSharedButton(btnExit);
    }

    private static void PrepareSharedButton(Button btn)
    {
        if (btn == null)
            return;

        foreach (var tmp in btn.GetComponentsInChildren<TMP_Text>(true))
            tmp.raycastTarget = false;

        btn.transform.SetAsLastSibling();
    }

    private void TryAutoBindSharedButtons()
    {
        var searchRoots = new System.Collections.Generic.List<Transform>();
        if (bbMachineRoot != null)
            searchRoots.Add(bbMachineRoot.transform);

        var canvas = bbMachineRoot != null
            ? bbMachineRoot.GetComponentInParent<Canvas>()
            : GetComponentInParent<Canvas>();
        if (canvas != null && !searchRoots.Contains(canvas.transform))
            searchRoots.Add(canvas.transform);

        foreach (var root in searchRoots)
        {
            if (root == null)
                continue;

            foreach (var btn in root.GetComponentsInChildren<Button>(true))
            {
                if (IsUnderRingHierarchy(btn.transform))
                    continue;

                string label = btn.GetComponentInChildren<TMP_Text>()?.text?.Trim().ToLowerInvariant() ?? string.Empty;

                switch (label)
                {
                    case "up" when btnUp == null:
                        btnUp = btn;
                        break;
                    case "down" when btnDown == null:
                        btnDown = btn;
                        break;
                    case "choose" when btnChoose == null:
                        btnChoose = btn;
                        break;
                    case "exit" when btnExit == null:
                        btnExit = btn;
                        break;
                }
            }
        }
    }

    private void OnUpPressed()
    {
        if (_state == BBPhoneState.ContactList)
            contactListView?.MoveUp();
        else if (_state == BBPhoneState.Dialogue)
            bbDialogueView?.NavigateUp();
    }

    private void OnDownPressed()
    {
        if (_state == BBPhoneState.ContactList)
            contactListView?.MoveDown();
        else if (_state == BBPhoneState.Dialogue)
            bbDialogueView?.NavigateDown();
    }

    private void OnChoosePressed()
    {
        if (_state == BBPhoneState.ContactList)
            contactListView?.Confirm();
        else if (_state == BBPhoneState.Dialogue)
            bbDialogueView?.ConfirmChoice();
    }

    /// <summary>Tab：Idle 时打开联系人页；Ringing 时接听进对话。</summary>
    public void TryOpenOrAccept()
    {
        if (_state == BBPhoneState.Dialogue)
            return;

        if (_state == BBPhoneState.Ringing)
        {
            AnswerIncomingCall();
            return;
        }

        if (_state == BBPhoneState.Idle)
            OpenContactList();
    }

    /// <summary>打开 BB 机面板（不含 Ring）。</summary>
    public void OpenContactList()
    {
        if (_state == BBPhoneState.Dialogue)
            return;

        _state = BBPhoneState.ContactList;
        HideRingButton();
        SetBBMachineActive(true);
        PrepareSharedButtons();

        bbDialogueView?.Hide();
        contactListView?.Open();
    }

    /// <summary>Exit：关闭 BB 机面板；Ring 隐藏直至下次来电响铃。</summary>
    public void CloseBBMachine()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            DialogueManager.Instance.StopDialogue();

        contactListView?.Close();
        bbDialogueView?.Hide();

        SetBBMachineActive(false);
        HideRingButton();

        ringButtonView?.SetRinging(false);
        _pendingIncomingNode = null;
        _state = BBPhoneState.Idle;
    }

    public void TriggerIncomingCall()
    {
        if (_state == BBPhoneState.Dialogue)
            return;

        if (incomingCallResolver == null)
        {
            Debug.LogError("[BBPhoneController] 未绑定 IncomingCallResolver。");
            return;
        }

        var pick = incomingCallResolver.PickIncoming();
        _pendingIncomingNode = pick.Node;
        _pendingIncomingSource = pick.Source;
        if (_pendingIncomingNode == null)
        {
            Debug.LogWarning("[BBPhoneController] 未能解析来电起点。");
            return;
        }

        if (_state == BBPhoneState.ContactList)
        {
            StartDialogue(_pendingIncomingNode, DialogueSessionContext.ForIncoming(_pendingIncomingSource));
            _pendingIncomingNode = null;
            return;
        }

        _state = BBPhoneState.Ringing;
        ShowRingButton();
        ringButtonView?.SetRinging(true);
    }

    /// <summary>点 Ring：打开 BB 机面板并直接进入来电对话（跳过联系人页）。</summary>
    private void OnRingButtonClicked()
    {
        if (_state == BBPhoneState.Dialogue)
            return;

        AnswerIncomingCall();
    }

    /// <summary>接听：关响铃表现，打开 BB 机，StartDialogue（不打开联系人列表）。</summary>
    private void AnswerIncomingCall()
    {
        if (_state == BBPhoneState.Dialogue)
            return;

        if (_pendingIncomingNode == null)
        {
            if (incomingCallResolver == null)
            {
                Debug.LogError("[BBPhoneController] 未绑定 IncomingCallResolver，无法接听。");
                return;
            }

            var pick = incomingCallResolver.PickIncoming();
            _pendingIncomingNode = pick.Node;
            _pendingIncomingSource = pick.Source;
            if (_pendingIncomingNode == null)
            {
                Debug.LogWarning("[BBPhoneController] 未能解析来电起点。");
                return;
            }
        }

        ringButtonView?.SetRinging(false);
        _state = BBPhoneState.Idle;

        var node = _pendingIncomingNode;
        var source = _pendingIncomingSource;
        _pendingIncomingNode = null;
        StartDialogue(node, DialogueSessionContext.ForIncoming(source));
    }

    private void OnContactConfirmed(int index)
    {
        if (contactDatabase == null)
            return;

        var profile = contactDatabase.Get(index);
        if (profile == null || profile.defaultStartNode == null)
        {
            Debug.LogWarning("[BBPhoneController] 联系人未配置或缺少 defaultStartNode。");
            return;
        }

        StartDialogue(profile.defaultStartNode, DialogueSessionContext.ForContact(profile, index));
    }

    private void StartDialogue(DialogueNode startNode, DialogueSessionContext context = null)
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[BBPhoneController] 场景中缺少 DialogueManager。");
            return;
        }

        if (DialogueManager.Instance.IsDialogueActive || startNode == null)
            return;

        _state = BBPhoneState.Dialogue;
        HideRingButton();
        SetBBMachineActive(true);
        PrepareSharedButtons();

        contactListView?.Close();
        bbDialogueView?.Show();

        DialogueManager.Instance.StartDialogue(
            startNode,
            context ?? new DialogueSessionContext());
    }

    private void OnDialogueEnded()
    {
        CloseBBMachine();
    }

    private void OnDestroy()
    {
        if (contactListView != null)
            contactListView.OnConfirmContact -= OnContactConfirmed;

        if (ringButtonView != null)
            ringButtonView.OnClicked -= OnRingButtonClicked;

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.OnDialogueEnded -= OnDialogueEnded;
    }
}
