using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwimmingState : State
{
    Vector2 moveInput;
    Vector2 lookInput;
    float verticalInput;
    float sensitivity = 10;
    private float pitch = 0f;
    public float pitchClamp = 80f;
    private float buoyancyStrength = 0.5f;
    private float dampingStrength = 0.8f;
    private float dropVelocity;
    private bool isDropping;
    public SwimmingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        character.rb.useGravity = false;

        dropVelocity = character.rb.velocity.y;
        isDropping = dropVelocity < 0f;
    }

    public override void HandleInput()
    {
        base.HandleInput();
        lookInput = lookAction.ReadValue<Vector2>();

        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;

        // 1) Yaw around world-up (turn left/right)
        character.transform.Rotate(Vector3.up, mouseX, Space.World);

        // 2) Pitch around the character's local right
        character.transform.Rotate(character.transform.right, -mouseY, Space.World);

        pitch = Mathf.Clamp(pitch - mouseY, -pitchClamp, pitchClamp);
        character.playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        moveInput = moveAction.ReadValue<Vector2>();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 3) Build move direction from camera forward/right
        Transform cam = character.playerCamera.transform;
        Vector3 camFwd = cam.forward.normalized;
        Vector3 camRight = cam.right.normalized;
        
        Vector3 moveDir = (camRight * moveInput.x + camFwd * moveInput.y).normalized;

        // 4) Horizontal velocity
        Vector3 horizontalVel = new Vector3(
            moveDir.x * character.playerSwimSpeed,
            0f,
            moveDir.z * character.playerSwimSpeed
        );

        // 5) Vertical drop-then-swim logic (unchanged)
        float finalY = isDropping
            ? DampenDropVelocity()
            : moveDir.y * character.playerSwimSpeed;

        // 6) Clamp at surface
        if (character.transform.position.y >= character.waterSurfaceY && finalY > 0f)
            finalY = 0f;

        // 7) Apply
        character.rb.velocity = horizontalVel + Vector3.up * finalY;
    }

    public override void Exit()
    {
        base.Exit();
    }
    
    private float DampenDropVelocity()
    {
        dropVelocity = Mathf.Lerp(dropVelocity, 0f, dampingStrength * Time.fixedDeltaTime);
        if (dropVelocity >= -0.1f) isDropping = false;
        return dropVelocity;
    }
}
