using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("背包设置")]
    public int rows = 3;
    public int columns = 4;
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public Transform slotParent;

    private List<InventorySlot> slots = new List<InventorySlot>();
    private bool isOpen = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        int totalSlots = rows * columns;
        for (int i = 0; i < totalSlots; i++)
        {
            slots.Add(new InventorySlot());
        }
        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
    }

    //背包接收资源

    public bool AddResource(ResourceType type, int amount)
    {
        ResourceData data = ResourceSpawner.Instance.GetResourceData(type);
        int maxStack = data != null ? data.maxStack : 99;

        // 先尝试堆叠到已有的同类格子上
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].resourceType == type && slots[i].quantity < maxStack)
            {
                int spaceLeft = maxStack - slots[i].quantity;
                int toAdd = Mathf.Min(amount, spaceLeft);
                slots[i].quantity += toAdd;
                amount -= toAdd;

                if (amount <= 0) return true;
            }
        }

        // 同类格子都满了，找空格子
        while (amount > 0)
        {
            int emptySlot = FindEmptySlot();
            if (emptySlot == -1)
            {
                Debug.Log("背包已满！");
                return false;
            }

            int toAdd = Mathf.Min(amount, maxStack);
            slots[emptySlot].resourceType = type;
            slots[emptySlot].quantity = toAdd;
            amount -= toAdd;
        }

        return true;
    }

    int FindEmptySlot()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty())
                return i;
        }
        return -1;
    }

    public void RemoveResource(int slotIndex, int amount)
    {
        if (slotIndex >= 0 && slotIndex < slots.Count)
        {
            slots[slotIndex].quantity -= amount;
            if (slots[slotIndex].quantity <= 0)
            {
                slots[slotIndex].Clear();
            }
        }
    }

    public void SwapSlots(int fromIndex, int toIndex)
    {
        InventorySlot temp = slots[fromIndex];
        slots[fromIndex] = slots[toIndex];
        slots[toIndex] = temp;
    }

    public void DropItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;
        if (slots[slotIndex].IsEmpty()) return;

        ResourceType type = slots[slotIndex].resourceType;
        ResourceData data = ResourceSpawner.Instance.GetResourceData(type);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && data != null && data.prefab != null)
        {
            Vector3 dropPos = player.transform.position + player.transform.forward * 1f;
            Instantiate(data.prefab, dropPos, Quaternion.identity);
        }

        RemoveResource(slotIndex, 1);
    }

    public InventorySlot GetSlot(int index)
    {
        if (index >= 0 && index < slots.Count)
            return slots[index];
        return null;
    }

    public int GetSlotCount()
    {
        return slots.Count;
    }
}
[System.Serializable]
public class InventorySlot
{
    public ResourceType resourceType;
    public int quantity;

    public bool IsEmpty()
    {
        return quantity <= 0;
    }

    public void Clear()
    {
        resourceType = ResourceType.Wood;
        quantity = 0;
    }
}
