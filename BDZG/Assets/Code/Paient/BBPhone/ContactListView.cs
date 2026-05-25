using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// BB 机联系人列表：只负责显示与索引，按钮由 BBPhoneController 统一分发。
/// </summary>
public class ContactListView : MonoBehaviour
{
    [SerializeField] private ContactDatabase database;
    [SerializeField] private BBPhoneUITheme theme;

    [Header("联系人 Panel（进对话时隐藏）")]
    [SerializeField] private GameObject contactPanelRoot;

    [Header("显示当前选中联系人")]
    [SerializeField] private TMP_Text contactNameText;
    [SerializeField] private TMP_Text contactAddressText;
    [SerializeField] private TMP_Text contactPhoneText;
    [SerializeField] private Image contactIconImage;
    [SerializeField] private SpriteStateImage contactHighlightImage;

    private int _index;

    public System.Action<int> OnConfirmContact;

    private Transform BindRoot => contactPanelRoot != null ? contactPanelRoot.transform : transform;

    private void Awake()
    {
        TryAutoBindDisplayFields();
    }

    public void BindDatabase(ContactDatabase db)
    {
        database = db;
    }

    public void Open()
    {
        TryAutoBindDisplayFields();
        ClampIndexToValidContact();
        SetContactPanelActive(true);
        RefreshDisplay();
    }

    public void Close()
    {
        SetContactPanelActive(false);
    }

    private void SetContactPanelActive(bool active)
    {
        if (contactPanelRoot != null)
            contactPanelRoot.SetActive(active);
    }

    /// <summary>上一条联系人（循环：在第一条时回到最后一条）。</summary>
    public void MoveUp()
    {
        MoveIndex(-1);
    }

    /// <summary>下一条联系人（循环：在最后一条时回到第一条）。</summary>
    public void MoveDown()
    {
        MoveIndex(1);
    }

    private void MoveIndex(int delta)
    {
        if (database == null || database.Count == 0)
            return;

        int count = database.Count;
        if (count == 1)
        {
            ClampIndexToValidContact();
            RefreshDisplay();
            return;
        }

        int start = _index;
        for (int step = 0; step < count; step++)
        {
            _index = Mod(_index + delta, count);
            if (database.Get(_index) != null)
            {
                RefreshDisplay();
                return;
            }
        }

        _index = start;
        ClampIndexToValidContact();
        RefreshDisplay();
    }

    private static int Mod(int value, int count)
    {
        int r = value % count;
        return r < 0 ? r + count : r;
    }

    private void ClampIndexToValidContact()
    {
        if (database == null || database.Count == 0)
        {
            _index = 0;
            return;
        }

        if (database.Get(_index) != null)
            return;

        int valid = FindNextValidIndex(0, 1);
        _index = valid >= 0 ? valid : 0;
    }

    private int FindNextValidIndex(int start, int delta)
    {
        if (database == null || database.Count == 0)
            return -1;

        int count = database.Count;
        int i = start;
        for (int step = 0; step < count; step++)
        {
            if (database.Get(i) != null)
                return i;
            i = Mod(i + delta, count);
        }

        return -1;
    }

    public void Confirm()
    {
        if (database == null || database.Count == 0)
            return;
        OnConfirmContact?.Invoke(_index);
    }

    private void RefreshDisplay()
    {
        if (database == null)
            return;

        var profile = database.Get(_index);
        if (profile == null)
            return;

        if (contactNameText != null)
            contactNameText.text = profile.displayName ?? string.Empty;

        if (contactAddressText != null)
            contactAddressText.text = profile.address ?? string.Empty;

        if (contactPhoneText != null)
            contactPhoneText.text = profile.phoneNumber ?? string.Empty;

        if (contactIconImage != null)
        {
            contactIconImage.sprite = profile.listIcon;
            contactIconImage.enabled = profile.listIcon != null;
        }

        if (contactHighlightImage != null && theme != null)
        {
            contactHighlightImage.SetSprites(theme.contactEntryNormal, theme.contactEntrySelected);
            contactHighlightImage.SetSelected(true);
        }
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
            if (contactNameText == null && (n.Contains("contactname") || n == "name" || n.Contains("姓名")))
                contactNameText = tmp;
            else if (contactAddressText == null && (n.Contains("address") || n.Contains("addr") || n.Contains("住所")))
                contactAddressText = tmp;
            else if (contactPhoneText == null && (n.Contains("phone") || n.Contains("tel") || n.Contains("电话")))
                contactPhoneText = tmp;
        }
    }
}
