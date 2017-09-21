using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsFriendPrefab : MonoBehaviour {

    public GameObject chatBoxPrefab;
    public void StartChatButton()
    {
        GameObject chatBoxTemp = Instantiate(chatBoxPrefab);
        chatBoxTemp.transform.parent = GameObject.Find("CanvasMenu").transform;
        chatBoxTemp.gameObject.transform.position = GameObject.Find("CanvasMenu").transform.position;

    }
}
