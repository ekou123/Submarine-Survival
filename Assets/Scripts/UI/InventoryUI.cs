using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform slotContainer;
    [SerializeField] private GameObject slotPrefab;

    private readonly List<ItemData> items = new List<ItemData>();
    private readonly List<InventorySlot> slots = new List<InventorySlot>();

    public void AddItem(ItemData newItem)
    {
        items.Add(newItem);
        CreateSlot(newItem);
    }

    private void CreateSlot(ItemData item)
    {
        var go = Instantiate(slotPrefab, slotContainer);
        var slot = go.GetComponent<InventorySlot>();
        slot.Setup(item.iconSprite);
    }

    public void ClearInventory()
    {
        items.Clear();
        foreach (var slot in slots)
        {
            Destroy(slot.gameObject);
            slots.Clear();
        }
    }
}
