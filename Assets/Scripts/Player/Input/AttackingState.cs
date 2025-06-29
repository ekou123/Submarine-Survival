using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;

public class AttackingState : State
{
    float timePassed;
    float clipLength;
    float clipSpeed;
    public bool canUseNextAttack;

    float timeSinceLastAttack;

    float gravityValue;
    public bool inCombat;
    Vector3 currentVelocity;
    bool grounded;
    float playerSpeed;

    Vector3 cVelocity;

    public AttackingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        canUseNextAttack = false;
        character.animator.applyRootMotion = true;
        timePassed = 0f;
        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;
        canUseNextAttack = false;

        playerSpeed = character.playerBaseSpeed;
        grounded = character.controller.isGrounded;
        gravityValue = character.gravityValue;
       
        //character.animator.SetTrigger("attack");  
        //character.photonView.RPC("RPC_SetTrigger", RpcTarget.Others, "attack", PhotonNetwork.LocalPlayer.ActorNumber);
        //character.photonView.RPC("RPC_PlayAnimation", RpcTarget.Others, character.animator.GetCurrentAnimatorStateInfo(1).fullPathHash, character.animator.GetCurrentAnimatorStateInfo(1).normalizedTime);
        //canUseNextAttack = true;     
        //character.photonView.RPC("RPC_PlayAnimation", RpcTarget.Others, character.animator.GetCurrentAnimatorStateInfo(1).fullPathHash, character.animator.GetCurrentAnimatorStateInfo(1).normalizedTime);
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
        character.animator.applyRootMotion = false;
    }
}
