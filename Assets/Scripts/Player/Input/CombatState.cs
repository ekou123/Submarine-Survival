using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CombatState : State
{
    float gravityValue;
    Vector3 currentVelocity;
    bool grounded;
    float playerSpeed;
    bool sheathWeapon;
    Vector3 cVelocity;

    public CombatState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        sheathWeapon = false;
        input = Vector2.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;

        playerSpeed = -character.playerBaseSpeed;
        velocity = character.playerVelocity;
        grounded = character.controller.isGrounded;
        gravityValue = character.gravityValue;
    }

    public override void HandleInput()
    {
        base.HandleInput();

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
        character.standing.inCombat = false;
    }
}
