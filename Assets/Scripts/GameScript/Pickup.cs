using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour {

    private Inventory inventory;
    public GameObject itemButton;
    //public Sprite m_Sprite_1;
    //public Sprite m_Sprite_2;
    //public Sprite m_Sprite_3;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("RealPlayer").GetComponent<Inventory>();
        //m_Sprite_1 = Resources.Load<Sprite>("Ipad_Sprite");
        //m_Sprite_2 = Resources.Load<Sprite>("Laptop_Sprite");
        //m_Sprite_3 = Resources.Load<Sprite>("TV_Sprite");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RealPlayer") && Input.GetKey(KeyCode.G)) {

            for (int i = 0; i < inventory.slots.Length; i++)
            {
                if (inventory.isFull[i] == false) { // check whether the slot is EMPTY
                    Instantiate(itemButton, inventory.slots[i].transform, false);

                    if (this.gameObject.CompareTag("Ipad"))
                    {
                        inventory.slots[i].GetComponent<Image>().sprite = inventory.sprites[0];
                    }
                    else if (this.gameObject.CompareTag("Laptop"))
                    {
                        inventory.slots[i].GetComponent<Image>().sprite = inventory.sprites[1];
                    }
                    else if (this.gameObject.CompareTag("TV"))
                    {
                        inventory.slots[i].GetComponent<Image>().sprite = inventory.sprites[2];

                    }
                    //Debug.Log("Collide 2");
                    Destroy(gameObject);

                    inventory.isFull[i] = true;
                    break;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (inventory.isFull[4])
        {
            Debug.Log("You are win!");
            GameObject timer = GameObject.Find("LevelTimer");
            Destroy(timer);
            GameObject[] Fireworksystem = GameObject.FindGameObjectsWithTag("Fireworks");
            foreach (GameObject go in Fireworksystem)
            {
                go.GetComponent<ParticleSystem>().Play();
            }
        }
        else if (Time.timeSinceLevelLoad > 300) {
            Debug.Log("You are lost!");
        }
    }
}
