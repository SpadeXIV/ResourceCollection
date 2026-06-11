using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ResourceSpawner : MonoBehaviour
{
    public static ResourceSpawner Instance;

    [Header("资源数据")]
    public ResourceData woodData;
    public ResourceData ironData;
    public ResourceData magicDustData;

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
            SpawnResource(woodData.prefab, ResourceType.Wood);

        for (int i = 0; i < ironCount; i++)
            SpawnResource(ironData.prefab, ResourceType.Iron);

        for (int i = 0; i < magicCount; i++)
            SpawnResource(magicDustData.prefab, ResourceType.MagicDust);
    }

    GameObject SpawnResource(GameObject prefab, ResourceType type)
    {
        Vector3 randomPos = GetRandomPosition();
        GameObject obj = Instantiate(prefab, randomPos, Quaternion.identity);
        obj.GetComponent<CollectibleResource>().resourceType = type;

        activeResources[type].Add(obj);
        return obj;
    }
    //采集
    public void OnResourceCollected(ResourceType type, GameObject collectedObj)
    {
        activeResources[type].Remove(collectedObj);
        StartCoroutine(RespawnAfterDelay(type));
    }
    //协程 被采集后生成新资源
    System.Collections.IEnumerator RespawnAfterDelay(ResourceType type)
    {
        yield return new WaitForSeconds(respawnDelay);

        GameObject prefab = GetPrefabByType(type);
        SpawnResource(prefab, type);
    }
     //种类判断
    public ResourceData GetResourceData(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Wood: return woodData;
            case ResourceType.Iron: return ironData;
            case ResourceType.MagicDust: return magicDustData;
            default: return null;
        }
    }
   
    public GameObject GetPrefabByType(ResourceType type)
    {
        ResourceData data = GetResourceData(type);
        return data != null ? data.prefab : null;
    }
    //随机地点生成
    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-spawnArea.x, spawnArea.x);
        float z = Random.Range(-spawnArea.y, spawnArea.y);
        return new Vector3(x, yPosition, z);
    }
}