using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

public class Interactable : MonoBehaviourPunCallbacks
{
    [Header("Interaction Data")]
    public string interactableName = "";
    public float interactionDistance = 2;
    [SerializeField] bool isInteractable = true;

    public Character characterInteracting;

    [SerializeField] InteractableNameText interactableNameText;
    // public GameObject interactableNameCanvas;


    public virtual void Start()
    {
        if (interactableNameText == null)
            interactableNameText = FindObjectOfType<InteractableNameText>();
        
    }

    public void TargetOn()
    {
        if (interactableNameText == null) return;
        interactableNameText.ShowText(this);
        interactableNameText.SetInteractableNamePosition(this);
    }

    public void TargetOff()
    {
        if (interactableNameText == null) return;
        interactableNameText.HideText();
    }

    public void Interact(Character character)
    {
        characterInteracting = character;
        if (isInteractable) Interaction();
    }

    protected virtual void Interaction()
    {

    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }

    private void OnDestroy() 
    {
        TargetOff();
    }
}
