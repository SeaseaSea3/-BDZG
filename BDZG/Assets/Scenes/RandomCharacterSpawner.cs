using UnityEngine;
using System;

[Serializable]
public class CharacterProbability
{
    public GameObject characterPrefab; // 人物预制体
    public float probability;          // 概率（可在面板调）
}

public class RandomCharacterSpawner : MonoBehaviour
{
    [Header("概率配置（自由增删人物）")]
    public CharacterProbability[] characterList;

    [Header("都不出现的概率")]
    public float noSpawnProbability = 50f;

    [Header("生成位置")]
    public Transform spawnPoint;

    private GameObject currentSpawned;

    /// <summary>
    /// 外部调用这个方法，生成随机人物
    /// </summary>
    public void SpawnRandomCharacter()
    {
        // 销毁上一次生成的人物
        if (currentSpawned != null)
            Destroy(currentSpawned);

        // 计算总概率
        float totalProb = noSpawnProbability;
        foreach (var c in characterList)
            totalProb += c.probability;

        float random = UnityEngine.Random.Range(0, totalProb);
        float current = 0;

        // 随机选人
        foreach (var c in characterList)
        {
            current += c.probability;
            if (random < current)
            {
                Spawn(c.characterPrefab);
                Debug.Log("🎭 生成人物：" + c.characterPrefab.name);
                return;
            }
        }

        // 都不生成
        Debug.Log("🎭 本次没有生成人物");
    }

    void Spawn(GameObject prefab)
    {
        if (prefab == null || spawnPoint == null) return;
        currentSpawned = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
    }
}