using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerScoreList : MonoBehaviour {

    public GameObject playerScoreEntryPrefab;

    private ScoreManager scoreManager;

    int lastChangeCounter;

    // Use this for initialization
    void Start ()
    {
        scoreManager = GameObject.Find("ScoreBoard").GetComponent<ScoreManager>();

        lastChangeCounter = scoreManager.GetChangeCounter();
    }

    void InsertDataInScoreBoardBasedOnPlayersRoom()
    {
        scoreManager.InsertDataInScoreBoard();
    }
    // Update is called once per frame
    void Update()
    {

        if (WindowManager.checkNow)
        {


            if (scoreManager == null)
            {
                Debug.LogError("You forgot to add the score manager component to a game object!");
                return;
            }

            InsertDataInScoreBoardBasedOnPlayersRoom();

            if (scoreManager.GetChangeCounter() == lastChangeCounter)
            {
                // No change since last update!
                return;
            }

            lastChangeCounter = scoreManager.GetChangeCounter();

            while (transform.childCount > 0)
            {
                Transform c = transform.GetChild(0);
                c.SetParent(null); // Become Batman
                Destroy(c.gameObject);
            }

            string[] names = scoreManager.GetPlayerNames("kills");
            foreach (string name in names)
            {
                GameObject go = (GameObject) Instantiate(playerScoreEntryPrefab);
                go.transform.SetParent(transform);
                go.transform.Find("Username").GetComponent<Text>().text = name;
                go.transform.Find("Kills").GetComponent<Text>().text = scoreManager.GetScore(name, "kills").ToString();
                go.transform.Find("Deaths").GetComponent<Text>().text =
                    scoreManager.GetScore(name, "deaths").ToString();
            }
        }
    }
}
