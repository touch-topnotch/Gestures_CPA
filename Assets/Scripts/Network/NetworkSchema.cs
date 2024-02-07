using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Union(0, typeof(HelloCmd))]
public interface INetworkData
{
    string cmd { get; set; }
}

[MessagePackObject(keyAsPropertyName: true)]
public class HelloCmd : INetworkData
{
    public string cmd { get; set; } = "hello";
    public string msg;
}