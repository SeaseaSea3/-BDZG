using UnityEngine;

public class CellSpawner : MonoBehaviour
{
    public GameObject cellPrefab;
    public RectTransform spawnParent;

    public float spawnX = -650f;
    public float minY = -180f;
    public float maxY = 120f;

    public float minInterval = 0.7f;
    public float maxInterval = 1.4f;

    private bool spawning = true;

    void Start()
    {
        spawning = true;
        Invoke(nameof(SpawnCell), 0.5f);
    }

    void SpawnCell()
    {
        if (!spawning) return;

        GameObject cell = Instantiate(cellPrefab, spawnParent);

        RectTransform rect = cell.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(spawnX, Random.Range(minY, maxY));

        Invoke(nameof(SpawnCell), Random.Range(minInterval, maxInterval));
    }

    public void StopSpawn()
    {
        spawning = false;
        CancelInvoke(nameof(SpawnCell));
    }
}