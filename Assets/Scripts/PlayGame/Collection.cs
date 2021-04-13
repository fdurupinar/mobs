using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour
{
    public static int ProductCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        Collection.ProductCount += 1;
    }

    private void OnTriggerEnter(Collider other) {
    	//  && Input.GetKeyDown("C")
        if(other.CompareTag("RealPlayer")){
            Debug.Log("Collide");
            Destroy(gameObject);
    	}
    }

    private void OnDestroy() {
    	--Collection.ProductCount;
    	if (Collection.ProductCount <= 0) {
            GameObject timer = GameObject.Find("LevelTimer");
            Destroy(timer);
            GameObject[] Fireworksystem = GameObject.FindGameObjectsWithTag("Fireworks");
            foreach (GameObject go in Fireworksystem) {
                go.GetComponent<ParticleSystem>().Play();
            }
    	}
    }

}
