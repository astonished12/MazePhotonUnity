using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatBox : MonoBehaviour
{

   
    private RectTransform rect;
    Vector3 initialScale;
    private SocketIOComponent SocketIO;
    public bool newMessage;
    public string inputField = "";
    public string senderId;
    public ArrayList entries = new ArrayList();
    JSONParser myJsonParser = new JSONParser();

    public void Awake()
    {
        rect = GetComponent<RectTransform>();
        initialScale = gameObject.transform.localScale;
        //SocketIO = GameObject.Find("SocketRegisterLogin").GetComponent<SocketIOComponent>();
    }

    /*public void OnCloseButtonPressed()
    {
        string friendName = gameObject.transform.Find("To").GetComponent<Text>().text;
        ChatManager.chatList.Remove(friendName);
        Destroy(gameObject);
    }
    */
    public void OnMinimizeButtonPressed()
    {
        gameObject.transform.localScale /= 5f;
    }

    public void OnMaximizeButtonPressed()
    {
        gameObject.transform.localScale = initialScale;
    }

    /*public void SendMessageButtonPressed()
    {
        string message = messageInputField.GetComponent<InputField>().text;
        newMessage = true;
        //to do add to messageScroll and send to node
        SocketIO.Emit("newMessageGlobalChat", new JSONObject(myJsonParser.MessageToPersonToJson(message, gameObject.transform.Find("To").GetComponent<Text>().text)));
    }

    public void AddChatEntry(string name, string msg, bool isMine)
    {
        ChatEntry newEntry = new ChatEntry();
        newEntry.name = name;
        newEntry.message = msg;
        newEntry.isMine = isMine;
        newEntry.timeTag = "[" + System.DateTime.Now.Hour.ToString() + ":" + System.DateTime.Now.Minute.ToString() + "]";
        entries.Add(newEntry);
        GameObject newMessage = Instantiate(newMessagePrefab);
        newMessage.transform.SetParent(transform.Find("ScrollData").transform.Find("Viewport").GetChild(0).transform, false);
        if (newEntry.isMine)
            newMessage.GetComponent<Text>().color = Color.white;
        else
            newMessage.GetComponent<Text>().color = Color.green;

        newMessage.GetComponent<Text>().text = (newEntry.timeTag + " " + newEntry.name + ": " + newEntry.message);
    }*/
    public void OnDrag(UnityEngine.EventSystems.BaseEventData eventData)
    {
        var pointerData = eventData as UnityEngine.EventSystems.PointerEventData;
        if (pointerData == null) { return; }

        var currentPosition = rect.position;
        currentPosition.x += pointerData.delta.x;
        currentPosition.y += pointerData.delta.y;
        rect.position = currentPosition;
    }
}
