using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    private ItemData heldItem;

    public void Setup(Sprite itemSprite, ItemData item)
    {
        icon.sprite = itemSprite;
        icon.enabled = itemSprite != null;
        heldItem = item;
    }
}
