using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using System;

public class NetworkManager : Photon.MonoBehaviour
{

    private const string roomName = "RoomName";
    private TypedLobby lobbyName = new TypedLobby("New_Lobby", LobbyType.Default);
    private RoomInfo[] roomsList;
    public GameObject player;
    public GameObject standbyCamera;

    
    //Map sync
    bool sent;
    int seed;


    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("v4.2");
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

    void OnGUI()
    {
        if (!PhotonNetwork.connected)
        {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        }
        else if (PhotonNetwork.room == null)
        {
            // Create Room
            if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
            {
                PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 2, IsOpen = true, IsVisible = true }, lobbyName);
            }

            // Join Room
            if (roomsList != null)
            {
                for (int i = 0; i < roomsList.Length; i++)
                {
                    if (GUI.Button(new Rect(100, 250 + (110 * i), 250, 100), "Join " + roomsList[i].Name))
                    {
                        PhotonNetwork.JoinRoom(roomsList[i].Name);
                    }
                }
            }
        }
       
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
        Vector3 initialSpawnPoint = new Vector3((-worldGen.GetComponent<MazeGenerator>().Width / 2) + worldGen.GetComponent<MazeGenerator>().wallLength / 2, 0.0f, (-worldGen.GetComponent<MazeGenerator>().Height / 2) + worldGen.GetComponent<MazeGenerator>().wallLength / 2);
        initialSpawnPoint += new Vector3(-1f, worldGen.GetComponent<MazeGenerator>().wallLength, 0f);
        GameObject myPlayer = PhotonNetwork.Instantiate(player.name, initialSpawnPoint, Quaternion.identity, 0); // spawneaza la toti
        myPlayer.transform.Find("FirstPersonCharacter").gameObject.SetActive(true);
        myPlayer.GetComponent<FirstPersonController>().enabled = true;
        //AM MODIFICAT CEVA
    }
}