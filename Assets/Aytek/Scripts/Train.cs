using UnityEngine;
using System.Collections;

public class Train : MonoBehaviour {

    public GameObject rightDoor1, rightDoor2;
    public GameObject leftDoor1, leftDoor2;
    public float doorDist = 3;

    Vector3 rightDoor1Target, rightDoor2Target, leftDoor1Target, leftDoor2Target;

    bool isOpening = false;

	// Use this for initialization
	void Start () {
        rightDoor1Target = rightDoor1.transform.position + Vector3.right * doorDist;
        rightDoor2Target = rightDoor2.transform.position + Vector3.right * doorDist;
        leftDoor1Target = leftDoor1.transform.position + -Vector3.right * doorDist;
        leftDoor2Target = leftDoor2.transform.position + -Vector3.right * doorDist;

        Invoke("OpenDoors", 1);
        Invoke("EndOpenDoors", 3);
	}

    // Update is called once per frame
    void Update()
    {
        if (isOpening)
        {
            rightDoor1.transform.position = Vector3.Lerp(rightDoor1.transform.position, rightDoor1Target, Time.deltaTime * 1f);
            rightDoor2.transform.position = Vector3.Lerp(rightDoor2.transform.position, rightDoor2Target, Time.deltaTime * 1f);
            leftDoor1.transform.position = Vector3.Lerp(leftDoor1.transform.position, leftDoor1Target, Time.deltaTime * 1f);
            leftDoor2.transform.position = Vector3.Lerp(leftDoor2.transform.position, leftDoor2Target, Time.deltaTime * 1f);
        }
    }

    void OpenDoors()
    {
        isOpening = true;
    }

    void EndOpenDoors()
    {
        isOpening = false;
    }
}
