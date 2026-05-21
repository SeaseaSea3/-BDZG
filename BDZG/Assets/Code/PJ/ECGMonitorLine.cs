using UnityEngine;
using UnityEngine.UI;

public class ECGMonitorLine : MaskableGraphic
{
    [Header("线条设置")]
    public float lineThickness = 3f;
    public int pointCount = 160;

    [Header("动画速度")]
    public float moveSpeed = 1.4f;

    [Header("心跳强度")]
    public float amplitude = 35f;

    [Header("小抖动")]
    public float noiseStrength = 3f;
    public float noiseSpeed = 8f;

    [Header("生命值")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("心跳频率")]
    public float heartRate = 1.2f;

    private float timeOffset;

    protected override void Start()
    {
        base.Start();
        raycastTarget = false;
    }

    private void Update()
    {
        timeOffset += Time.unscaledDeltaTime * moveSpeed;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (pointCount < 2)
        {
            return;
        }

        Rect rect = rectTransform.rect;
        Vector2[] points = new Vector2[pointCount];

        float healthRate = Mathf.Clamp01(currentHealth / maxHealth);

        for (int i = 0; i < pointCount; i++)
        {
            float t = (float)i / (pointCount - 1);
            float x = Mathf.Lerp(rect.xMin, rect.xMax, t);

            float wave = GetECGWave(t, timeOffset);

            float noise = Mathf.PerlinNoise(
                t * 12f,
                Time.unscaledTime * noiseSpeed
            );

            noise = (noise - 0.5f) * 2f;

            float y;

            if (currentHealth <= 0)
            {
                y = noise * 0.2f;
            }
            else
            {
                y = wave * amplitude * healthRate;
                y += noise * noiseStrength * healthRate;
            }

            points[i] = new Vector2(x, y);
        }

        for (int i = 0; i < pointCount - 1; i++)
        {
            DrawLineSegment(vh, points[i], points[i + 1], lineThickness, color);
        }
    }

    private float GetECGWave(float x, float time)
    {
        float t = (x * heartRate + time) % 1f;

        float y = 0f;

        y += Gaussian(t, 0.16f, 0.035f, 0.25f);
        y += Gaussian(t, 0.32f, 0.012f, -0.35f);
        y += Gaussian(t, 0.35f, 0.010f, 1.4f);
        y += Gaussian(t, 0.38f, 0.014f, -0.65f);
        y += Gaussian(t, 0.58f, 0.060f, 0.45f);

        return y;
    }

    private float Gaussian(float x, float center, float width, float height)
    {
        float value = x - center;
        return height * Mathf.Exp(-(value * value) / (2f * width * width));
    }

    private void DrawLineSegment(VertexHelper vh, Vector2 start, Vector2 end, float thickness, Color lineColor)
    {
        Vector2 direction = (end - start).normalized;

        if (direction == Vector2.zero)
        {
            return;
        }

        Vector2 normal = new Vector2(-direction.y, direction.x) * thickness * 0.5f;

        int index = vh.currentVertCount;

        UIVertex v1 = UIVertex.simpleVert;
        UIVertex v2 = UIVertex.simpleVert;
        UIVertex v3 = UIVertex.simpleVert;
        UIVertex v4 = UIVertex.simpleVert;

        v1.color = lineColor;
        v2.color = lineColor;
        v3.color = lineColor;
        v4.color = lineColor;

        v1.position = start + normal;
        v2.position = start - normal;
        v3.position = end - normal;
        v4.position = end + normal;

        vh.AddVert(v1);
        vh.AddVert(v2);
        vh.AddVert(v3);
        vh.AddVert(v4);

        vh.AddTriangle(index, index + 1, index + 2);
        vh.AddTriangle(index, index + 2, index + 3);
    }

    public void SetHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);
        SetVerticesDirty();
    }
}