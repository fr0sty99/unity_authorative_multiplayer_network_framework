using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerNetwork : MonoBehaviour
{
    [SerializeField]
    float moveSpeed;

    CharacterController controller;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        LocalClientUpdate();
    }

    void LocalClientUpdate() {
        Move(Input.GetAxis("Horizontal") * moveSpeed, Input.GetAxis("Vertical") * moveSpeed);
    }

    void Move(float horizontal, float vertical)
    {
        controller.Move(new Vector3(horizontal, 0, vertical));
    }
}
