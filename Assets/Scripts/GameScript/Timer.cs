using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public float MaxTime = 300f;

    [SerializeField]
    private float CountDown = 0f;
    // Start is called before the first frame update
    void Start()
    {
        CountDown = MaxTime;
    }

    // Update is called once per frame
    void Update()
    {
        CountDown -= Time.deltaTime;
        if (CountDown <= 0f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("You Lost");
        }
    }
}