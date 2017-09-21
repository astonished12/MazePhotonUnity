using SocketIO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsManager : MonoBehaviour {

    public GameObject friendOnlinePrefab;
    public GameObject friendOfflinePrefab;
    public GameObject contentParent;
    public RawImage avatar;
    public GameObject inputNameFriendSearch;
    public GameObject messageAlert;

    private static SocketIOComponent SocketIO;
    private Dictionary<string, GameObject> friendList = new Dictionary<string, GameObject>();
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
        SocketIO.On("playerNotOnline", OnPlayerNotOnline);
        SocketIO.On("newFriend", OnNewFriend);

    }

    private void OnNewFriend(SocketIOEvent obj)
    {
        GameObject dialogMessage = Instantiate(messageAlert);
        dialogMessage.transform.parent = transform;
        dialogMessage.transform.position = inputNameFriendSearch.transform.position;
        dialogMessage.transform.Find("Message").gameObject.GetComponent<Text>().text = "New friend";

        JSONObject temp = new JSONObject();
        temp.AddField("username", UserData.userName);
        SocketIO.Emit("GetMyFriends",temp);

    }

    private void OnPlayerNotOnline(SocketIOEvent obj)
    {
        GameObject dialogMessage = Instantiate(messageAlert);
        dialogMessage.transform.parent = transform;
        dialogMessage.transform.position = inputNameFriendSearch.transform.position;
        dialogMessage.transform.Find("Message").gameObject.GetComponent<Text>().text = "User not online";
    }

    public void OnPhotoReceive(SocketIOEvent eventObj)
    {
        String base64string = myJsonParser.ElementFromJsonToString(eventObj.data.GetField("photoBase64").ToString())[1];
        Texture2D convertedBase64String = new Texture2D(128, 128);
        byte[] decodedBytes = Convert.FromBase64String(base64string);
        convertedBase64String.LoadImage(decodedBytes);
        avatar.texture = convertedBase64String;
    }

    public void AddFriendButtonClicked()
    {
        string posibleFriend = inputNameFriendSearch.GetComponent<InputField>().text;
        if (String.IsNullOrEmpty(posibleFriend)==false)
        {
            SocketIO.Emit("addFriend", new JSONObject(myJsonParser.NewFriendPackage(UserData.userName, posibleFriend)));
        }
        else
        {
            GameObject dialogMessage = Instantiate(messageAlert);
            dialogMessage.transform.parent = transform;
            dialogMessage.transform.position = inputNameFriendSearch.transform.position;
            dialogMessage.transform.Find("Message").gameObject.GetComponent<Text>().text = "Set an valid user";
        }
    }

    private void OnReceiveListFriends(SocketIOEvent obj)
    {
        DestroyAllFriendsGameObjects();
        friendList = new Dictionary<string, GameObject>();

        JSONObject friends = obj.data.GetField("friends");
        for (int i = 0; i < friends.Count; i++)
        {
            string username = myJsonParser.ElementFromJsonToString(friends[i].GetField("username").ToString())[1];
            string isOnline = friends[i].GetField("isOnline").ToString().Replace("\"", "");
            if (isOnline == "0")
            {
                GameObject newFriend = Instantiate(friendOfflinePrefab);
                newFriend.transform.Find("Text").GetComponent<Text>().text = username;
                newFriend.transform.SetParent(contentParent.transform, false);
                friendList.Add(username, newFriend);
            }
            else if (isOnline == "1")
            {
                GameObject newFriend = Instantiate(friendOnlinePrefab);
                newFriend.transform.Find("Text").GetComponent<Text>().text = username;
                newFriend.transform.SetParent(contentParent.transform, false);
                friendList.Add(username, newFriend);
            }
        }
    }

    private void DestroyAllFriendsGameObjects()
    {
        foreach (string key in friendList.Keys)
        {
            Destroy(friendList[key]);
        }
    }

}
