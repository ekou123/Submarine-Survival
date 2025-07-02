using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class InteractableLoot : Interactable
{
    [Header("UI References")]
    public GameObject lootCanvas;
    public GameObject lootItemButtonPrefab;
    public Transform lootListParent;
    public TextMeshProUGUI itemNameText;
    public bool lootMenuOpen;

    public List<Item> droppedItems;
    public delegate void LootAction(Item item);
    public event LootAction onLootItem;

    public override void Start()
    {
        base.Start();
    }

    private void Update() 
    {
    }

    protected override void Interaction()
    {
        base.Interaction();
    
    }
    
    void OpenLoot()
    {

    }

    void TriggerLoot(bool openLoot)
    {
        if (openLoot)
        {

        }
        else
        {
            
        }
        
        
    }

    void DisableCollider()
    {
        if (TryGetComponent(out BoxCollider boxCollider))
        {
            boxCollider.enabled = false;
        }
        else if (TryGetComponent(out CapsuleCollider capsuleCollider))
        {
            capsuleCollider.enabled = false;
        }
        else
        {
            Debug.LogError("Error, not collider found!");
            return;
        }     
    }
}
