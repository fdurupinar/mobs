using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    private GameObject Main; 

    void Start()
    {
        Main = GameObject.Find("Main");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RealPlayer"){
            //&& Input.GetKeyDown("i"))
            Main.GetComponent<MainScript>().AIEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "RealPlayer") {
            Main.GetComponent<MainScript>().AIExit();
        }
    }
}