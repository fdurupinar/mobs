using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    //private GameObject[] AIPanel;
    //List<GameObject> AIPanel;
    public GameObject AIPanel;

    void Start()
    {
        AIPanel = GameObject.Find("AIPanel");
        //AIPanel = new GameObject[4];
        //AIPanel = GameObject.FindGameObjectsWithTag("AIPanel");
        //AIPanel = new List<GameObject>();
        //Debug.Log(AIPanel == null);
        AIPanel.SetActive(false);

        /*
        for (int i = 0; i < AIPanel.Length; i++)
        {
            AIPanel[i].SetActive(false);
        }*/
        //AIPanel.SetActive(false);
        
    }

    public void AIEnter()
    {
        AIPanel.SetActive(true);
        //AIPanel = GameObject.Find("AIPanel");
        /*
        for (int i = 0; i < AIPanel.Length; i++)
        {
            AIPanel[i].SetActive(true);
        }*/
    }

    public void AIExit()
    {
        AIPanel.SetActive(false);
        /*
        for (int i = 0; i < AIPanel.Length; i++)
        {
            AIPanel[i].SetActive(false);
        }*/
    }
}