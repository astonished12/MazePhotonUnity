using UnityEngine;
using System.Collections;

public class WindowManager : MonoBehaviour {

    public GameObject scoreBoard;
    
    void Update () {
        if(Input.GetKeyDown(KeyCode.Tab)) {
            scoreBoard.SetActive( !scoreBoard.activeSelf );
        }
    }
}
