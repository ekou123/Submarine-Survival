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

        var cam    = character.playerCamera.transform;
        Vector3 rawDir   = cam.right * moveInput.x + cam.forward * moveInput.y;
        Vector3 swimDir  = rawDir;
        if (rawDir.sqrMagnitude > 1f) swimDir = rawDir.normalized;

        // 2) Compose horizontal and vertical speeds
        Vector3 horizontalVel = new Vector3(
        swimDir.x * character.playerSwimSpeed,
        0f,
        swimDir.z * character.playerSwimSpeed
        );

        // 3) vertical from drop or swimDir
        float finalY = isDropping
            ? DampenDropVelocity()
            : swimDir.y * character.playerSwimSpeed;

        // 4) optional explicit up/down keys
        if (verticalInput != 0f)
            finalY = verticalInput * character.playerSwimSpeed;

        // 5) clamp at surface
        if (character.transform.position.y >= character.waterSurfaceY && finalY > 0f)
            finalY = 0f;

        Vector3 worldVel = horizontalVel + Vector3.up * finalY;

        // 6) bank & camera‐logic (unchanged)
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
        // Apply the roll on the model pivot
        character.modelPivot.localRotation = Quaternion.Euler(0f, 0f, smoothedZ);

        // 7) move the rigidbody
        character.rb.velocity = horizontalVel + Vector3.up * finalY;

        
        // 6) Finally, apply the velocity to the Rigidbody
        // character.rb.velocity = worldVel;
        
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
