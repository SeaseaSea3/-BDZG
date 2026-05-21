using UnityEngine;

public class PatientRecordImageUI : MonoBehaviour
{
    [Header("²¡ÀúÍ¼Æ¬Ò³")]
    public GameObject page1;
    public GameObject page2;

    private int currentPage = 1;

    private void Start()
    {
        ShowPage(1);
    }

    public void NextPage()
    {
        currentPage++;

        if (currentPage > 2)
        {
            currentPage = 2;
        }

        ShowPage(currentPage);
    }

    public void PrevPage()
    {
        currentPage--;

        if (currentPage < 1)
        {
            currentPage = 1;
        }

        ShowPage(currentPage);
    }

    private void ShowPage(int page)
    {
        currentPage = page;

        page1.SetActive(currentPage == 1);
        page2.SetActive(currentPage == 2);
    }
}