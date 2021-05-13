using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPManager : MonoBehaviour
{
    public static int hp = 5;

    public GameObject Heart1;
    public GameObject Heart2;
    public GameObject Heart3;
    public GameObject Heart4;
    public GameObject Heart5;

    // Start is called before the first frame update
    void Start()
    {
        Heart1.GetComponent<Image>().enabled = true;
        Heart2.GetComponent<Image>().enabled = true;
        Heart3.GetComponent<Image>().enabled = true;
        Heart4.GetComponent<Image>().enabled = true;
        Heart5.GetComponent<Image>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        switch (hp) {
            case 4:
                Heart5.GetComponent<Image>().enabled = false;
                break;
            case 3:
                Heart4.GetComponent<Image>().enabled = false;
                break;
            case 2:
                Heart3.GetComponent<Image>().enabled = false;
                break;
            case 1:
                Heart2.GetComponent<Image>().enabled = false;
                break;
            case 0:
                Heart1.GetComponent<Image>().enabled = false;
                break;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Target") {
            Debug.Log("Collide");
            HPManager.hp -= 1;
            Destroy(gameObject);
        }
    }
}
