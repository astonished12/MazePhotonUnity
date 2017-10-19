using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

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
    Queue<string> messages= new Queue<string>();
    const int messageCount = 6;
    private Text messageWindow;


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
            MazeGenerator.realSeed = seed;
            MazeGenerator.realSize = mapRoomNameSize[PhotonNetwork.room.Name];

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

    void StartSpawnProcess(object[] parameters)
    {
        StartCoroutine("SpawnPlayer", parameters);
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

    IEnumerator SpawnPlayer(object[] parameters)
    {


        float respawnTime = (float) parameters[0];
        int index = (int) parameters[1];
        yield return new WaitForSeconds(respawnTime);

        Vector3 initialSpawnPoint = worldGen.GetComponent<MazeGenerator>().cellsGroundPositionSpawn[index];
        GameObject myPlayer = PhotonNetwork.Instantiate(player.name, initialSpawnPoint, Quaternion.identity, 0); // spawneaza la toti
        myPlayer.transform.Find("FirstPersonCharacter").gameObject.SetActive(true);
        myPlayer.transform.Find("HealthCrosshair").gameObject.SetActive(true);
        myPlayer.GetComponent<FirstPersonController>().enabled = true;
        myPlayer.GetComponent<PlayerMovement>().enabled = true;
        myPlayer.GetComponent<NetworkCharacter>().enabled = true;
        myPlayer.GetComponent<PlayerShoting>().enabled = true;
        myPlayer.GetComponent<Health>().enabled = true;

        myPlayer.GetComponent<Health>().RespawnMe += StartSpawnProcess;
        myPlayer.GetComponent<Health>().SendNetworkMessage += AddMessage;




        AddMessage("Spawned player: " + UserData.userName);

    }


    IEnumerator GameIsReadyToPlay()
    {

        GameObject temp = GameObject.Find("Network").gameObject.transform.GetChild(0).gameObject;
        temp.SetActive(true);
        SetMessages(temp);
        
        //CREATE SPAWN POINTS
        standbyCamera.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        object[] parameters = new object[2]{0f,0};
        StartCoroutine("SpawnPlayer", parameters);

        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
            StartCoroutine(GenerateExitByCallingRpc());

      }




    void SetMessages(GameObject temp)
    {
        messageWindow = temp.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<Text>();
    }



    IEnumerator GenerateExitByCallingRpc()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<PhotonView>().RPC("GenerateExit", PhotonTargets.All, worldGen.GetComponent<MazeGenerator>().cellsGroundPositionSpawn[worldGen.GetComponent<MazeGenerator>().cellsGroundPositionSpawn.Count-1]);

    }

    

    void AddMessage(string message)
    {
        GetComponent<PhotonView>().RPC("AddMessage_RPC", PhotonTargets.All, message);
    }


    [PunRPC]
    void AddMessage_RPC(string message)
    {
        messages.Enqueue(message);
        if (messages.Count > messageCount)
            messages.Dequeue();

        messageWindow.text = "";
        foreach (string m in messages)
            messageWindow.text += m + "\n";
    }


    [PunRPC]
    void GenerateExit(Vector3 initialSpawnPoint)
    {
        Instantiate(exit, initialSpawnPoint, exit.transform.rotation);
    }

}