using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public float MaxTime = 60f;

    [SerializeField]
    private float CountDown = 0;
    // Start is called before the first frame update
    void Start()
    {
        CountDown = MaxTime;
    }

    // Update is called once per frame
    void Update()
    {
        CountDown -= Time.deltaTime;
        if (CountDown <= 0) {
        	Collection.ProductCount = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("You Lost");
        }
    }
}
