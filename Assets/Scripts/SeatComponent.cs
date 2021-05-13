using UnityEngine;
using System.Collections;

public class SeatComponent : MonoBehaviour {

	// Use this for initialization
    bool isPicked = false;

    public bool IsPicked
    {
        get { return isPicked; }
        set { isPicked = value; }
    }

	void Start () {
        isPicked = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
