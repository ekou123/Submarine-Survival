using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviourPunCallbacks
{
    [SerializeField] float maxInteractingDistance = 10;
    [SerializeField] float interactingRadius = 1;

    LayerMask layerMask;
    Transform cameraTransform;
    InputAction interactAction;

    Vector3 origin;
    Vector3 direction;
    Vector3 hitPosition;
    float hitDistance;

    private float interactCooldown = 0.1f;
    private float lastInteractionTime;

    public Interactable interactableTarget;

    
    void Start()
    {
        cameraTransform = Camera.main.transform;
        layerMask = LayerMask.GetMask("Interactable", "Enemy", "NPC");

        interactAction = GetComponent<PlayerInput>().actions["InteractGameplay"];
        interactAction.performed += Interact;        
    }

    

    // Update is called once per frame
    void Update()
    {
        direction = cameraTransform.forward;
        origin = cameraTransform.position;
        //RaycastHit hit;

        RaycastHit[] hits = Physics.SphereCastAll(cameraTransform.position, interactingRadius, direction, maxInteractingDistance, layerMask);
        
        var seenRoots = new HashSet<GameObject>();
        if (hits.Length > 0)
        {
            RaycastHit closestHit = default;
            float minDist = float.MaxValue;
            Interactable nearestInteractable = null;

            foreach (var h in hits)
            {
                GameObject rootObj = h.transform.root.gameObject;

                if (seenRoots.Contains(rootObj))
                {
                    continue;
                }

                float dist = h.distance;
                if (dist < minDist && h.transform.TryGetComponent<Interactable>(out var possible))
                {
                    minDist = dist;
                    closestHit = h;
                    nearestInteractable = possible;
                }
            }

            if (nearestInteractable != null)
            {
                if (interactableTarget != nearestInteractable && interactableTarget != null)
                {
                    interactableTarget.TargetOff();
                }

                interactableTarget = nearestInteractable;
                interactableTarget.TargetOn();
                return;
            }
        }

        if (interactableTarget != null)
        {
            interactableTarget.TargetOff();
            interactableTarget = null;
        }
        
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        if (Time.time - lastInteractionTime < interactCooldown) return;

        lastInteractionTime = Time.time;

        if (interactableTarget != null)
        {
            if (Vector3.Distance(transform.position, interactableTarget.transform.position) <= interactableTarget.interactionDistance && photonView.IsMine)
            {
                Character myCharacter = GetComponent<Character>();
                if (myCharacter == null)
                {
                    Debug.LogError("Cannot find character to cast interact function");
                }

                interactableTarget.Interact(myCharacter);
            }
            else
            {
                print("Nothing to interact with.");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, origin + direction * hitDistance);
        Gizmos.DrawWireSphere(hitPosition, interactingRadius);
    }

    private void OnDestroy() 
    {
        interactAction.performed -= Interact;        
    }
}
