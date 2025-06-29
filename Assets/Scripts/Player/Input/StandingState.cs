using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        grounded = character.controller.isGrounded;
        gravityValue = character.gravityValue;
    }

    public override void HandleInput()
    {
        base.HandleInput();

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

        input = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(input.x, 0, input.y);

        // Vector3 inputDir = character.orientation.forward * input.x + character.orientation.right * input.y;
            

        // if (inputDir != Vector3.zero)
        // {
        //     character.transform.forward = Vector3.Slerp(character.transform.forward, inputDir.normalized, Time.deltaTime * character.rotationSpeed);
        // }

         Vector3 camForward = character.cameraTransform.forward;
        Vector3 camRight = character.cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;

        //velocity = velocity.x * character.cameraTransform.right.normalized + velocity.z * character.cameraTransform.forward.normalized;
        velocity = velocity.x * camRight.normalized + velocity.z * camForward.normalized;

        velocity.y = 0f;

        
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

        gravityVelocity.y += gravityValue * Time.deltaTime;

        
        if (grounded)
        {
            gravityVelocity.y = 0f;
        }

        Vector3 rbVelocity = character.rb.velocity;
        float yVelocity = rbVelocity.y;

        Vector3 horizontalVelocity = velocity * playerSpeed;
        

        Vector3 finalVelocity = new Vector3(horizontalVelocity.x, yVelocity, horizontalVelocity.z);

        character.rb.velocity = finalVelocity;


        Vector3 flattened = new Vector3(finalVelocity.x, 0f, finalVelocity.z);
        if (flattened.sqrMagnitude >= 0.01f)
        {
            character.transform.forward = flattened.normalized;
        }

        //currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, character.velocityDampTime);

        //character.controller.Move((currentVelocity * playerSpeed + gravityVelocity) * Time.deltaTime);

        //grounded = character.controller.isGrounded;


        // if (character.photonView.IsMine)
        // {
        //     if (currentVelocity.sqrMagnitude > 0)
        //     {
        //         character.transform.forward = currentVelocity.normalized;
        //     }
        // }
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
