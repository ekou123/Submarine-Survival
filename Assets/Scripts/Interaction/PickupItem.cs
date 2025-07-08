using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PickupItem : Interactable
{
    [Header("Item Data")]
    [SerializeField] string itemName;
    public ItemData item;

    private Rigidbody rb;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        interactableName = itemName;

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // rb.useGravity = true;
            rb.isKinematic = false;
        }
    }

    protected override void Interaction()
    {
        base.Interaction();

        
        Inventory playerInventory = characterInteracting .GetComponent<Inventory>();

        if (playerInventory != null)
        {
            if (playerInventory.AddItem(item))
            {
                print("I put " + itemName + " in my inventory");
                //photonView.RPC("RPC_DestroyObject", RpcTarget.All);
                //OnPickupAttempt(PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else
            {
                print("Inventory is full");
            }
        }
        else
        {
            Debug.LogWarning("No inventory found on player");
            return;
        }

    }


    [PunRPC]
    public void RPC_DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
