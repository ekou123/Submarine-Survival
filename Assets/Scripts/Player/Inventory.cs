using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private InventoryUI inventoryUI;

    [SerializeField]
    public List<ItemData> items = new List<ItemData>();
    
    [HideInInspector]
    public int maxInventorySlots = 34;

    private void Start()
    {
    }

    public bool AddItem(ItemData item)
    {
        items.Add(item);
        
        return true;
    }


}
