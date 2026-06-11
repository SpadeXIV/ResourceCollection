using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ResourceSpawner : MonoBehaviour
{
    public static ResourceSpawner Instance;

    [Header("资源预制体")]
    public GameObject woodPrefab;
    public GameObject ironPrefab;
    public GameObject magicPrefab;

    [Header("场景地面")]
    public GameObject groundPlane;          
    public float spawnAreaPercent = 0.8f;    // 生成区域占地面的百分比

    [Header("生成设置")]
    public float yPosition = 0.5f;
    public float respawnDelay = 2f;

    [Header("初始数量")]
    public int woodCount = 5;
    public int ironCount = 3;
    public int magicCount = 2;

    private Vector2 spawnArea;
    private Dictionary<ResourceType, List<GameObject>> activeResources = new Dictionary<ResourceType, List<GameObject>>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        CalculateSpawnArea();

        activeResources[ResourceType.Wood] = new List<GameObject>();
        activeResources[ResourceType.Iron] = new List<GameObject>();
        activeResources[ResourceType.MagicDust] = new List<GameObject>();

        SpawnInitialResources();
    }

    void CalculateSpawnArea()
    {
        if (groundPlane != null)
        {
            Vector3 scale = groundPlane.transform.localScale;
            float halfX = 5f * scale.x * spawnAreaPercent;
            float halfZ = 5f * scale.z * spawnAreaPercent;
            spawnArea = new Vector2(halfX, halfZ);
        }
    }

    void SpawnInitialResources()
    {
        for (int i = 0; i < woodCount; i++)
            SpawnResource(woodPrefab, ResourceType.Wood);

        for (int i = 0; i < ironCount; i++)
            SpawnResource(ironPrefab, ResourceType.Iron);

        for (int i = 0; i < magicCount; i++)
            SpawnResource(magicPrefab, ResourceType.MagicDust);
    }

    GameObject SpawnResource(GameObject prefab, ResourceType type)
    {
        Vector3 randomPos = GetRandomPosition();
        GameObject obj = Instantiate(prefab, randomPos, Quaternion.identity);
        obj.GetComponent<CollectibleResource>().resourceType = type;

        activeResources[type].Add(obj);
        return obj;
    }

    public void OnResourceCollected(ResourceType type, GameObject collectedObj)
    {
        activeResources[type].Remove(collectedObj);
        StartCoroutine(RespawnAfterDelay(type));
    }

    System.Collections.IEnumerator RespawnAfterDelay(ResourceType type)
    {
        yield return new WaitForSeconds(respawnDelay);

        GameObject prefab = GetPrefabByType(type);
        SpawnResource(prefab, type);
    }

    GameObject GetPrefabByType(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Wood: return woodPrefab;
            case ResourceType.Iron: return ironPrefab;
            case ResourceType.MagicDust: return magicPrefab;
            default: return null;
        }
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-spawnArea.x, spawnArea.x);
        float z = Random.Range(-spawnArea.y, spawnArea.y);
        return new Vector3(x, yPosition, z);
    }
}