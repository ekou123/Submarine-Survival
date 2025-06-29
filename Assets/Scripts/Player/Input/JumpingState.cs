using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : State
{
    public bool grounded;

    float gravityValue;
    float jumpHeight;
    float playerSpeed;

    Vector3 airVelocity;

    public JumpingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }


    public override void Enter()
    {
        base.Enter();
        

        Jump();
    }

    public override void HandleInput()
    {
        base.HandleInput();

        input = moveAction.ReadValue<Vector2>();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void Exit()
    {
        base.Exit();
    }

    void Jump()
    {
        gravityVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }
}
