using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;

    public class Client
    {
        int connectionID;
        int maxConnections = 10;
        int reliableChannelID;
        int hostID;
        int socketPort;
        byte error;

        void Start()
        {
        NetworkTransport.Init();
        }

        void Update()
        {

        }

    public void connect() {
        ConnectionConfig config = new ConnectionConfig();
        reliableChannelID = config.AddChannel(QosType.ReliableSequenced); // unrealiable is even faster here
        HostTopology topology = new HostTopology(config, maxConnections);
        hostID = NetworkTransport.AddHost(topology, socketPort, null); // openening a socket, anyone(null) can connect to the server
        Debug.Log("Socket open. Host ID is " + hostID);

        // connecting to localhost / 127.0.0.1
        connectionID = NetworkTransport.Connect(hostID, "127.0.0.1", socketPort, 0 , out error);
    }

    public void disconnect() {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }

    public void sendMessage(String message) {
        byte[] buffer = Encoding.Unicode.GetBytes(message); // string to byte array
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, message.Length * sizeof(char), out error);
    }

    public void sendBytes(byte[] bytes) {
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, bytes, bytes.Length, out error);
    }
}
