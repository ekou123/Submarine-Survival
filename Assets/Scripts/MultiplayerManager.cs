using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public MultiplayerManager Instance {get; private set;}

    public GameObject playerUIPrefab;
    public GameObject inventoryPrefab;
    public GameObject playerManagerPrefab;
    public GameObject playerCameraPrefab;
    


    private void Awake() 
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;

        // if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.LocalPlayer != null)
        // {
        //     SetupPlayerCharacter();
        // }

        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.SendRate = 30;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        
    }

    public override void OnJoinedRoom()
    {
        // Successfully joined a room, load the game scene
        Debug.Log("Joined in game room");
        SetupPlayerCharacter();

        SetupInteractions();
    }

    public override void OnConnectedToMaster()
    {
        //PhotonNetwork.JoinLobby();
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("We are now connected to " + PhotonNetwork.CloudRegion + " server! Server Name: ");
        PhotonNetwork.AutomaticallySyncScene = true;

        
    }

    public override void OnJoinedLobby()
    {
        //PhotonNetwork.JoinRoom("testroom");
        //SceneManager.LoadScene("JoinAndCreate");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // If joining random failed, create a new room
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 4 });
    }


    private void SetupPlayerCharacter()
    {
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.LocalPlayer != null)
        {
            Debug.Log($"Instantiating Player Character for {PhotonNetwork.LocalPlayer.ActorNumber}");

            GameObject playerCharacterObject = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player"), new Vector3(0, -5f, 0), Quaternion.identity);
            Character playerCharacter = playerCharacterObject.GetComponent<Character>();            
            
            
            
            

            

            //GameObject inventoryObject = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    private void SetupInteractions()
    {
        
    }

}
