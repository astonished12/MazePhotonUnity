using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Profile : MonoBehaviour {

    public GameObject userName;
    public GameObject email;
    public GameObject nomatches;
    public GameObject nomatchesWon;
    public RawImage output;
    public static SocketIOComponent SocketIO;

    private void Start()
    {

        SocketIO = GameObject.Find("SetupSocketConnectionToGame").GetComponent<SocketIOComponent>();

        userName.GetComponent<Text>().text += " "+ UserData.userName;
        email.GetComponent<Text>().text += " "+UserData.email;
        nomatches.GetComponent<Text>().text += " "+UserData.nomatches;
        nomatchesWon.GetComponent<Text>().text += " "+UserData.nomatchesWon;

        if (UserData.photourl != "")
        {
            SocketIO.Emit("newPhoto");
        }
        else
        {
            output.texture = Resources.Load<Texture2D>("unknown");

        }
        SocketIO.On("photoReceive", OnPhotoReceive);
    }
    
    public void OnPhotoReceive(SocketIOEvent eventObj)
    {
        Debug.Log("Ar trebui sa primesc poza");
    }
}

