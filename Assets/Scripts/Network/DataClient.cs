using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using MessagePack;

public class NewBehaviourScript : MonoBehaviour
{
    WebSocket websocket;
    // Start is called before the first frame update
    void Start()
    {
        websocket = new WebSocket("ws://localhost:3001");
        websocket.OnOpen += () =>
        {
            Debug.Log("Connected!");
        };

        websocket.OnMessage += (bytes) =>
        {
            INetworkData data = MessagePackSerializer.Deserialize<INetworkData>(bytes);
            Debug.Log(MessagePackSerializer.ConvertToJson(bytes));

            data = MessagePackSerializer.Deserialize<HelloCmd>(bytes);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Disconnected!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.Connect();
    }

    public void Send(INetworkData data)
    {
        MessagePackSerializer.Serialize(data);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
