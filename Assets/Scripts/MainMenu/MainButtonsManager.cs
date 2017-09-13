using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MainButtonsManager : MonoBehaviour {

    public GameObject profilePanel;
    public GameObject chatBoxPrefab;

    public void ProfileButtonPressed()
    {
        GameObject profilePanelTmp = Instantiate(profilePanel);
        profilePanelTmp.transform.parent = GameObject.Find("CanvasMenu").transform;
        profilePanelTmp.gameObject.transform.position = GameObject.Find("CanvasMenu").transform.position;
    }

    public void StartChatButton()
    {
        GameObject chatBoxTemp = Instantiate(chatBoxPrefab);
        chatBoxTemp.transform.parent = GameObject.Find("CanvasMenu").transform;
        chatBoxTemp.gameObject.transform.position = GameObject.Find("CanvasMenu").transform.position;
        
    }
}
