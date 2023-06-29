
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class UserPersonalitySurveyGUI : MonoBehaviour {

	// static string[] _tipi = {" Extraverted, enthusiastic", "Critical, quarrelsome", "Dependable, self-disciplined", "Anxious, easily upset", "Open to new experiences, complex", "Reserved, quiet", "Sympathetic, warm", "Disorganized, careless", "Calm, emotionally stable", "Conventional, uncreative" }; 
	

	public Transform[] QInput = new Transform[10];
	public Transform AttentionObj;

	

	int _attentionAnswer;
	[DllImport("__Internal")]
	private static extern void SendSurveyToPage(string surveyType, int quality, float[] responses, int size);

	private void Start() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		AttentionObj = GameObject.Find("QAttention").transform;
		
		for(int i = 0; i < 10; i++)
			UserInfo.TIPI[i] = 2;
	}

	public void UpdateTIPI(int qNumber) {

		UserInfo.TIPI[qNumber] = (float)QInput[qNumber].Find("Slider").GetComponent<Slider>().value;

		//Debug.Log(qNumber + " " + UserInfo.TIPI[qNumber]);
	}
	public void Submit() {
		if(_attentionAnswer == 4) {
			UserInfo.Quality++;
		}
		else {
			UserInfo.Quality--;
		}

		

#if !UNITY_EDITOR && UNITY_WEBGL
		SendSurveyToPage("personality", UserInfo.Quality, UserInfo.TIPI, UserInfo.TIPI.Length);
#endif

		SceneManager.LoadScene("UserPreStudySurvey");

    }

	public void UpdateAttention() {

		_attentionAnswer = (int)AttentionObj.Find("Slider").GetComponent<Slider>().value;
		
	}

}

	