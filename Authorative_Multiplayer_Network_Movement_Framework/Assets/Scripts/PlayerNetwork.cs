using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerNetwork : NetworkPackageController
{
    [SerializeField]
    float moveSpeed;

    [SerializeField]
    [Range(0.1f, 1)]
    float networkSendRate = .5f;

    CharacterController controller;

    List<ReceivePackage> predictedPackages;
    Vector3 lastPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();

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
            PackageManager.AddPackage(new Package
            {
                horizontal = Input.GetAxis("Horizontal"),
                vertical = Input.GetAxis("Vertical"),
                timeStamp = time
            });
        }

        //               Move(Input.GetAxis("Horizontal") * moveSpeed, Input.GetAxis("Vertical") * moveSpeed);

    }

    void ServerUpdate() {
        Debug.Log("ServerUpdate");
        if(!isServer || isLocalPlayer) {
            return;
        }
        Debug.Log("authorizde");

        Package packageData = PackageManager.GetNextDataReceived();
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

        ServerPackageManager.AddPackage(new ReceivePackage
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

        transform.position = new Vector3(data.x, data.y, data.z);
        
    }

}
