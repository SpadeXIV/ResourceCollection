using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewResource", menuName = "Game/Resource Data")]
public class ResourceData : ScriptableObject
{
    public string resourceName;      // 资源名称
    public ResourceType type;        // 资源类型枚举
    public Sprite Itemicon;          // 背包图标
    public GameObject prefab;        // 场景中的预制体
    public int maxStack = 64;        // 最大堆叠数量
}