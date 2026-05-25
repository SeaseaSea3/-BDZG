using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScannerToolController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("病人区域")]
    public RectTransform patientArea;

    [Header("显示器屏幕")]
    public GameObject scannerScreen;
    public Image toolIcon;

    [Header("7个道具")]
    public ToolData[] tools;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 startPosition;

    private bool hasScanned = false;
    private int currentToolIndex = 0;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startPosition = rectTransform.anchoredPosition;

        hasScanned = false;
        currentToolIndex = 0;

        if (scannerScreen != null)
        {
            scannerScreen.SetActive(false);
        }

        if (toolIcon != null)
        {
            toolIcon.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!hasScanned)
        {
            return;
        }

        SwitchToolByMouseWheel();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ConfirmCurrentTool();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (hasScanned)
        {
            return;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (hasScanned)
        {
            return;
        }

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (hasScanned)
        {
            return;
        }

        if (IsOverPatient())
        {
            ShowToolsOnScanner();
        }
        else
        {
            rectTransform.anchoredPosition = startPosition;
        }
    }

    private bool IsOverPatient()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            patientArea,
            Input.mousePosition,
            canvas.worldCamera
        );
    }

    private void ShowToolsOnScanner()
    {
        hasScanned = true;
        currentToolIndex = 0;

        if (scannerScreen != null)
        {
            scannerScreen.SetActive(true);
        }

        ShowCurrentTool();
    }

    private void SwitchToolByMouseWheel()
    {
        float wheel = Input.GetAxis("Mouse ScrollWheel");

        if (wheel > 0f)
        {
            PreviousTool();
        }
        else if (wheel < 0f)
        {
            NextTool();
        }
    }

    private void NextTool()
    {
        if (tools == null || tools.Length == 0)
        {
            Debug.LogError("Tools 道具列表为空！");
            return;
        }

        currentToolIndex++;

        if (currentToolIndex >= tools.Length)
        {
            currentToolIndex = 0;
        }

        ShowCurrentTool();
    }

    private void PreviousTool()
    {
        if (tools == null || tools.Length == 0)
        {
            Debug.LogError("Tools 道具列表为空！");
            return;
        }

        currentToolIndex--;

        if (currentToolIndex < 0)
        {
            currentToolIndex = tools.Length - 1;
        }

        ShowCurrentTool();
    }

    private void ShowCurrentTool()
    {
        if (tools == null || tools.Length == 0)
        {
            Debug.LogError("Tools 道具列表为空！");
            return;
        }

        ToolData currentTool = tools[currentToolIndex];

        if (toolIcon != null)
        {
            toolIcon.gameObject.SetActive(true);
            toolIcon.sprite = currentTool.toolIcon;
            toolIcon.preserveAspect = true;
        }
    }

    private void ConfirmCurrentTool()
    {
        if (tools == null || tools.Length == 0)
        {
            Debug.LogError("Tools 道具列表为空！");
            return;
        }

        ToolData currentTool = tools[currentToolIndex];


        if (string.IsNullOrEmpty(currentTool.sceneName))
        {
            Debug.LogError("当前道具没有填写 Scene Name！");
            return;
        }

        SceneManager.LoadScene(currentTool.sceneName);
    }
}