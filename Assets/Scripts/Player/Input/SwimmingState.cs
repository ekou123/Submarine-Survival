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
    private bool  isDropping;
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

        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();

        verticalInput = 0f;
        if (moveAction.triggered)
        {
            verticalInput = moveInput.y;
        }

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 1) Mouse look (yaw + pitch)
        Vector2 look  = lookAction.ReadValue<Vector2>();
        float mouseX  = look.x * sensitivity * Time.deltaTime;
        float mouseY  = look.y * sensitivity * Time.deltaTime;

        character.transform.Rotate(Vector3.up * mouseX);
        pitch = Mathf.Clamp(pitch - mouseY, -pitchClamp, pitchClamp);
        character.cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // 2) Read WASD move input
        Vector2 move2D = moveAction.ReadValue<Vector2>();

        // 3) Build a single moveDir from the pitched camera
        Transform cam = character.cameraTransform;
        Vector3 camFwd = cam.forward.normalized;
        Vector3 camRight = cam.right.normalized;

        Vector3 moveDir = (camRight * move2D.x + camFwd * move2D.y).normalized;

        // 4) Horizontal velocity (X/Z)
        Vector3 horizontalVel = new Vector3(
            moveDir.x * character.playerSwimSpeed,
            0f,
            moveDir.z * character.playerSwimSpeed
        );

        // 5) Vertical velocity: first drop + damp, then swim via moveDir.y
        float finalY;
        if (isDropping)
        {
            // Dampen the initial drop velocity toward zero
            dropVelocity = Mathf.Lerp(dropVelocity, 0f, dampingStrength * Time.fixedDeltaTime);
            finalY = dropVelocity;

            if (dropVelocity >= -0.1f)
                isDropping = false;
        }
        else
        {
            // After drop, vertical movement comes from camera-forward pitch
            finalY = moveDir.y * character.playerSwimSpeed;
        }

        // 6) Clamp so you can’t poke above the water surface
        if (character.transform.position.y >= character.waterSurfaceY && finalY > 0f)
            finalY = 0f;

        // 7) Apply combined velocity
        character.rb.velocity = horizontalVel + Vector3.up * finalY;
    }

    public override void Exit()
    {
        base.Exit();
    }
}
