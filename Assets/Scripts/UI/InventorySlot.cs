using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    private ItemData heldItem;

    public void Setup(ItemData item)
    {
        
        icon.sprite = item.iconSprite;
        icon.enabled = item.iconSprite != null;
        heldItem = item;
    }
}
