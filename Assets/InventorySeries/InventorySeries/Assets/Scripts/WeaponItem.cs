using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : MonoBehaviour {

    private Transform playerPos;
    public GameObject sword;

    private void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("RealPlayer").transform;
    }

    public void Use() {

        Instantiate(sword, playerPos.position, sword.transform.rotation, playerPos.transform);
        Destroy(gameObject);
    }
}
