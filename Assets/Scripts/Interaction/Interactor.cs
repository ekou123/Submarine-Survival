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
    [SerializeField] Transform cameraTransform;

    LayerMask layerMask;
    
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
        Character character = GetComponent<Character>();
        layerMask = LayerMask.GetMask("Interactable", "Enemy", "NPC");

        interactAction = GetComponent<PlayerInput>().actions["InteractGameplay"];
        interactAction.performed += Interact;        
    }



    // Update is called once per frame
    // void Update()
    // {
    //     direction = cameraTransform.forward;
    //     origin = cameraTransform.position;
    //     //RaycastHit hit;

    //     RaycastHit[] hits = Physics.SphereCastAll(cameraTransform.position, interactingRadius, direction, maxInteractingDistance, layerMask);

    //     var seenRoots = new HashSet<GameObject>();
    //     if (hits.Length > 0)
    //     {
    //         RaycastHit closestHit = default;
    //         float minDist = float.MaxValue;
    //         Interactable nearestInteractable = null;

    //         foreach (var h in hits)
    //         {
    //             GameObject rootObj = h.transform.root.gameObject;

    //             if (seenRoots.Contains(rootObj))
    //             {
    //                 continue;
    //             }

    //             float dist = h.distance;
    //             if (dist < minDist && h.transform.TryGetComponent<Interactable>(out var possible))
    //             {
    //                 minDist = dist;
    //                 closestHit = h;
    //                 nearestInteractable = possible;
    //             }
    //         }

    //         if (nearestInteractable != null)
    //         {
    //             if (interactableTarget != nearestInteractable && interactableTarget != null)
    //             {
    //                 interactableTarget.TargetOff();
    //             }

    //             interactableTarget = nearestInteractable;
    //             interactableTarget.TargetOn();
    //             return;
    //         }
    //     }

    //     if (interactableTarget != null)
    //     {
    //         interactableTarget.TargetOff();
    //         interactableTarget = null;
    //     }

    // }
    
    void Update()
{
    var cam = cameraTransform; 
    Ray ray = new Ray(cam.position, cam.forward);
    if (Physics.Raycast(ray, out RaycastHit hit, maxInteractingDistance, layerMask))
    {
        // grab the Interactable on that collider or any parent
        var target = hit.collider.GetComponentInParent<Interactable>();
        if (target != null)
        {
            if (interactableTarget != target)
            {
                // we’re looking at a new one
                if (interactableTarget != null)
                    interactableTarget.TargetOff();

                interactableTarget = target;
                interactableTarget.TargetOn();   // e.g. shows your “Press E to pick up” text
            }
            return;
        }
    }

    // if we fall through, nothing valid under the crosshair
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
            if (Vector3.Distance(transform.position, interactableTarget.transform.position) <= interactableTarget.interactionDistance)
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
