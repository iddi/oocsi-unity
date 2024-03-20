/*************************************************************
      Goal: Help Unity users to send / receive OOCSI messages
   Made by: I-Tang (Eden) Chiang
Created on: Mar.13, 2024
*************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using UnityEngine;
using UnityEngine.UI;
using NativeWebSocket;


public class SendOOCSIMsgWithJObject : MonoBehaviour {
    /*
    * Private parameters
    */
    private WebSocket websocket;
    private bool isSettingConnection = false;
    private int ping_counter = 0;

    /*
    * Public parameters (to connect Unity objects)
    */
    public Button btn_sendOOCSIMsg;
    public Text btn_text;

    // channel setting
    public string channel_to_receive;
    public string channel_to_send;
    // ping_channel

    // Start is called before the first frame update
    void Start() {
        // Unity app is not running in background by default
        Application.runInBackground = true;
        SetupConnection();
    }

    /*
    * (Re)Create connection with server
    */
    async void SetupConnection() {
        // start connecting
        isSettingConnection = true;
        // target server to connect to
        websocket = new WebSocket("wss://oocsi.id.tue.nl/ws");
        
        // connection is established
        websocket.OnOpen += () => {
            if (isSettingConnection)    isSettingConnection = false;
            // Keep sending messages at every 13.0s
            InvokeRepeating("KeepConnection", 0.0f, 13.0f);
            Debug.Log("Connection open!");
            // subscribe to channelName
            if ( channel_to_receive != null && channel_to_receive.Trim().Length > 0) {
                websocket.SendText("subscribe " + channel_to_receive);
                Debug.Log("Subscribed to " + channel_to_receive);
            }

            btn_text.text = "setup!";
        };

        // error during establishing connection
        websocket.OnError += (e) => {
            Debug.Log("Error! " + e);
        };

        // connection closed
        websocket.OnClose += (e) => {
            Debug.Log("Connection closed!" + e);
        };

        // message is incoming
        websocket.OnMessage += (bytes) => {
            // Debug.Log("OnMessage!");
            // Debug.Log(bytes);    // byte[]

            // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("OnMessage: " + message);


            if (message == "ping") {
                ping_counter++;
                message += "\n" + ping_counter.ToString();
                btn_text.text = message;
            } else {
                JObject messageInJObj = JObject.Parse(message);

                if (messageInJObj["message"] != null) {
                    ping_counter++;
                    // message += "\n" + ping_counter.ToString();
                    btn_text.text = "Click me!";
                } else {
                    OnMessageReceived(messageInJObj);
                }
            }
        };

        // send message only when the button is clicked
        btn_sendOOCSIMsg.onClick.AddListener(delegate { SendWebSocketMessage(); });

        // keep connection and wait for messages
        await websocket.Connect();
    }

    // check incoming data
    void Update() {
        #if !UNITY_WEBGL || UNITY_EDITOR
            // read incoming messages and trigger .OnMessage()
            websocket.DispatchMessageQueue();
        #endif
    }

    /*
    * Send data to OOCSI with websocket
    */
    async void SendWebSocketMessage() {
        Debug.Log("to send message");
        
        // check connection status
        if (websocket.State == WebSocketState.Closed || websocket.State == WebSocketState.Closing) {
            // if not connected or the connection is closing
            // reconnect and send message again
            SetupConnection();
            SendWebSocketMessage();
        } else if (websocket.State == WebSocketState.Connecting) {
            // if it's connecting
            // wait until connected and send message again
            while (websocket.State != WebSocketState.Open) {
                continue;
            }
            SendWebSocketMessage();
        } else {
            // connection is ready to send message
            btn_text.text = "button pressed!";
            
            // set data
            JObject o = JObject.FromObject(new
            {
                device_id = "da090a6d51a804c66",
                airquality = 0.56f,
                doorclosed = true
            });
            // Debug.Log("jobject:\n" + o.ToString());

            string msgToSend = "sendjson " + channel_to_send + " "+ o.ToString();
            // Debug.Log("sent message: " + o.ToString());

            // send message
            await websocket.SendText(msgToSend);
        }
    }

    private void OnMessageReceived(JObject messageInJObj) {
        btn_text.text = messageInJObj.ToString();
        int num = (int)messageInJObj["data"]["number"];
        string text = (string)messageInJObj["data"]["text"];
        bool doorIsOpen = (bool)messageInJObj["data"]["boolean"];

        Debug.Log("Received JSON object: number = " + num);
        Debug.Log("Received JSON object: text = " + text);
        Debug.Log("Received JSON object: boolean = " + doorIsOpen);
    }

    /*
    * Close websocket when quit app
    */
    private async void OnApplicationQuit() {
        if (websocket != null && websocket.State == WebSocketState.Open) {
            // await websocket.Connect();
            await websocket.Close();
        }
    }

    /*
    * To send "ping;;" to  "ping_channel" to keep connected
    */
    private async void KeepConnection() {
        ping_counter++;
        btn_text.text = "'ping;;' sent!\n" + ping_counter.ToString();

        // set ping message
        JObject o = JObject.FromObject(new
        {
            message = "ping;;" + ping_counter.ToString()
        });

        string msgToSend = "sendjson ping_channel "+ o.ToString();
        // Debug.Log("sent ping message: " + o.ToString());
        // Sending ping message
        await websocket.SendText(msgToSend);
    }
}