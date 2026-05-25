using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 挂在已有 Image 上，通过切换 Sprite 表示选中/未选中。不 Instantiate 任何 UI。
/// </summary>
[RequireComponent(typeof(Image))]
public class SpriteStateImage : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite selectedSprite;

    private void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
    }

    public void ApplyTheme(BBPhoneUITheme theme, bool useSelectedStyle)
    {
        if (theme == null)
            return;
        normalSprite = theme.optionSlotNormal;
        selectedSprite = theme.optionSlotSelected;
        SetSelected(useSelectedStyle);
    }

    public void SetSprites(Sprite normal, Sprite selected)
    {
        normalSprite = normal;
        selectedSprite = selected;
    }

    public void SetSelected(bool selected)
    {
        if (targetImage == null)
            return;
        var s = selected ? selectedSprite : normalSprite;
        if (s != null)
            targetImage.sprite = s;
    }
}
