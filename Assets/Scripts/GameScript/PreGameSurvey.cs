using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreGameSurvey : MonoBehaviour
{
    public GameObject[] questionArr;
    public QAClass[] qaArr;
    //public GameObject question;
    //public GameObject answer;
    private string Question;
    private string Answer;
    private string[] Lines;
    private string form;
    private string foldername;

    void Start()
    {
        qaArr = new QAClass[questionArr.Length];
    }

    void Update()
    {
       //Answer = answer.GetComponent<Toggle>().isOn;
    }

    public void SubmitAnswer() {
        /*
        for (int i = 0; i < qaArr.Length; i++)
        {
            qaArr[i] = ReadQuestionAndAnswer(questionArr[i]);
        }
        */
        for (int i = 0; i < qaArr.Length; i++)
        {
            if (!System.IO.File.Exists(@"/Users/jiehyun/Jenna/UMassBoston/Research/game_survey" + i + ".txt"))
            {
                for (i = 0; i < qaArr.Length; i++)
                {
                    qaArr[i] = ReadQuestionAndAnswer(questionArr[i]);
                    form = (ReadQuestionAndAnswer(questionArr[i]).Question + "\n" + ReadQuestionAndAnswer(questionArr[i]).Answer);
                    System.IO.File.WriteAllText(@"/Users/jiehyun/Jenna/UMassBoston/Research/game_survey" + i + ".txt", form);
                    Debug.Log("File Saved");
                }
                //form = (ReadQuestionAndAnswer(questionArr[i]).Question + "\n" + ReadQuestionAndAnswer(questionArr[i]).Answer);
                //System.IO.File.WriteAllText(@"/Users/jiehyun/Jenna/UMassBoston/Research/game_survey"+ i + ".txt", form);
                //Debug.Log("File Saved");
                Application.LoadLevel("Sales");
            }
        }
    }

    QAClass ReadQuestionAndAnswer(GameObject question)
    {
        QAClass result = new QAClass();

        GameObject q = question.transform.Find("Question").gameObject;
        GameObject a = question.transform.Find("Answer").gameObject;

        result.Question = q.transform.Find("Text").GetComponent<Text>().text;

        //Toggle group Answer
        if (a.GetComponent<ToggleGroup>() != null)
        {
            for (int i = 0; i < a.transform.childCount; i++)
            {
                if (a.transform.GetChild(i).GetComponent<Toggle>().isOn)
                {
                    result.Answer = a.transform.GetChild(i).Find("Text").GetComponent<Text>().text;
                    break;
                }
            }
        }
        return result;      
    }

}

[System.Serializable]
public class QAClass {
    public string Question = "";
    public string Answer = "";
}