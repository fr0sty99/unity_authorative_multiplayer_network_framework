using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;

public class Client : MonoBehaviour
{
    int connectionID;
    int maxConnections = 10;
    int reliableChannelID;
    int hostID;
    int socketPort = 8888;
    int serverSocketPort = 8887;
    byte error;

    void Start()
    {
        NetworkTransport.Init();

    }

    void Update()
    {
        // receiving
        int recHostID;
        int recConnectionID;
        int recChannelID;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;

        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

        switch (recNetworkEvent)
        {

            case NetworkEventType.ConnectEvent:

                break;

            case NetworkEventType.DataEvent:

                break;
            case NetworkEventType.DisconnectEvent:

                break;
            case NetworkEventType.Nothing:

                break;
        }
    }

    public void connect()
    {
        ConnectionConfig config = new ConnectionConfig();
        reliableChannelID = config.AddChannel(QosType.ReliableSequenced); // unrealiable is even faster here
        Debug.Log("ChannelID: " + reliableChannelID);
        HostTopology topology = new HostTopology(config, maxConnections);
        hostID = NetworkTransport.AddHost(topology, socketPort, null);
        Debug.Log("Socket open. Host ID is " + hostID);

        // connecting to localhost / 127.0.0.1
        connectionID = NetworkTransport.Connect(hostID, "127.0.0.1", serverSocketPort, 0, out error);
        Debug.Log("Client connected:  connectionID: " + connectionID);
        NetworkError e = (NetworkError)error;
        Debug.Log("connection ok: " + (e == NetworkError.Ok));
    }

    public void disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }

    public void sendMessage(String message)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(message); // string to byte array
        bool successfull = NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, message.Length * sizeof(char), out error);
        Debug.Log("hostID: " + hostID);
        Debug.Log("connectionID: " + connectionID);
        Debug.Log("reliableChannelID: " + reliableChannelID);
        Debug.Log("buffer: " + buffer);

        Debug.Log("Client trying to sendMessage: successful?: " + successfull);
        Debug.Log("error: " + error.ToString());
    }

    public void sendBytes(byte[] bytes)
    {
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, bytes, bytes.Length, out error);
    }
}
