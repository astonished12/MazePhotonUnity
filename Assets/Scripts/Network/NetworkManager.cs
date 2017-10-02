using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using System;

public class NetworkManager : Photon.MonoBehaviour
{

    private const string roomName = "RoomName";
    private  static TypedLobby lobbyName = new TypedLobby("New_Lobby", LobbyType.Default);
    public static RoomInfo[] roomsList;
    public GameObject player;
    public GameObject standbyCamera;

    public bool offlinemode = false;
    
    //Map sync
    bool sent;
    int seed;


    void Start()
    {
        Connect();
    }

    public void Connect()
    {
        if (offlinemode)
        {
            PhotonNetwork.offlineMode = true;
            OnJoinedLobby();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings("v4.3");
        }
    }
    public GameObject worldGen;


    //TO DO ONE TIME ACTIVATION OF MASTER CLIENT MAZE GENERATION WHEN OTHER MASTER GOING OUT:))
    void Update()
    {
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient && !sent)
        {
            // If i'm in a room, the master client, and i haven't already sent the seed
            sent = true;
            seed = Guid.NewGuid().GetHashCode();
            worldGen.GetComponent<MazeGenerator>().realSeed = seed;
            worldGen.GetComponent<PhotonView>().RPC("ReceiveSeed", PhotonTargets.OthersBuffered, seed);
            //Others buffered means that anyone who joins later will get this RPC

        }
    }

  

    public static void CreateRoom(string roomName,int _maxPlayers)
    {
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = (byte)_maxPlayers, IsOpen = true, IsVisible = true }, lobbyName);
    }

    public static void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
         

    void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(lobbyName);
    }

    void OnReceivedRoomListUpdate()
    {
        Debug.Log("Room was created");
        roomsList = PhotonNetwork.GetRoomList();
    }

    void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    void OnJoinedRoom()
    {
        Debug.Log("Connected to Room");
        //CREATE SPAWN POINTS
        standbyCamera.SetActive(false);
        Vector3 initialSpawnPoint = new Vector3((-worldGen.GetComponent<MazeGenerator>().Width / 2+1f) + worldGen.GetComponent<MazeGenerator>().wallLength / 2, 0.0f, (-worldGen.GetComponent<MazeGenerator>().Height / 2) + worldGen.GetComponent<MazeGenerator>().wallLength / 2);
        GameObject myPlayer = PhotonNetwork.Instantiate(player.name, initialSpawnPoint, Quaternion.identity, 0); // spawneaza la toti
        myPlayer.transform.Find("FirstPersonCharacter").gameObject.SetActive(true);
        myPlayer.transform.Find("Crosshair").gameObject.SetActive(true);
        myPlayer.GetComponent<FirstPersonController>().enabled = true;
        myPlayer.GetComponent<PlayerMovement>().enabled = true;
        myPlayer.GetComponent<NetworkCharacter>().enabled = true;


        //AM MODIFICAT CEVA
    }
}