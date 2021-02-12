using UnityEngine;
using System.Collections;

public class UpdateProtestCenter : MonoBehaviour
{
    // Use this for initialization
    AgentComponent[] agentComponents;

    void Start()
    {
        agentComponents = FindObjectsOfType(typeof(AgentComponent)) as AgentComponent[];
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 location = Vector3.zero;
        int cnt = 0;
        
        foreach (AgentComponent a in agentComponents) {
            if (a.GetComponent<ProtesterBehavior>()!= null) {
                location += a.transform.position;
                cnt++;
            }
        }

        if(cnt > 0) {
            location /= cnt;
            transform.Translate(location - transform.position);
        }
    }
}