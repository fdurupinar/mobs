using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    public int[] items;
    public bool[] isFull;
    public GameObject[] slots;
    public Sprite[] sprites;
    public Sprite box;

    private void Start()
    {
        sprites[0] = Resources.Load<Sprite>("Ipad_Sprite");
        sprites[1] = Resources.Load<Sprite>("Laptop_Sprite");
        sprites[2] = Resources.Load<Sprite>("TV_Sprite");
        box = Resources.Load<Sprite>("Inventory_Icons");
    }
}
