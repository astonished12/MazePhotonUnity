using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsPrompt : MonoBehaviour {

	public void CancelOnClick()
    {
        Destroy(gameObject);
    }
}
