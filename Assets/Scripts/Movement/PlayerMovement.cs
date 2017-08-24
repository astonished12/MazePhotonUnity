using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    /*
     * 
     *   This component is used just for my player my local..
     *  
     * 
     */
    public float speed = 1f;
    Vector3 myDirection = Vector3.zero;

    CharacterController chContrl;
	// Use this for initialization
	void Start () {
        chContrl = GetComponent<CharacterController>();
    }
	
	// Update is called once per frame
	void Update () {
        myDirection = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized; 
	}


    //FixedUpdate is called once per physcs loop
    // Do all Movement and other physics here
    private void FixedUpdate()
    {
        chContrl.SimpleMove(myDirection * speed);
    }
}
