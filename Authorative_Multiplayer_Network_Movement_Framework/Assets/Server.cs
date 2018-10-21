using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections.Generic;
using System.Xml.Serialization;

public class Server : MonoBehaviour {
    int connectionID;
    int maxConnections = 10;
    int reliableChannelID;
    int hostID;
    int serverSocketPort = 8887;
    byte error;

    public GameObject playerObject;
    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

	void Start()
	{
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        reliableChannelID = config.AddChannel(QosType.ReliableSequenced); // unrealiable is even faster here
        HostTopology topology = new HostTopology(config, maxConnections);                         
        hostID = NetworkTransport.AddHost(topology, serverSocketPort, null); // openening a socket, anyone(null) can connect to the server
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

        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

        switch (recNetworkEvent)
        {

            case NetworkEventType.ConnectEvent:
                Debug.Log("Server: ConnectEvent");
                GameObject temp = Instantiate(playerObject, transform.position, transform.rotation);
                players.Add(recConnectionID, temp);
                break;

            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                Debug.Log("Receiving: " + msg);
                // TODO: call move() with coords here

                string[] splitData = msg.Split('|');

                switch(splitData[0]) {
                    case "MOVE":
                        // move player 
                        Move(splitData[1], splitData[2], players[recConnectionID]);
                        break;
                }

                break;
            case NetworkEventType.DisconnectEvent:

                break;
            case NetworkEventType.Nothing:

                 break;
        }
    }

    void Move(string x, string y, GameObject obj) {
        // move the object to (x,y)
        float xMov = float.Parse(x);
        float yMove = float.Parse(y);

        obj.transform.Translate(xMov, 0, yMove);
    }
}