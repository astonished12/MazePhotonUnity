using SocketIO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsManager : MonoBehaviour {

    //public GameObject friendOnlinePrefab;
    //public GameObject friendOfflinePrefab;
    public RawImage avatar;

    public static SocketIOComponent SocketIO;
    private JSONParser myJsonParser;
    private void Start()
    {

        SocketIO = GameObject.Find("SetupSocketConnectionToGame").GetComponent<SocketIOComponent>();
        myJsonParser = new JSONParser();
        GameObject.Find("CanvasMenu").transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<Text>().text = "Welcome, " + UserData.userName;
        if (UserData.photourl == "empty")
        {
            avatar.texture = Resources.Load<Texture2D>("unknown");
        }
        else
        {
            SocketIO.Emit("getPhoto", new JSONObject(myJsonParser.LoginUserAndUrlPhotoToJSON(UserData.userName, UserData.photourl)));
        }

        SocketIO.On("photobase64", OnPhotoReceive);
    }

    public void OnPhotoReceive(SocketIOEvent eventObj)
    {
        String base64string = myJsonParser.ElementFromJsonToString(eventObj.data.GetField("photoBase64").ToString())[1];
        Texture2D convertedBase64String = new Texture2D(128, 128);
        byte[] decodedBytes = Convert.FromBase64String(base64string);
        convertedBase64String.LoadImage(decodedBytes);
        avatar.texture = convertedBase64String;
    }

}
