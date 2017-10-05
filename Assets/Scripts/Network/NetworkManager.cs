﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using System;
using System.Collections.Generic;

public class NetworkManager : Photon.MonoBehaviour
{

    private const string roomName = "RoomName";
    private  static TypedLobby lobbyName = new TypedLobby("New_Lobby", LobbyType.Default);
    public static RoomInfo[] roomsList;
    public GameObject player;
    public  static GameObject standbyCamera;
    public GameObject exit;
    public GameObject waitPanel;
    private GameObject waitPanelInitilized;
    public bool offlinemode = false;
    public static Dictionary<string,int>  mapRoomNameSize = new Dictionary<string, int>();

    private bool gamestart=false;
    private Vector3 hiddenPosition = new Vector3(-200f, -200f, -200f);
    //Map sync
    bool sent;
    int seed;

    private void Awake()
    {
        standbyCamera = GameObject.FindGameObjectWithTag("MainCamera");
     }
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
            worldGen.GetComponent<MazeGenerator>().realSize = mapRoomNameSize[PhotonNetwork.room.Name];

            worldGen.GetComponent<PhotonView>().RPC("ReceiveSeed", PhotonTargets.OthersBuffered, seed);
            worldGen.GetComponent<PhotonView>().RPC("ReceiveSize", PhotonTargets.OthersBuffered, mapRoomNameSize[PhotonNetwork.room.Name]);

            //Others buffered means that anyone who joins later will get this RPC
          

        }
        if (PhotonNetwork.inRoom && PhotonNetwork.room.PlayerCount == PhotonNetwork.room.MaxPlayers && gamestart ==false)
        {
            Destroy(waitPanelInitilized);
            gamestart = true;
            StartCoroutine(GameIsReadyToPlay());
        }
    }

  

    public static void CreateRoom(string roomName,int _maxPlayers,int sizeMaze)
    {
        mapRoomNameSize.Add(roomName, sizeMaze);
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = (byte)_maxPlayers, IsOpen = true, IsVisible = true, RealSize = sizeMaze }, lobbyName);
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
        if (PhotonNetwork.room.PlayerCount < PhotonNetwork.room.MaxPlayers)
        {
            waitPanelInitilized = Instantiate(waitPanel);
            waitPanelInitilized.transform.parent = GameObject.Find("CanvasMenu").transform;
            waitPanelInitilized.gameObject.transform.position = GameObject.Find("CanvasMenu").transform.position;
        }

    }

    IEnumerator GameIsReadyToPlay()
    {
       
        //CREATE SPAWN POINTS
        standbyCamera.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        Vector3 initialSpawnPoint = worldGen.GetComponent<MazeGenerator>().cellsGroundPositionSpawn[0];
        GameObject myPlayer = PhotonNetwork.Instantiate(player.name, initialSpawnPoint, Quaternion.identity, 0); // spawneaza la toti
        myPlayer.transform.Find("FirstPersonCharacter").gameObject.SetActive(true);
        myPlayer.transform.Find("HealthCrosshair").gameObject.SetActive(true);
        myPlayer.GetComponent<FirstPersonController>().enabled = true;
        myPlayer.GetComponent<PlayerMovement>().enabled = true;
        myPlayer.GetComponent<NetworkCharacter>().enabled = true;
        myPlayer.GetComponent<PlayerShoting>().enabled = true;
        myPlayer.GetComponent<Health>().enabled = true;

        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
            StartCoroutine(GenerateExitByCallingRpc());
    }


    IEnumerator GenerateExitByCallingRpc()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<PhotonView>().RPC("GenerateExit", PhotonTargets.All, worldGen.GetComponent<MazeGenerator>().cellsGroundPositionSpawn[worldGen.GetComponent<MazeGenerator>().cellsGroundPositionSpawn.Count-1]);

    }

    [PunRPC]
    void GenerateExit(Vector3 initialSpawnPoint)
    {
        Instantiate(exit, initialSpawnPoint, exit.transform.rotation);
    }

    [PunRPC]
    void PlayerDies()
    {
        transform.position = hiddenPosition;
    }

    [PunRPC]
    void PlayerRespawns()
    {
        transform.position = worldGen.GetComponent<MazeGenerator>().cellsGroundPositionSpawn[0]; 
    }


}