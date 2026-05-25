using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 右下角响铃按钮：自行替换 Image 贴图 / Animator / 音效。代码只切换状态，不生成 UI。
/// </summary>
public class RingButtonView : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;

    [Header("不响铃 / 响铃 贴图（拖入 Sprite）")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite ringingSprite;

    [Header("响铃表现（可选，自行配置）")]
    [SerializeField] private Animator ringAnimator;
    [SerializeField] private string ringBoolParameter = "IsRinging";
    [SerializeField] private AudioSource ringAudioSource;

    private bool _isRinging;

    public bool IsRinging => _isRinging;

    public System.Action OnClicked;

    private void Start()
    {
        if (button != null)
            button.onClick.AddListener(() => OnClicked?.Invoke());
        ApplyVisual();
    }

    public void SetRinging(bool ringing)
    {
        _isRinging = ringing;
        ApplyVisual();
    }

    private void ApplyVisual()
    {
        if (buttonImage != null)
        {
            var s = _isRinging ? ringingSprite : idleSprite;
            if (s != null)
                buttonImage.sprite = s;
        }

        if (ringAnimator != null && !string.IsNullOrEmpty(ringBoolParameter))
            ringAnimator.SetBool(ringBoolParameter, _isRinging);

        if (ringAudioSource != null)
        {
            if (_isRinging && !ringAudioSource.isPlaying)
                ringAudioSource.Play();
            else if (!_isRinging && ringAudioSource.isPlaying)
                ringAudioSource.Stop();
        }
    }
}
