using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private InventoryUI inventoryUI;

    private readonly List<ItemData> items = new List<ItemData>();

    public bool AddItem(ItemData item)
    {
        items.Add(item);
        inventoryUI.AddItem(item);
        return true;
    }


}
