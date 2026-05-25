using UnityEngine;
using UnityEngine.UI;

public class MedicalBoxPrefabClick : MonoBehaviour
{
    [Header("左下角医疗包按钮")]
    public Button boxButton;

    [Header("要显示/隐藏的弧线物体")]
    public GameObject arcLine;

    [Header("弹出来后可点击的物体")]
    public GameObject candy;
    public GameObject hpPack;

    [Header("弧线上的动画")]
    public Animator arcAnimator;

    [Header("点击打开后，多久允许糖果和血包点击")]
    public float enableItemDelay = 1.0f;

    private bool isOpen = false;
    private float timer = 0f;
    private bool waitingEnable = false;

    void Start()
    {
        if (boxButton != null)
        {
            boxButton.onClick.AddListener(ToggleBox);
        }

        // 一开始隐藏弧线
        if (arcLine != null)
        {
            arcLine.SetActive(false);
        }

        // 一开始糖果和血包不能点
        SetItemInteractable(candy, false);
        SetItemInteractable(hpPack, false);
    }

    void Update()
    {
        if (waitingEnable)
        {
            timer += Time.deltaTime;

            if (timer >= enableItemDelay)
            {
                waitingEnable = false;

                SetItemInteractable(candy, true);
                SetItemInteractable(hpPack, true);

                Debug.Log("糖果和血包可以点击了");
            }
        }
    }

    public void ToggleBox()
    {
        if (isOpen)
        {
            CloseBox();
        }
        else
        {
            OpenBox();
        }
    }

    void OpenBox()
    {
        isOpen = true;

        if (arcLine != null)
        {
            arcLine.SetActive(true);
        }

        if (arcAnimator != null)
        {
            arcAnimator.enabled = true;
            arcAnimator.Play(0, 0, 0f);
        }

        SetItemInteractable(candy, false);
        SetItemInteractable(hpPack, false);

        timer = 0f;
        waitingEnable = true;
    }

    void CloseBox()
    {
        isOpen = false;
        waitingEnable = false;

        if (arcLine != null)
        {
            arcLine.SetActive(false);
        }

        SetItemInteractable(candy, false);
        SetItemInteractable(hpPack, false);
    }

    void SetItemInteractable(GameObject obj, bool canInteract)
    {
        if (obj == null) return;

        Image image = obj.GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = canInteract;
        }

        Button button = obj.GetComponent<Button>();
        if (button != null)
        {
            button.interactable = canInteract;
        }

        Collider2D col = obj.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = canInteract;
        }
    }
}