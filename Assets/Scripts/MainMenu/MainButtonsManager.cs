using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MainButtonsManager : MonoBehaviour {

    public GameObject profilePanel;
    
    public void ProfileButtonPressed()
    {
        GameObject profilePanelTmp = Instantiate(profilePanel);
        profilePanelTmp.transform.parent = GameObject.Find("CanvasMenu").transform;
        profilePanelTmp.gameObject.transform.position = GameObject.Find("CanvasMenu").transform.position;
    }

   
}
