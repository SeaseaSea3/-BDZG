using UnityEngine;

public class CreateArcPoints : MonoBehaviour
{
    [Header("弧线设置")]
    public int pointCount = 30;
    public float radius = 200f;

    public float startAngle = 160f;
    public float endAngle = 20f;

    [ContextMenu("生成弧线点")]
    void Generate()
    {
        // 删除旧的
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < pointCount; i++)
        {
            float t = (float)i / (pointCount - 1);
            float angle = Mathf.Lerp(startAngle, endAngle, t) * Mathf.Deg2Rad;

            GameObject point = new GameObject("Point_" + i);
            point.transform.SetParent(transform);

            RectTransform rt = point.AddComponent<RectTransform>();

            rt.anchoredPosition = new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius
            );

            rt.localScale = Vector3.one;
        }
    }
}