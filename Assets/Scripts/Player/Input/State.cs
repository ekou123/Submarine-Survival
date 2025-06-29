using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class State
{
    public Character character;
    public StateMachine stateMachine;
    bool inventoryOpen;
    bool lootMenuOpen;

    protected Vector3 gravityVelocity;
    protected Vector3 velocity;
    protected Vector2 input;

    public InputAction interactAction;
    public InputAction interactUIAction;
    public InputAction moveAction;
    public InputAction lookAction;
    public InputAction jumpAction;
    public InputAction sprintAction;
    public InputAction crouchAction;
    public InputAction drawWeaponAction;
    public InputAction attackAction;
    public InputAction inventoryAction;
    public InputAction switchItemAction;
    public InputAction dropItemAction;
    public InputAction selectUIAction;
    public InputAction openClassSkillsUIAction;
    public InputAction exitUIAction;

    

    public State(Character _character, StateMachine _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;

        moveAction = character.playerInput.actions["Move"];
        jumpAction = character.playerInput.actions["Jump"];
        lookAction = character.playerInput.actions["Look"];
        sprintAction = character.playerInput.actions["Sprint"];
        crouchAction = character.playerInput.actions["Crouch"];
        drawWeaponAction = character.playerInput.actions["DrawWeapon"];
        attackAction = character.playerInput.actions["Attack1"];
        inventoryAction = character.playerInput.actions["Inventory"];
        interactAction = character.playerInput.actions["InteractGameplay"];
        interactUIAction = character.playerInput.actions["InteractUI"];
        dropItemAction = character.playerInput.actions["DropItem"];
        selectUIAction = character.playerInput.actions["SelectUI"];
        exitUIAction = character.playerInput.actions["ExitUI"];
        openClassSkillsUIAction = character.playerInput.actions["OpenClassSkills"];
        //SetupSlotBinds();
    }

    private void SetupSlotBinds()
    {
    
    }

    private void SwitchItem(InputAction.CallbackContext context)
    {
        
        
    }

    public virtual void Enter()
    {
        Debug.Log("Enter state: " + this.ToString());
    }

    private void OnEnable() 
    {
        //switchItemAction
    }

    public virtual void HandleInput()
    {
    
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        
        
    }

    public virtual void Exit()
    {

    }
}
