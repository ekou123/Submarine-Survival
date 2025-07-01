using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class Character : MonoBehaviour {
    public static Character Instance {get; private set;}
    
    [Header("PlayerObject")]
    public Transform orientation;
    public Canvas inventoryCanvas;
    public Transform cameraTransform;
    public event Action<Character> OnCharacterInitialized;

    

    [Header("Controls")]
    public float playerSpeed;
    public float playerBaseSpeed = 5.0f;
    public float playerBaseSwimSpeed = 3f;
    public float playerSwimSpeed;
    public float crouchSpeed = 2.0f;
    public float sprintSpeed = 7.0f;
    public float jumpHeight = 0.8f;
    public float gravityMultiplier = 2;
    public float rotationSpeed = 5f;
    public float crouchColliderHeight = 1.35f;
    
    [Header("Weapon Config")]
    public GameObject currentItemInHandPrefab;

    [Header("Animation Smoothing")]
    [Range(0,1)]
    public float speedDampTime = 0.1f;
    [Range(0,1)]
    public float velocityDampTime = 0.9f;
    [Range(0,1)]
    public float rotationDampTime = 0.2f;
    [Range(0,1)]
    public float airControl = 0.5f;

    public StateMachine movementSM;
    public StandingState standing;
    public JumpingState jumping;
    public SprintingState sprinting;
    public SprintJumpState sprintJumping;
    public CombatState combatting;
    public AttackingState attacking;
    public SwimmingState swimming;

    [HideInInspector]
    public float gravityValue = -9.81f;
    [HideInInspector]
    public float normalColliderHeight;
    [HideInInspector]
    public CharacterController controller;
    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public Vector3 playerVelocity;
    [HideInInspector]
    public float waterSurfaceY = 0f; 
    
    
    // public CinemachineFreeLook cinemachineFreeLook; 
    // Start is called before the first frame update

    private void Awake()
    {

        //     if (photonView.IsMine)
        //     {
        //         GetComponent<PlayerInput>().enabled = true;
        //         this.enabled = true;
        //     }
        //     else
        //     {
        //         GetComponent<PlayerInput>().enabled = false;
        //     }



        controller = GetComponent<CharacterController>();
        // animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        // cameraTransform = Camera.main.transform;
        rb.freezeRotation = true;

        // cinemachineFreeLook = PlayerCamera.Instance.cinemachineFreeLook;

        //     SetupCameraSettings();



        movementSM = new StateMachine();
        standing = new StandingState(this, movementSM);
        swimming = new SwimmingState(this, movementSM);
        //     jumping = new JumpingState(this, movementSM);
        //     sprinting = new SprintingState(this, movementSM);
        //     sprintJumping = new SprintJumpState(this, movementSM);
        //     combatting = new CombatState(this, movementSM);
        //     attacking = new AttackingState(this, movementSM);
        movementSM.Initialize(swimming);

        //     playerSpeed = playerBaseSpeed;


        //     //currentWeaponInHand = GetComponentInChildren<WeaponItem>();

        //     normalColliderHeight = controller.height;
        //     gravityValue *= gravityMultiplier;

    }
    void Start()
    {
        playerSwimSpeed = playerBaseSwimSpeed;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        movementSM.currentState.HandleInput();

        movementSM.currentState.LogicUpdate();        
    }

    private void FixedUpdate() 
    {
        movementSM.currentState.PhysicsUpdate();
    }
}
