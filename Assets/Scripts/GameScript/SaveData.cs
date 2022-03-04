using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Score : MonoBehaviour
{
    public string baseURL = "http://localhost:3000/";
    string path;
    string jsonString;
    
    void Start()
    {
        path = Application.dataPath + "/WorkerRecords.json";
        jsonString = File.ReadAllText(path);
        ListaRecords listaRecords = JsonUtility.FromJson<ListaRecords>(jsonString);
        print(listaRecords);
        foreach (Record record in listaRecords)
        {
            Debug.Log("ID: " + record.workerId + "Time: " + record.time);
        }

    }

    void Update()
    {

    }
}


[System.Serializable]
public class Record
{
    public string workerId;
    public int time;
}

[System.Serializable]
public class ListaRecords
{
    public List<Record> Records;
}


//https://answers.unity.com/questions/1473952/how-to-write-and-read-json-in-unity.html