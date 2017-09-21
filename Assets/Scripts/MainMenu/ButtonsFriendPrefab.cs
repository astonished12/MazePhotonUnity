using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonsFriendPrefab : MonoBehaviour {

    public GameObject chatBoxPrefab;
    public void StartChatButton()
    {
        GameObject chatBoxTemp = Instantiate(chatBoxPrefab);
        chatBoxTemp.transform.parent = GameObject.Find("CanvasMenu").transform;
        chatBoxTemp.gameObject.transform.position = GameObject.Find("CanvasMenu").transform.position;

    }

    public void RemoveFriendButton()
    {
        GameObject btnTmp = EventSystem.current.currentSelectedGameObject;
        FriendsManager.RemoveFriend(btnTmp.transform.parent.Find("Text").GetComponent<Text>().text);
    }
}
