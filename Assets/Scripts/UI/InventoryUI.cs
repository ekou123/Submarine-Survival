using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Profiling;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform slotContainer;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Character owner;
    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();
    private readonly List<ItemData> items = new List<ItemData>();


    
    


    public void ClearInventory()
    {
        items.Clear();
        foreach (var slot in slots)
        {
            Destroy(slot.gameObject);
            slots.Clear();
        }
    }

    public void Setup(Character character)
    {
        owner = character;
    }

    public void ResetInventoryUI()
    {
        // 1) Hide all
        foreach (var slot in slots)
            slot.gameObject.SetActive(false);

    // 2) Fill & show the ones you need
        for (int i = 0; i < items.Count && i < slots.Count; i++)
        {
            slots[i].Setup(items[i].iconSprite, items[i]);
            slots[i].gameObject.SetActive(true);
        }
    }
}
