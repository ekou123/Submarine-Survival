using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

public class WaterTrigger : MonoBehaviourPunCallbacks
{
    public bool playerInWater = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        var playerCharacter = other.GetComponent<Character>();
        if (playerCharacter == null)
        {
            Debug.LogError("[WaterTrigger] Player has no Character Component");
            return;
        }

        playerCharacter.movementSM.ChangeState(playerCharacter.swimming);
    }

    private void OnTriggerExit(Collider other)
    {
        // 1) Only care about Player‐tagged objects
        if (!other.CompareTag("Player")) return;

        // 2) Only for our local player
        var pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        // 3) Grab the Character & switch back to Standing
        var playerCharacter = other.GetComponent<Character>();
        if (playerCharacter != null)
            playerCharacter.movementSM.ChangeState(playerCharacter.standing);
    }


}
