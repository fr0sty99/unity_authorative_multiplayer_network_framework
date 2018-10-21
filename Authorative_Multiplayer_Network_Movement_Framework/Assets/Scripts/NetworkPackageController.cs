
using UnityEngine.Networking;
using System;

// this class is responsible for sending and receiving packages in out network
public class NetworkPackageController : NetworkBehaviour {


    [Serializable]
    public class Package {
        public float horizontal;
        public float vertical;
        public float timeStamp;
    }

    [Serializable]
    public class ReceivePackage {
        public float x;
        public float y;
        public float z;
        public float timeStamp;
    }
    private MyNetworkPackageManager<MyPackage> m_PackackageManager;
    public MyNetworkPackageManager<MyPackage> PackageManager {
        get
        {
            if (m_PackackageManager == null)
            {
                m_PackackageManager = new MyNetworkPackageManager<MyPackage>();
                if (isLocalPlayer)
                {
                    m_PackackageManager.OnRequirePackageTransmit += TransmitPackageToServer;
                }
            }
            return m_PackackageManager;
        }
    }

    private MyNetworkPackageManager<MyReceivePackage> m_ServerPackageManager;
    public MyNetworkPackageManager<MyReceivePackage> ServerPackageManager {
        get {
            if(m_ServerPackageManager == null) {
                m_ServerPackageManager = new MyNetworkPackageManager<MyReceivePackage>();
                if (isServer)
                {
                    m_ServerPackageManager.OnRequirePackageTransmit += TransmitPackageToClient;
                }
            }
            return m_ServerPackageManager;
        }
    }
    /* TODO: recrete this with messagePack
    private NetworkPackageManager<Package> m_PackageManager;
    public NetworkPackageManager<Package> PackageManager {
        get {
            if(m_PackageManager == null) {
                m_PackageManager = new NetworkPackageManager<Package>();
                if(isLocalPlayer) {
                    m_PackageManager.OnRequirePackageTransmit += TransmitPackageToServer;
                }
            }
            return m_PackageManager;
        }
    }

    private NetworkPackageManager<ReceivePackage> m_ServerPackageManager;
    public NetworkPackageManager<ReceivePackage> ServerPackageManager
    {
        get
        {
            if (m_ServerPackageManager == null)
            {
                m_ServerPackageManager = new NetworkPackageManager<ReceivePackage>();
                if (isServer)
                {
                    m_ServerPackageManager.OnRequirePackageTransmit += TransmitPackageToClient;
                }
            }
            return m_ServerPackageManager;
        }
    }
    */

    private void TransmitPackageToClient(byte[] bytes) {
        RpcReceiveDataOnClient(bytes);
    }

    private void TransmitPackageToServer(byte[] bytes)
    {
        CmdTransmitPackages(bytes);
    }

    [Command]
    void CmdTransmitPackages(byte[] data) {
        PackageManager.ReceiveData(data);
    }

    [ClientRpc]
    void RpcReceiveDataOnClient(byte[] data) {
        ServerPackageManager.ReceiveData(data);
    }

    public virtual void FixedUpdate() {
        PackageManager.Tick();
        ServerPackageManager.Tick();
    }
}
