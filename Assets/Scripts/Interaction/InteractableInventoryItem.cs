using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableInventoryItem : Interactable
{
    private Animator animator;

    [Header("Locked Chest Options")]
    public bool isLocked;
    public string chestID;
    public bool isOpen;

    public override void Start()
    {
        base.Start();
        isOpen = false;
    }

    protected override void Interaction()
    {
        base.Interaction();

        if (!isLocked)
        {
            if (!isOpen)
            {
                OpenChest();
                print("Opening chest.");
            }
            else
            {
                CloseChest();
                print("Closing chest.");
            }
        }
    }

    void OpenChest()
    {
        animator.SetTrigger("openChest");
        isOpen = !isOpen;
    }

    void CloseChest()
    {
        animator.SetTrigger("closeChest");
        isOpen = !isOpen;
    }

}
