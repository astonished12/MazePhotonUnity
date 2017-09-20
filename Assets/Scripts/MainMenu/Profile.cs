using SocketIO;
using System;
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
    private JSONParser myJsonParser;
    private void Start()
    {

        SocketIO = GameObject.Find("SetupSocketConnectionToGame").GetComponent<SocketIOComponent>();
        myJsonParser = new JSONParser();
        userName.GetComponent<Text>().text += " "+ UserData.userName;
        email.GetComponent<Text>().text += " "+UserData.email;
        nomatches.GetComponent<Text>().text += " "+UserData.nomatches;
        nomatchesWon.GetComponent<Text>().text += " "+UserData.nomatchesWon;
        Debug.Log(UserData.photourl);
        if (UserData.photourl == "empty")
        {        
            output.texture = Resources.Load<Texture2D>("unknown");
        }
        else
        {
            SocketIO.Emit("getPhoto",new JSONObject(myJsonParser.LoginUserAndUrlPhotoToJSON(UserData.userName, UserData.photourl)));
        }

        SocketIO.On("photobase64", OnPhotoReceive);
    }
    
    public void OnPhotoReceive(SocketIOEvent eventObj)
    {
        String base64string = myJsonParser.ElementFromJsonToString(eventObj.data.GetField("photoBase64").ToString())[1];
        Debug.Log("aplicam textura : "+ base64string);
        Texture2D convertedBase64String = new Texture2D(128, 128);
        byte[] decodedBytes = Convert.FromBase64String(base64string);
        convertedBase64String.LoadImage(decodedBytes);
        output.texture = convertedBase64String;
    }
}

