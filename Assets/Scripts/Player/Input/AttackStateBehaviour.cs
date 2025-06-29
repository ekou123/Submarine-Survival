using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateBehaviour : StateMachineBehaviour
{

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) 
    {
        // //Debug.Log("Running OnStateExit");
        // Character character = animator.GetComponent<Character>();
        // if (character != null)
        // {
            
        //     character.animator.SetTrigger("move");
        //     character.movementSM.ChangeState(character.combatting);
        // }        
    }
}
