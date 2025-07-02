using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WaterTrigger : MonoBehaviour
{
    public bool playerInWater = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInWater = true;
            Character playerCharacter = other.GetComponent<Character>();
            if (playerCharacter == null)
            {
                Debug.LogError("Could not find Character Component on Player");
                return;
            }

            playerCharacter.movementSM.ChangeState(playerCharacter.swimming);

        }
    }

    private void OerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInWater = false;
            Debug.Log("Player exited water");
        }
    }


}
