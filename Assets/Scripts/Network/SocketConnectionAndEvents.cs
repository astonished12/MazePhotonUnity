using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketConnectionAndEvents : MonoBehaviour {

    SocketIOComponent socket;
	// Use this for initialization
	void Start () {
        socket = GetComponent<SocketIOComponent>();
        socket.Connect();
    }
	
	
}
