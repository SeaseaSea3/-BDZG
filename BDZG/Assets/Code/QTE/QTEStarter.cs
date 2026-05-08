using UnityEngine;

public class QTEStarter : MonoBehaviour
{
    public GameObject mainUI;   // ⭐主界面
    public GameObject qtePanel;
    public QTEController qte;

    public void StartQTE()
    {
        // ⭐隐藏主界面
        if (mainUI != null)
            mainUI.SetActive(false);

        // ⭐显示QTE
        qtePanel.SetActive(true);

        if (qte != null)
            qte.StartQTE();
    }
}