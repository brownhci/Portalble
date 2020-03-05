using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;


public class IOSWSManager : MonoBehaviour //, WebSocketUnityDelegate
{

    // Web Socket for Unity
    // private WebSocketUnity webSocket;
    public string websocketServer = "192.168.1.201";
    public string websocketPort = "9999";
    private GameObject hand_l, hand_r;
    private string handinfo_l = "";
    private string handinfo_r = "";
    private string gestureinfo = "";
    private getTime m_timeManager;
    private string[] queueActiveHand;
    private const int ACTIVE_HAND_BUFFER_SIZE = 10;
    private int handqueue_idx = 0;

    private bool websocketReceived = false;
    private int[] websocketReceivingEventQue;
    private const int WEBSOCKET_EVENT_QUE_SIZE = 10;
    private int websocket_idx = 0;
    private float websocketLastUpdate = 0;
    private bool websocketIdel = true;
    private string curr_message = "";

    //  public Vector2 faceTrackingScreenDims = new Vector2 (480, 320);
    //  private float eyeDistance = -1.0f;
    //  private float eyeScale = -1.0f;
    //  private Vector2 eyeCentroid = new Vector2(0, 0);
    //
    private GameObject InputHolder;
    // private InputField websocketInputField;
    // Use this for initialization
    void Start()
    {

        InputHolder = GameObject.Find("NodeServer");
        // websocketInputField = InputHolder.GetComponentInChildren<UnityEngine.UI.InputField> ();
        // websocketInputField.text = websocketServer;
        // websocketInputField.onEndEdit.AddListener(delegate { updateWebSocketServerInfo();});

        hand_l = GameObject.Find("Hand_l");
        hand_r = GameObject.Find("Hand_r");

        m_timeManager = GetComponent<getTime>();
        if (m_timeManager == null)
        {
            m_timeManager = gameObject.AddComponent<getTime>();
        }

        queueActiveHand = new string[ACTIVE_HAND_BUFFER_SIZE];
        websocketReceivingEventQue = new int[WEBSOCKET_EVENT_QUE_SIZE];
        for (int i = 0; i < WEBSOCKET_EVENT_QUE_SIZE; i++)
            websocketReceivingEventQue[i] = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (m_timeManager != null)
        {
            if ((m_timeManager.getCurrentTime() - websocketLastUpdate) > 1)
            {
                websocketIdel = true;
                websocketLastUpdate = m_timeManager.getCurrentTime();
            }
        }

        bool result = Portalble.Funcs.idleHandManager(hand_l, hand_r, getActiveHand());
        if (!result)
            Debug.Log("Something went wrong with idleHandManager, unkonwn issue");
        // issue here, if No_hand is found in begining, 
        // it will deactivate hand_r and hand_l, thus making
        // other script using that not usable.
        curr_message = Jetfire.curr_message;
        // p_message
        if (curr_message != "")
        {
            OnWebSocketUnityReceiveMessage(curr_message);
        }
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    public void OnEnable()
    {
        // Create web socket
        Debug.Log("Connecting" + websocketServer);
        string url = "ws://" + websocketServer + ":" + websocketPort;
        // webSocket = new WebSocketUnity(url, this);

        // Open the connection
        // webSocket.Open();
        Jetfire.Open(url);
    }

    private void updateWebSocketServerInfo()
    {
        // websocketServer = websocketInputField.text;
        Jetfire.Close();
        OnEnable();
    }

    #region Jetfire implementation

    // These callbacks come from WebSocketUnityDelegate
    // You will need them to manage websocket events
    public string getHandInfoLeft()
    {
        return handinfo_l;
    }
    public string getHandInfoRight()
    {
        return handinfo_r;
    }

    public string getGestureInfoRight()
    {
        return gestureinfo;
    }

    // This event happens when the websocket is opened
    public void OnWebSocketUnityOpen(string sender)
    {
        Debug.Log("WebSocket connected, " + sender);
        //GameObject.Find("NotificationText").GetComponent<TextMesh>().text = "WebSocket connected, "+sender;
    }

    // This event happens when the websocket is closed
    public void OnWebSocketUnityClose(string reason)
    {
        Debug.Log("WebSocket Close : " + reason);
        //GameObject.Find("NotificationText").GetComponent<TextMesh>().text = "WebSocket Close : "+reason;
    }

    // This event happens when the websocket received a message
    public void OnWebSocketUnityReceiveMessage(string message)
    {
        var hand_list = message.Split(new string[] { "#OneMore#" }, System.StringSplitOptions.None);
        var gesture_list = message.Split(new string[] { "#GestureDetected#" }, System.StringSplitOptions.None);
        // Assign partial message to left/hand variable
        //var List = message.Split (new char[] {',', ':', ';'});
        string handinfo_l_temp = "";
        string handinfo_r_temp = "";
        for (int hand_i = 0; hand_i < hand_list.Length; hand_i++)
        {
            var hand_info = hand_list[hand_i].Split(new char[] { ',', ':', ';' });
            if (hand_info[0].Contains("hand_type"))
            {
                //Debug.Log (hand_info [i]);
                if (hand_info[1].Contains("left"))
                {
                    handinfo_l_temp = hand_list[hand_i];
                }
                else
                {
                    handinfo_r_temp = hand_list[hand_i];
                }

                string hand_type = hand_info[1];
                // update values
                if (m_timeManager != null)
                    handQueAdd(hand_type);
            }
        }


        handinfo_l = handinfo_l_temp;
        handinfo_r = handinfo_r_temp;


        // Find if there are gestures detected
        if (gesture_list.Length > 1)
        {
            gestureinfo = gesture_list[1];
        }

        if (m_timeManager != null)
        {
            websocketIdel = false;
            websocketQueAdd(1);
        }
        // Debug.Log("test for comm");
        // Debug.Log(getStringMode(queueActiveHand));
    }

    private void websocketQueAdd(int que)
    {
        websocketReceivingEventQue[websocket_idx] = que;
        websocket_idx += 1;
        if (websocket_idx >= WEBSOCKET_EVENT_QUE_SIZE)
            websocket_idx = 0;
        websocketLastUpdate = m_timeManager.getCurrentTime();

    }

    private void handQueAdd(string hand_type)
    {
        queueActiveHand[handqueue_idx] = hand_type;
        handqueue_idx += 1;
        if (handqueue_idx >= ACTIVE_HAND_BUFFER_SIZE)
            handqueue_idx = 0;
    }

    /*
    // This event happens when the websocket received data (on mobile : ios and android)
    // you need to decode it and call after the same callback than PC
    public void OnWebSocketUnityReceiveDataOnMobile(string base64EncodedData)
    {
        // it's a limitation when we communicate between plugin and C# scripts, we need to use string
        byte[] decodedData = System.Convert.FromBase64String(base64EncodedData);
        OnWebSocketUnityReceiveData(decodedData);
    }

    // This event happens when the websocket did receive data
    public void OnWebSocketUnityReceiveData(byte[] data)
    {
        int testInt1 = System.BitConverter.ToInt32(data, 0);
        int testInt2 = System.BitConverter.ToInt32(data, 4); ;

        //Debug.Log("Received data from server : " + testInt1+", "+testInt2);
        //GameObject.Find("NotificationText").GetComponent<TextMesh>().text = "Received data from server : " + testInt1+", "+testInt2;
    }
    */

    // This event happens when you get an error@
    public void OnWebSocketUnityError(string error)
    {
        Debug.LogError("WebSocket Error : " + error);
        // GameObject.Find("NotificationText").GetComponent<TextMesh>().text = "WebSocket Error : "+ error;
    }


    /// <summary>
    /// Get current buffered active hand
    /// </summary>
    /// <returns>a string of 'NO_HAND','LEFT_HAND','RIGHT_HAND'</returns>
    public string getActiveHand()
    {
        return getStringMode(queueActiveHand);
    }

    // get the highest frequency of string in the queue
    private string getStringMode(string[] que)
    {

        if (websocketIdel)
            return "NO_HAND";

        string tmp = "Start:" + que.Length + ",";
        for (var i = 0; i < que.Length; i++)
            tmp += que[i] + ",";

        // Debug.Log(tmp);

        List<string> t = new List<string>(que);

        Dictionary<string, int> vote = new Dictionary<string, int>();
        int outv = 0;
        foreach (string v in t)
        {
            if (v == null)
                continue;
            if (vote.TryGetValue(v, out outv))
            {
                vote[v] = outv + 1;
            }
            else
            {
                vote.Add(v, 1);
            }
        }

        var result = vote.OrderByDescending(i => i.Value).First();

        // Debug.Log("sorting result");
        //Debug.Log(result.Key + ";" + result.Value);
        // Debug.Log(result.Value);
        if (result.Key.ToString().Contains("right"))
        {
            return "RIGHT_HAND";
        }
        else if (result.Key.ToString().Contains("left"))
        {
            return "LEFT_HAND";
        }
        else
        {
            return "NO_HAND";
        }

        return "ERROR";
    }

    #endregion 
}