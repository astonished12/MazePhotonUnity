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
    private void Start()
    {

      
        userName.GetComponent<Text>().text += " " + UserData.userName;
        email.GetComponent<Text>().text += " " + UserData.email;
        nomatches.GetComponent<Text>().text += " " + UserData.nomatches;
        nomatchesWon.GetComponent<Text>().text += " " + UserData.nomatchesWon;
        //SET AVATAR LIKE IN  FRINEDS MANAGER ACTIVITY WINDOW
        output.texture = GameObject.Find("CanvasMenu").transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<RawImage>().texture;
    }
}

