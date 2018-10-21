using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections.Generic;

public class Server : MonoBehaviour {
    int connectionID;
    int maxConnections = 10;
    int reliableChannelID;
    int hostID;
    int socketPort;

    public GameObject playerObject;
    List<GameObject> players;

	void Start()
	{
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        reliableChannelID = config.AddChannel(QosType.ReliableSequenced); // unrealiable is even faster here
        HostTopology topology = new HostTopology(config, maxConnections);                         
        hostID = NetworkTransport.AddHost(topology, socketPort, null); // openening a socket, anyone(null) can connect to the server
        Debug.Log("Socket open. Host ID is " + hostID);
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
        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, out recBuffer, bufferSize, out dataSize, out NetworkConnectionError);

        switch (recNetworkEvent)
        {
            case NetworkEventType.ConnectEvent:
                GameObject temp = Instantiate(playerObject, transform.position, transform.rotation);
                players.Add(temp);
                break;

            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                Debug.Log("Receiving: " + msg);
                // TODO: call move() with coords here

                break;
            case NetworkEventType.DisconnectEvent:

                break;
            case NetworkEventType.Nothing:

                 break;
        }
    }

    void Move(float x, float y, GameObject obj) {
        // move the object to (x,y)
    }
}