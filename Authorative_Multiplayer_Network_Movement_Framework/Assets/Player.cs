using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Client client;

    void Start()
    {
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Translate(horizontal, 0, vertical);

        if(horizontal != 0 || vertical != 0) {
            client.sendMessage("MOVE|" + horizontal.ToString() + "|" + vertical.ToString());
        }
    }

}
