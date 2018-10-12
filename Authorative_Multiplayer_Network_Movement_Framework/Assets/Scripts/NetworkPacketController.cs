using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPacketController : NetworkBehaviour {
    [System.Serializable]
    public class Package {
        public float horizontal;
        public float vertical;
        public float timeStamp;
    }
}
