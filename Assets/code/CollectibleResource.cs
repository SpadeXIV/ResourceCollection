using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Wood,
    Iron,
    MagicDust
}

public class CollectibleResource : MonoBehaviour
{
    public ResourceType resourceType;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    void Collect()
    {
        InventoryManager.Instance.AddResource(resourceType, 1);
        Debug.Log($"收集了: {resourceType}");
        // 通知生成器这个资源被收集了
        ResourceSpawner.Instance.OnResourceCollected(resourceType, gameObject);
        gameObject.SetActive(false);
    }
}