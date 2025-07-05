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
    [SerializeField] private Inventory ownerInventory;
    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();
    private readonly List<ItemData> items = new List<ItemData>();


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        this.gameObject.SetActive(false);
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

    public void Setup(Character character)
    {
        ownerInventory = character.GetComponent<Inventory>();
        if (ownerInventory == null)
        {
            Debug.LogError("[InventoryUI] Could not find Inventory on Character Object");
        }
    }

    public void ResetInventoryUI()
    {
        // 1) Hide all
        foreach (var slot in slots)
            slot.gameObject.SetActive(false);

        // 2) Fill & show the ones you need
        for (int i = 0; i < ownerInventory.items.Count && i < slots.Count; i++)
        {
            slots[i].Setup(ownerInventory.items[i]);

        }

        foreach (var slot in slots)
            slot.gameObject.SetActive(true);

    }
}
