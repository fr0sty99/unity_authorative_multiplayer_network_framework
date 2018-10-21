using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

// our PacketManager accepts a generic type <T> which has to be a class
using MessagePack.Decoders;
public class NetworkPackageManager<T> where T : class {
    Client client;
    public event Action<byte[]> OnRequirePackageTransmit; // event, which gets raised when we require to transmit our packages

    private float m_SendSpeed = .2f; // .2 seconds
    public float SendSpeed {
        get {
            if (m_SendSpeed < 0.1f) {
                return m_SendSpeed = .1f;
            } else {
                return m_SendSpeed;
            }
        }
        set {
            m_SendSpeed = value;
        }
    }

    float nextTick;

    // these lists hold all the packages we want to transmit
    private List<T> m_Packages;
    public List<T> Packages {
        get {
            if (m_Packages == null) {
                m_Packages = new List<T>();
            }
            return m_Packages;
        }
    }

    // queue with packages we have received
    public Queue<T> receivedPackages;

    /// <summary>
    /// Add a new package to the List T we would like to transmit soon.
    /// </summary>
    /// <param name="package">Package.</param>
    public void AddPackage(T package) {
        Packages.Add(package);
    }

    /// <summary>
    /// Processes any data received.
    /// </summary>
    /// <param name="bytes">Bytes.</param>
    public void ReceiveData(byte[] bytes)
    {
        if (receivedPackages == null)
        {
            receivedPackages = new Queue<T>();
        }

        T[] packages = ReadBytes(bytes).ToArray();

        for (int i = 0; i < packages.Length; i++)
        {
            receivedPackages.Enqueue(packages[i]);
        }

    }

    // will raise "OnRequirePackageTransmit" when the time is here
    public void Tick() {
        nextTick += 1 / this.SendSpeed * Time.fixedDeltaTime;
        if(nextTick > 1 && Packages.Count > 0) {
            nextTick = 0;
            if(OnRequirePackageTransmit != null) {
                byte[] bytes = CreateBytes();
                Packages.Clear();

                // TODO: implement new client with LLAPI
                // client.sendBytes(bytes);

                OnRequirePackageTransmit(bytes); // raise event 
            }
        }
    }

    public T GetNextDataReceived() {
        if(receivedPackages == null || receivedPackages.Count == 0) {
            return default(T);
        }

        return receivedPackages.Dequeue(); // takes first one out of the list ( receivedPackages ) and remove it.. then return it
    }

    byte[] CreateBytes() {
        BinaryFormatter formatter = new BinaryFormatter();
        using(MemoryStream ms = new MemoryStream()) {
            formatter.Serialize(ms, this.Packages);
            return ms.ToArray();
        }
    }

    List<T> ReadBytes(byte[] bytes) {
        BinaryFormatter formatter = new BinaryFormatter();
        using(MemoryStream ms = new MemoryStream()) {
            ms.Write(bytes, 0, bytes.Length);
            ms.Seek(0, SeekOrigin.Begin);
            return (List<T>) formatter.Deserialize(ms);
        }
    }

}
