using System;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class UserPANASStudySurveyGUI : MonoBehaviour
{
	

	public GameObject[] QInput = new GameObject[10];
	float[] _panasAnswers = new float[10];

	int _attentionAnswer;
	//float _qHeight = 28;

	public bool IsPreStudy;
	public Transform AttentionObj;

	[DllImport("__Internal")]
	private static extern void SendSurveyToPage(string surveyType, int quality, float[] responses, int size, int crowdPrsonality);

	[DllImport("__Internal")]
	private static extern void SendCompletedToPage();

	private void Start() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		AttentionObj = GameObject.Find("QAttention").transform;

		for(int i = 0; i < _panasAnswers.Length; i++)
			_panasAnswers[i] = 2;

	}


	public void UpdatePANAS(int qNumber) {

		_panasAnswers[qNumber] = (float)QInput[qNumber].transform.Find("Slider").GetComponent<Slider>().value;

		//Debug.Log(qNumber + " " + _panasAnswers[qNumber]);
	}

	public void Submit() {
		if(_attentionAnswer == 4) {
			
			UserInfo.Quality++;
		}
		else {
			UserInfo.Quality--;
		}

		Debug.Log(UserInfo.Quality);

		if(IsPreStudy) {
#if !UNITY_EDITOR && UNITY_WEBGL

		SendSurveyToPage("preStudy", UserInfo.Quality, _panasAnswers, _panasAnswers.Length, UserInfo.PersonalityDistribution);
#endif

			//SceneManager.LoadScene("Warmup");
			SceneManager.LoadScene("Sales");
		}

        else {
#if !UNITY_EDITOR && UNITY_WEBGL

		SendSurveyToPage("postStudy",  UserInfo.Quality,  _panasAnswers, _panasAnswers.Length, UserInfo.PersonalityDistribution);
		SendCompletedToPage();
#endif

			SceneManager.LoadScene("End");

		}
    }

	public void UpdateAttention() {

		_attentionAnswer = (int)AttentionObj.Find("Slider").GetComponent<Slider>().value;

		
	}



}
