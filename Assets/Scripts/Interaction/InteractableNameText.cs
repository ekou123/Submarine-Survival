using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableNameText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Transform cameraTransform;

    
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        Character playerCharacter = PlayerUI.Instance.character;
        if (!playerCharacter)
        {
            Debug.LogError("Could not find Character component");
            return;
        }

        cameraTransform = playerCharacter.cameraTransform;
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
            transform.LookAt(2 * transform.position - cameraTransform.position);
        }
        else if (interactable.TryGetComponent(out CapsuleCollider capsuleCollider))
        {
            transform.position = interactable.transform.position + Vector3.up * capsuleCollider.height;
            transform.LookAt(2 * transform.position - cameraTransform.position);
        }
        else
        {
            print("Error, no collider found!");
        }
    }
}
