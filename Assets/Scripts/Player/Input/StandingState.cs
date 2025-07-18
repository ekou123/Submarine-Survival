using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class StandingState : State
{
    float gravityValue;
    bool jump;
    bool crouch;
    public bool inCombat;
    Vector3 currentVelocity;
    bool grounded;
    bool sprint;
    float playerSpeed;
    float sensitivity = 10;
    private float pitch = 0f;
    public float pitchClamp = 80f;

    Vector3 cVelocity;

    public StandingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("Entering Standing State");

        jump = false;
        crouch = false;
        sprint = false;
        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;

        playerSpeed = character.playerBaseSpeed;
        // grounded = character.controller.isGrounded;
        gravityValue = character.gravityValue;

        character.rb.useGravity = true;


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        float mouseX = 0;
        float mouseY = 0;

        if (jumpAction.triggered)
        {
            jump = true;
        }
        if (crouchAction.triggered)
        {
            crouch = true;
        }
        if (sprintAction.triggered)
        {
            sprint = true;
        }
        if (drawWeaponAction.triggered)
        {
            inCombat = true;
        }
        if (lookAction.triggered)
        {
            Vector2 lookInput = lookAction.ReadValue<Vector2>();

            mouseX = lookInput.x * sensitivity * Time.deltaTime;
            mouseY = lookInput.y * sensitivity * Time.deltaTime;

            character.transform.Rotate(Vector3.up * mouseX);

            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);
            character.playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }


        

        // Smoothly rotate character toward movement direction
        // if (moveDir.sqrMagnitude > 0.01f)
        // {
        //     Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        //     character.transform.rotation = Quaternion.Slerp(character.transform.rotation, targetRotation, Time.deltaTime * character.rotationSpeed);
        // }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        //character.SetAnimatorSpeed(input.magnitude, character.speedDampTime);

        if (sprint)
        {
            stateMachine.ChangeState(character.sprinting);
        }
        if (jump)
        {
            
            stateMachine.ChangeState(character.jumping);
        }
        if (inCombat)
        {
            stateMachine.ChangeState(character.combatting);
            //character.animator.SetTrigger("attack1");
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // Read movement input
        input = moveAction.ReadValue<Vector2>();

        // Get camera-relative directions
        Vector3 camForward = character.playerCamera.transform.forward;
        Vector3 camRight = character.playerCamera.transform.right;

        // Flatten camera direction (ignore vertical)
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

    // Compute movement direction relative to camera
        Vector3 moveDir = camRight * input.x + camForward * input.y;

        // Save as velocity
        velocity = moveDir;
    }

    public override void Exit()
    {
        base.Exit();

        gravityVelocity.y = 0f;
        character.playerVelocity = new Vector3(input.x, 0, input.y);

        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.LookRotation(velocity);
        }

    }
}
