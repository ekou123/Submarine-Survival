using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableNameText : MonoBehaviour
{
    private TextMeshProUGUI text;
    [SerializeField] private Camera playerCamera;

    
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        if (text == null)
        {
            Debug.LogError("Could not find Text Component on InteractableNameText Object Children");
        }

        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogError("Could not find Main Camera");
            return;
        }
        
        HideText();
    }

    public void ShowText(Interactable interactable)
    {
        if (interactable is PickupItem)
        {
            text.text = interactable.interactableName + "\n [E] Pick Up";
        }
        else if (interactable is InteractableChest)
        {
            if (((InteractableChest)interactable).isOpen)
            {
                text.text = interactable.interactableName + "\n [E] Close";
            }
            else
            {
                text.text = interactable.interactableName + "\n [E] Open";
            }
        }
        else if (interactable is InteractableLoot)
        {
            text.text = interactable.interactableName + "\n [E] Loot";
        }
        else if (interactable is InteractableNPC)
        {
            text.text = interactable.interactableName + " \n [E] Speak";
        }
        else
        {
            text.text = interactable.interactableName;
        }
    }

    public void HideText()
    {
        text.text = "";
    }

    public void SetInteractableNamePosition(Interactable interactable)
    {
        if (interactable.TryGetComponent(out BoxCollider boxCollider))
        {
            transform.position = interactable.transform.position + Vector3.up * boxCollider.bounds.size.y;
            transform.LookAt(2 * transform.position - playerCamera.transform.position);
        }
        else if (interactable.TryGetComponent(out CapsuleCollider capsuleCollider))
        {
            transform.position = interactable.transform.position + Vector3.up * capsuleCollider.height;
            transform.LookAt(2 * transform.position - playerCamera.transform.position);
        }
        else
        {
            print("Error, no collider found!");
        }
    }
}
