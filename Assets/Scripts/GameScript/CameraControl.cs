using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    float speed = 2.0f;
    float vspeed = 0.5f;

    void Start() {

    }

    void Update()
    {
        movingCamera();
    }

    void movingCamera() {
        float horizontal = speed * Input.GetAxis("Horizontal");
        float vertical = speed * Input.GetAxis("Vertical");

        transform.Translate(Vector3.right * speed * Time.deltaTime * horizontal, Space.World);
        transform.Translate(Vector3.forward * speed * Time.deltaTime * vertical, Space.World);

        if (Input.GetKey("q")){
            transform.Translate(Vector3.up * vspeed * Time.deltaTime * 90, Space.World);
        }

        if (Input.GetKey("e"))
        {
            transform.Translate(Vector3.up * -vspeed * Time.deltaTime * 90, Space.World);
        }
    }

}
