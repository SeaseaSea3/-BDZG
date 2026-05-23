using System;
using System.Collections.Generic;
using UnityEngine;

public enum GuestType
{
    // 普通客人1-4
    Normal1,
    Normal2,
    Normal3,
    Normal4,
    // 特殊客人1-2
    Special1,
    Special2
}

public class GuestQueueManager : MonoBehaviour
{
    [Header("生成间隔")]
    public float minSpawnInterval = 20f;
    public float maxSpawnInterval = 60f;

    [Header("绑定物体")]
    public GameObject[] guestPrefabs;
    public Transform queueParent;
    public GameObject frontDeskBg;

    [Header("排队位置")]
    public Vector3 firstGuestPos;
    public float spacing = 1.5f;

    // 队列和对象列表
    private Queue<GuestType> guestQueue = new Queue<GuestType>();
    private List<GameObject> spawnedGuests = new List<GameObject>();

    // 👇 单独计数：普通客人总数、特殊客人1、特殊客人2
    private int normalGuestCount = 0;
    private int special1Count = 0;
    private int special2Count = 0;

    private float spawnTimer;
    private float currentInterval;

    private void Awake()
    {
        currentInterval = GetRandomInterval();
    }

    private void Update()
    {
        // 后台持续计时生成客人
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentInterval)
        {
            SpawnGuest();
            spawnTimer = 0;
            currentInterval = GetRandomInterval();
        }

        // 根据前台背景状态同步显隐所有客人
        bool showGuest = frontDeskBg != null && frontDeskBg.activeSelf;
        foreach (var guest in spawnedGuests)
        {
            guest.SetActive(showGuest);
        }
    }

    private void SpawnGuest()
    {
        if (guestPrefabs == null || guestPrefabs.Length == 0) return;

        // 随机生成客人
        int idx = UnityEngine.Random.Range(0, guestPrefabs.Length);
        GuestType type = (GuestType)idx;
        GameObject prefab = guestPrefabs[idx];
        if (prefab == null) return;

        // 生成位置计算
        Vector3 pos = firstGuestPos + new Vector3(-spacing * spawnedGuests.Count, 0, 0);
        GameObject guest = Instantiate(prefab, pos, Quaternion.identity, queueParent);
        guest.SetActive(frontDeskBg != null && frontDeskBg.activeSelf);
        guest.name = $"Guest_{type}_{spawnedGuests.Count}";

        // 加入队列和计数
        guestQueue.Enqueue(type);
        spawnedGuests.Add(guest);

        // 👇 按类型分开计数
        switch (type)
        {
            case GuestType.Normal1:
            case GuestType.Normal2:
            case GuestType.Normal3:
            case GuestType.Normal4:
                normalGuestCount++;
                break;
            case GuestType.Special1:
                special1Count++;
                break;
            case GuestType.Special2:
                special2Count++;
                break;
        }

        Debug.Log($"[客人生成] {type} 来了 | 普通客人：{normalGuestCount} | 特殊1：{special1Count} | 特殊2：{special2Count}");
    }

    private float GetRandomInterval()
    {
        return UnityEngine.Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    // ===================== 给后续功能的接口 =====================

    /// <summary>
    /// 获取普通客人总数（1-4号合并）
    /// </summary>
    public int GetNormalGuestCount() => normalGuestCount;

    /// <summary>
    /// 获取特殊客人1的数量
    /// </summary>
    public int GetSpecial1Count() => special1Count;

    /// <summary>
    /// 获取特殊客人2的数量
    /// </summary>
    public int GetSpecial2Count() => special2Count;

    /// <summary>
    /// 获取所有客人总数
    /// </summary>
    public int GetTotalGuestCount() => guestQueue.Count;

    /// <summary>
    /// 获取队首客人类型（不移除）
    /// </summary>
    public GuestType? PeekNextGuestType()
    {
        return guestQueue.Count > 0 ? guestQueue.Peek() : null;
    }

    /// <summary>
    /// 获取队首客人对象
    /// </summary>
    public GameObject PeekNextGuestObject()
    {
        return spawnedGuests.Count > 0 ? spawnedGuests[0] : null;
    }

    /// <summary>
    /// 移除队首客人（服务完成后调用）
    /// </summary>
    public GuestType? DequeueGuest()
    {
        if (guestQueue.Count == 0 || spawnedGuests.Count == 0) return null;

        GuestType type = guestQueue.Dequeue();
        GameObject guestObj = spawnedGuests[0];
        spawnedGuests.RemoveAt(0);
        Destroy(guestObj);

        // 👇 客人离开时更新计数
        switch (type)
        {
            case GuestType.Normal1:
            case GuestType.Normal2:
            case GuestType.Normal3:
            case GuestType.Normal4:
                normalGuestCount--;
                break;
            case GuestType.Special1:
                special1Count--;
                break;
            case GuestType.Special2:
                special2Count--;
                break;
        }

        // 更新剩下客人的位置
        UpdateQueuePositions();

        Debug.Log($"[客人离开] {type} 已离开 | 普通客人：{normalGuestCount} | 特殊1：{special1Count} | 特殊2：{special2Count}");
        return type;
    }

    private void UpdateQueuePositions()
    {
        for (int i = 0; i < spawnedGuests.Count; i++)
        {
            spawnedGuests[i].transform.localPosition = firstGuestPos + new Vector3(-spacing * i, 0, 0);
        }
    }

    /// <summary>
    /// 重置所有队列和计数
    /// </summary>
    public void ResetQueue()
    {
        guestQueue.Clear();
        foreach (var g in spawnedGuests) Destroy(g);
        spawnedGuests.Clear();

        normalGuestCount = 0;
        special1Count = 0;
        special2Count = 0;

        spawnTimer = 0;
        Debug.Log("队列已重置，所有计数归零");
    }
}