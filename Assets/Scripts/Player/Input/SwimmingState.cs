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
    public SwimmingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();

        verticalInput = 0f;
        if (Keyboard.current.eKey.isPressed) verticalInput = 1;
        if (Keyboard.current.qKey.isPressed) verticalInput = -1f;

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Transform cam = character.cameraTransform;

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        Vector3 up = cam.up;

        Vector3 moveDir = forward * moveInput.y + right * moveInput.x + up * verticalInput;
        moveDir.Normalize();

        Vector3 targetVelocity = moveDir * character.playerSwimSpeed;

        Vector3 futurePosition = character.transform.position + targetVelocity * Time.fixedDeltaTime;

        if (futurePosition.y >= character.waterSurfaceY)
        {
            targetVelocity.y = Mathf.Min(0f, targetVelocity.y);
        }

        character.rb.velocity = targetVelocity;

        float mouseX;
        float mouseY;
        if (lookAction.triggered)
        {
            Vector2 lookInput = lookAction.ReadValue<Vector2>();

            mouseX = lookInput.x * sensitivity * Time.deltaTime;
            mouseY = lookInput.y * sensitivity * Time.deltaTime;

            character.transform.Rotate(Vector3.up * mouseX);

            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);
            character.cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        

        // if (moveDir.sqrMagnitude > 0.01f)
        // {
        //     character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * character.rotationSpeed);
        // }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
