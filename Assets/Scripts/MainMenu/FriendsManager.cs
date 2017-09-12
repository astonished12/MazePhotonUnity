using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsManager : MonoBehaviour {

    //public GameObject friendOnlinePrefab;
    //public GameObject friendOfflinePrefab;

	void Start()
    {
        GameObject.Find("CanvasMenu").transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<Text>().text = "Welcome, " + UserData.userName;
    }
}
