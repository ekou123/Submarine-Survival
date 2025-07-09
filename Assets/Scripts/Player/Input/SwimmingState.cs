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
    private float bankVelocity = 0f;
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

        character.rb.isKinematic = false;  
        character.rb.useGravity = false;

        dropVelocity = character.rb.velocity.y;
        isDropping = dropVelocity < 0f;
    }

    public override void HandleInput()
    {
        base.HandleInput();
        Vector2 look = lookAction.ReadValue<Vector2>();
        float mouseX = look.x * sensitivity * Time.deltaTime;
        float mouseY = look.y * sensitivity * Time.deltaTime;

        // — YAW on the player root (so camera & physics face into the turn) —
        character.transform.Rotate(Vector3.up, mouseX, Space.World);

        // — INVERSE‐YAW on the visual model so it “leans back” or “lags behind” —
        if (character.modelPivot != null)
            character.modelPivot.Rotate(Vector3.up, -mouseX, Space.Self);

        // (Optionally do the same for pitch/roll:)
        //  e.g. character.modelPivot.Rotate(character.modelPivot.right, mouseY, Space.Self);

        // … your existing pitch on the camera pivot …
        float pitchDelta = mouseY;
        pitch = Mathf.Clamp(pitch - pitchDelta, -pitchClamp, pitchClamp);
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
        Transform cam = character.cameraPivot != null
        ? character.cameraPivot
        : character.playerCamera.transform;
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

        Vector3 worldVel = horizontalVel + Vector3.up * finalY;

        // —————— BANKING: tilt the visual model into turn ——————
        // Convert that velocity into local space so x is “sideways”
        Vector3 localVel = character.transform.InverseTransformDirection(worldVel);

        // Target roll: proportional to sideways speed
        float targetBank = -localVel.x
            / character.playerSwimSpeed
            * character.maxBankAngle;

        // Unwrap current Z-angle to ±180
        float currentZ = character.modelPivot
        .localEulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;

        // Smoothly move toward the target bank
        float smoothedZ = Mathf.SmoothDamp(
            currentZ,
            targetBank,
            ref bankVelocity,
            character.bankSmoothTime,
            Mathf.Infinity,
            Time.fixedDeltaTime
        );

        Debug.Log("World Velocity: " + worldVel);
        // Apply the roll on the model pivot
        character.modelPivot.localRotation = Quaternion.Euler(0f, 0f, smoothedZ);
        // 6) Finally, apply the velocity to the Rigidbody
        character.rb.velocity = worldVel;
        
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
