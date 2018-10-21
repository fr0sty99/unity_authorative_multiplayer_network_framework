using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[RequireComponent(typeof(CharacterController))]
public class PlayerNetwork : NetworkPackageController
{
    [SerializeField]
    float moveSpeed;

    [SerializeField]
    [Range(0.1f, 1)]
    float networkSendRate = .5f;

    [SerializeField]
    bool isPredictionEnabled = true;

    [SerializeField]
    float correctionTreshold;

    CharacterController controller;

    List<ReceivePackage> predictedPackages;
    Vector3 lastPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        PackageManager.SendSpeed = networkSendRate;
        ServerPackageManager.SendSpeed = networkSendRate;

        predictedPackages = new List<ReceivePackage>();
     }

    // Update is called once per frame
    void Update()
    {
        LocalClientUpdate();
        ServerUpdate();
        RemoteClientUpdate();
    }

    void Move(float horizontal, float vertical)
    {
        controller.Move(new Vector3(horizontal, 0, vertical));
    }

    void LocalClientUpdate()
    {
        if (!isLocalPlayer)
            return;
        
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Debug.Log("Movement going on");
            Debug.Log("Horizontal: " + Input.GetAxis("Horizontal"));
            Debug.Log("Vertical: " + Input.GetAxis("Vertical"));
            float time = Time.time;

            PackageManager.AddPackage(new MyPackage
            {
                horizontal = Input.GetAxis("Horizontal"),
                vertical = Input.GetAxis("Vertical"),
                timeStamp = time
            });

          if (isPredictionEnabled)
            {
                Move(Input.GetAxis("Horizontal") * moveSpeed, Input.GetAxis("Vertical") * moveSpeed);
                predictedPackages.Add(new ReceivePackage
                {
                    timeStamp = time,
                    x = transform.position.x,
                    y = transform.position.y,
                    z = transform.position.z
                });
            }
        }
       }

    void ServerUpdate() {
        Debug.Log("ServerUpdate");
        if(!isServer || isLocalPlayer) {
            return;
        }
        Debug.Log("authorized");

        MyPackage packageData = PackageManager.GetNextDataReceived();
        Debug.Log("PackageData: " + packageData);

        if(packageData == null) {
            return;
        }

        Move(packageData.horizontal * moveSpeed, packageData.vertical * moveSpeed);
        Debug.Log("move horizontal: " + packageData.horizontal * moveSpeed);
        Debug.Log("move vertical: " + packageData.vertical * moveSpeed);
        if(transform.position == lastPosition) {
            return;
        }

        lastPosition = transform.position;

        ServerPackageManager.AddPackage(new MyReceivePackage
        {
            x = transform.position.x,
            y = transform.position.y,
            z = transform.position.z,
            timeStamp = packageData.timeStamp
        });

    }

    public void RemoteClientUpdate() {
        if(isServer) 
            return;

        var data = ServerPackageManager.GetNextDataReceived();

        if (data == null)
            return;

        if(isLocalPlayer && isPredictionEnabled) {
            var transmittedPackage = predictedPackages.Where(x => x.timeStamp == data.timeStamp).FirstOrDefault();
            if (transmittedPackage == null)
                return;

            // if we exceed the treshold distance
            if(Vector3.Distance(new Vector3(transmittedPackage.x, transmittedPackage.y, transmittedPackage.z), new Vector3(data.x, data.y, data.z)) > correctionTreshold) {
                // snap to received location
                transform.position = new Vector3(data.x, data.y, data.z);
            }

            // clear out old prediction
            predictedPackages.RemoveAll(x => x.timeStamp <= data.timeStamp);

        } else {
            transform.position = new Vector3(data.x, data.y, data.z);
        }

     }

}
