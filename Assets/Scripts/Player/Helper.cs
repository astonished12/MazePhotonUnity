using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : Photon.MonoBehaviour {

    public GameObject helpPet;

    [PunRPC]
    void SpawnHelper(Vector3 position)
    { //Get the seed from the master client in an RPC
      //This RPC is called automatically since it's a "buffered" rpc
        GameObject tmp = Instantiate(helpPet, position,Quaternion.identity) as GameObject;
      
    }

   
}
