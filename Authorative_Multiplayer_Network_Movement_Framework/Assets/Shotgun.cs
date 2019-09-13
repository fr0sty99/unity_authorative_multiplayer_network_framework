using UnityEngine;
using System.Collections;
using System.Security.Cryptography;

[RequireComponent(typeof(Transform))]
[System.Serializable]
public class Shotgun
{
    public string name = "Shotgun";
    public GameObject gunPrefab;

    public float fireRate = 1;
    public int damage = 10;
    public float range = 10f;
    public Transform firePoint;
}
