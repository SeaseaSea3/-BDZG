using UnityEngine;

public class CellMove : MonoBehaviour
{
    public float speed = 180f;
    public float destroyX = 700f;

    public static bool stopAll = false;

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (stopAll) return;

        rect.anchoredPosition += Vector2.right * speed * Time.deltaTime;

        if (rect.anchoredPosition.x > destroyX)
        {
            Destroy(gameObject);
        }
    }
}