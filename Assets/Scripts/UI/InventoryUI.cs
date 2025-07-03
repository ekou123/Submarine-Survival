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

    private Character owner;
    private readonly List<ItemData> items = new List<ItemData>();
    private readonly List<InventorySlot> slots = new List<InventorySlot>();

    private void Start()
    {
        Character playerCharacter = PlayerUI.Instance.character;
        if (playerCharacter == null)
        {
            Debug.LogError("[InventoryUI] Could not find Player Component in PlayerUI");
            return;
        }

        owner = playerCharacter;
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

    public void ResetInventoryUI()
    {
        Inventory playerInventory = owner.GetComponent<Inventory>();
        if (playerInventory == null)
        {
            Debug.LogError("[InventoryUI] Could not find Inventory Component on Character");
            return;
        }

        slots.Clear();

        for (int i = 0; i < playerInventory.maxInventorySlots; i++)
        {
            var go = Instantiate(slotPrefab, slotContainer);
            if (playerInventory.items[i] != null)
            {
                ItemData itemToShow = playerInventory.items[i];
                var slot = go.GetComponent<InventorySlot>();
                slot.Setup(itemToShow.iconSprite, itemToShow);
            }
        }
    }
}
