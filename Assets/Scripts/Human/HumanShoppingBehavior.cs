using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;


public class UserStats {
    public float TimeSpent = 0f;
    public int FightCnt = 0;
    public int PunchCnt = 0;
    public float AvgSpeed = 0f;
    public float TotalDistance = 0f; //Total distance traversed in the store
    public int CollectedItemCnt = 0; //Items collected from the shelves
    public int StolenItemCnt = 0; //Total item cnt
    public int FinalItemCnt = 0; //Total item cnt

    public UserStats() {
        TimeSpent = Time.time;
    }

}

public class HumanShoppingBehavior : MonoBehaviour {
    public GameObject CurrentObjs;
    public Transform _rightHand, _leftHand;

    public UserStats Stats;

   
    public Text IpadCntText;

    public Text WarningText;
    Animator _animator;

    HumanComponent _humanComponent;
    List<Collider> _ipadColliders;

    [SerializeField]
    string _currSceneName;

    string _missionMsg = "";

    Transform _desiredObj;
    int _missionItemCnt = 5; //number of iPads to collect
                              // Start is called before the first frame update

    bool _hasPaid, _hasCompletedShopping;


    Vector3 _prevPosition;
    
    int _fixedUpdateCnt = 0;



    [DllImport("__Internal")]
    private static extern void SendUserStatsToPage(float timeSpent, int fightCnt, int punchCnt, float avgSpeed, float totalDist, int collectedItemCnt, int stolenItemCnt, int finalItemCnt, int crowdPersonality);
    [DllImport("__Internal")]
    private static extern void SendMissionMessageToPage(string message);

   


    void SummarizeStats() {
        Stats.TimeSpent = Time.time - Stats.TimeSpent; //Timespent is initiated to the beginning time
        Stats.FinalItemCnt = CurrentObjs.transform.childCount;

        Stats.AvgSpeed /= _fixedUpdateCnt;
        Stats.FightCnt = GetComponent<HumanComponent>().FightCnt;

        //Debug.Log(Stats.TimeSpent);
        //Debug.Log(Stats.CollectedItemCnt);
        //Debug.Log(Stats.AvgSpeed);
        //Debug.Log(Stats.FightCnt);

#if !UNITY_EDITOR && UNITY_WEBGL
        SendUserStatsToPage(Stats.TimeSpent, Stats.FightCnt, Stats.PunchCnt, Stats.AvgSpeed,  Stats.TotalDistance, Stats.CollectedItemCnt, Stats.StolenItemCnt, Stats.FinalItemCnt, UserInfo.PersonalityDistribution );
#endif

    }
    void Start() {

       


        Stats = new UserStats();
        _prevPosition = transform.position;

        CurrentObjs = new GameObject("AchievedObjects");
        CurrentObjs.transform.parent = this.transform;
        //_animator = transform.Find("FpsArm2").GetComponent<Animator>();
        _animator = GetComponent<Animator>();


        _currSceneName = SceneManager.GetActiveScene().name;

        _hasCompletedShopping = false;
        _hasPaid = false;

    
        if(_currSceneName == "Warmup") {
            _missionMsg = "Walk around the store and collect 5 iPads.\n Pay at the counter when finished.";
#if !UNITY_EDITOR && UNITY_WEBGL
            SendMissionMessageToPage(_missionMsg);
#endif
        }

        else { //Actual shopping scenario
            _missionMsg = ""; // "Walk around the store and grab as many iPads as you can. \n Pay at the counter when finished.";

#if !UNITY_EDITOR && UNITY_WEBGL
            SendMissionMessageToPage(_missionMsg);
#endif

            
            if(UserInfo.PersonalityDistribution == 1) {  //Hostile

                float[] persMean = { 0f, -1f, 1f, -1f, 1f };
                float[] persStd = { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
                UpdatePersonalityAndBehavior(persMean, persStd);
            }
            else {

                float[] persMean = { 0f, 1f, -1f, 1f, -1f };
                float[] persStd = { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
                UpdatePersonalityAndBehavior(persMean, persStd);

            }
        }

        _ipadColliders = new List<Collider>();
        _humanComponent = GetComponent<HumanComponent>();

        _rightHand = transform.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
        _leftHand = transform.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftHand);


    }

    private void FixedUpdate() {
        Stats.TotalDistance += (transform.position - _prevPosition).magnitude;
        Stats.AvgSpeed += (transform.position - _prevPosition).magnitude / Time.deltaTime;
        _prevPosition = transform.position;
        _fixedUpdateCnt++;

    }

    public void UpdatePersonalityAndBehavior(float[] persMean, float[] persStd) {
        AgentComponent[] agentComponents = GameObject.Find("CrowdGroup1").GetComponentsInChildren<AgentComponent>();

        for(int i = 0; i < agentComponents.Length; i++) {

            if(agentComponents[i] != null) {
                agentComponents[i].GetComponent<AffectComponent>().UpdatePersonality(persMean, persStd);
                //Update steering behaviors according to Personality parameters
                agentComponents[i].GetComponent<PersonalityMapper>().PersonalityToSteering();
            }
        }
    }

    public bool IsWorthFighting() {
        //if we have any ipads
        return (CurrentObjs.transform.childCount > 0);
    }

    //public void ReceiveUserIdFromPage(string userId) {
    //    //Ready to run
    //    UserInfo.UserId = userId;

    //    if(userId!="")
    //        SceneManager.LoadScene("UserDemographics");
    //}

    

    IEnumerator DisplayEndOfWarmupScene() {
        {
    
            _missionMsg = "Warmup complete. \nPlease accept the HIT if you'd like to continue.";
            //check if user accepted the hit

#if !UNITY_EDITOR && UNITY_WEBGL
            SendMissionMessageToPage(_missionMsg);
            
#endif
            yield return new WaitForSeconds(2);

            //SceneManager.LoadScene("Sales");
            SceneManager.LoadScene("UserDemographics");
        }

    }
    IEnumerator DisplayEndOfShoppingScene() {
    
        _missionMsg = "Shopping complete. \nPlease answer the questions in the following scene. ";
        SummarizeStats();
#if !UNITY_EDITOR && UNITY_WEBGL
        SendMissionMessageToPage(_missionMsg);
#endif
        yield return new WaitForSeconds(2);

        SceneManager.LoadScene("UserPostStudySurvey");
        
    }


    private void OnTriggerEnter(Collider collider) {
        //Assign the closest ipad
        if(collider.CompareTag("Ipad")) {
            _ipadColliders.Add(collider);
        }
   
        //else if(collider.gameObject.CompareTag("ExitZone")) {
        //    if(_hasPaid) 
        //        _hasCompletedShopping = true;            
        //}


    }

    private void OnTriggerStay(Collider collider) {

        if(collider.CompareTag("PaymentZone")) {
            if(Input.GetKey(KeyCode.P)) {

                if(_currSceneName == "Warmup" && CurrentObjs.transform.childCount >= _missionItemCnt || _currSceneName == "Sales") {
                //if(_currSceneName == "Warmup" &&  _currSceneName == "Sales") {
                _hasPaid = true;
                _hasCompletedShopping = true;
                _missionMsg = "Payment complete. \nPlease answer the questions in the following scene.";

                collider.transform.Find("Dialog").gameObject.SetActive(true);
                }
                else {
                    _missionMsg = "Please collect all the " + _missionItemCnt + " iPads first.";


                }
            }
            
#if !UNITY_EDITOR && UNITY_WEBGL
            SendMissionMessageToPage(_missionMsg);
#endif

        }

    }

private void OnTriggerExit(Collider collider) {

        if(collider.CompareTag("PaymentZone"))
            collider.transform.Find("Dialog").gameObject.SetActive(false);

        //Assign the closest ipad
        if(_ipadColliders.Contains(collider)) {
            _ipadColliders.Remove(collider);
        }
        
        
    }



    // Update is called once per frame
    void Update() {

        //Boss key
        if(Input.GetKey(KeyCode.B) && Input.GetKey(KeyCode.Keypad6) && Input.GetKey(KeyCode.LeftControl)) {
            SceneManager.LoadScene("UserPostStudySurvey");
        }

        
        IpadCntText.text = CurrentObjs.transform.childCount.ToString();

        if(_hasCompletedShopping){
            if(_currSceneName == "Sales")
                StartCoroutine(DisplayEndOfShoppingScene());
            else {
                StartCoroutine(DisplayEndOfWarmupScene());
            }
        }


        if(_humanComponent.IsFighting()) {
            CurrentObjs.SetActive(false); //don't show items when fighting
            if(!_humanComponent.IsFightStarter)
                WarningText.text = "You are being attacked by someone!";
        }
        else {
            CurrentObjs.SetActive(true); //don't show items when fighting
            WarningText.text = "";
        }


        

        
        if(Input.GetKey(KeyCode.C)) {
            if(!_desiredObj) { //update only if the object is not being taken
                float minDist = 10000;
                foreach(Collider col in _ipadColliders) {
                    if(!col.GetComponent<ObjComponent>().Achieved) {
                        float dist = Vector3.Distance(col.transform.position, transform.position);
                        if(dist < minDist) {
                            minDist = dist;
                            _desiredObj = col.transform;
                        }

                    }
                }
            }
        }

        //human is inside the object's trigger zone
        if(_desiredObj) {
            if(_humanComponent.StartedWaiting == false) {

                _animator.SetTrigger("PickUp");

                Stats.CollectedItemCnt++;

                _humanComponent.HandPos = _desiredObj.position;


                _humanComponent.StartedWaiting = true;
                _humanComponent.FinishedWaiting = false;




            }
            else if(_humanComponent.StartedWaiting && !_humanComponent.FinishedWaiting && _desiredObj.GetComponent<ObjComponent>().Achieved == false) { //started waiting -- playing animation
                if(_desiredObj.GetComponent<ObjComponent>().ClosestAgent.Equals(this.gameObject)) {
                    _desiredObj.position = _rightHand.position + Vector3.up * 0.05f;
                }

                //_humanComponent.HandPos = _desiredObj.position;//+ Vector3.up * 0.1f;

            }

            if(_humanComponent.FinishedWaiting) {

                if(_desiredObj.GetComponent<ObjComponent>().Achieved == false) { //make sure someone else didn't pick it before me                

                    _desiredObj.parent = CurrentObjs.transform;
                    _desiredObj.position = CurrentObjs.transform.position - 0.05f * Vector3.up + (CurrentObjs.transform.childCount - 1) * 0.05f * Vector3.up;

                    _desiredObj.GetComponent<ObjComponent>().AchievingAgent = this.gameObject;
                    _desiredObj.GetComponent<ObjComponent>().Achieved = true;


                    _ipadColliders.Remove(_desiredObj.GetComponent<Collider>());
                }


                _humanComponent.StartedWaiting = false;


                _desiredObj = null;
            }

        }

    }

    //Arrange positions of object
    public void SortObjects() {
        for(int i = 0; i < CurrentObjs.transform.childCount; i++)
            CurrentObjs.transform.GetChild(i).transform.position = CurrentObjs.transform.position - 0.05f * Vector3.up + i * 0.05f * Vector3.up;

    }


    //Give all your objects to your opponent who is always an agent
    public void YieldObjects(GameObject opponent) {

        List<Transform> children = new List<Transform>();
        int childCnt = CurrentObjs.transform.childCount;
        for(int i = 0; i < childCnt; i++) 
            children.Add(CurrentObjs.transform.GetChild(i));

        foreach(Transform c in children) {
            c.parent = opponent.GetComponent<ShopperBehavior>().CurrentObjs.transform;        
        }

        opponent.GetComponent<ShopperBehavior>().SortObjects();
        CurrentObjs.SetActive(true);

        Debug.Log("yielded objects");
    }

    private void OnGUI() {
        GUIStyle style = new GUIStyle();

        if(!_humanComponent.IsFighting()) {
            GUILayout.BeginArea(new Rect(x: 400, y: 10, width: 600, height: 300));

            //style.normal.textColor = Color.Lerp(Color.red, Color.blue, 0.3f);
            style.normal.textColor = Color.black;
            style.fontSize = 20;

            GUILayout.Label(_missionMsg, style); 
            GUILayout.EndArea();
        }


    }


    //Not working properly for human
    void OnAnimatorIK() {
        if(_animator.GetCurrentAnimatorStateInfo(2).IsName("pickingUp")) {
            if(_desiredObj) {
                float reach = _animator.GetFloat("RightHandReach");  //param is a curve set in pickingUp animation


                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, reach);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, reach);
                _animator.SetIKPosition(AvatarIKGoal.RightHand, _desiredObj.position);
                _animator.SetIKRotation(AvatarIKGoal.RightHand, _desiredObj.rotation);
            }

        }

    }

    //Called as an animation event when the picking up animation ends
    void PickedObject() {
        _humanComponent.FinishedWaiting = true;

    }

    private void LateUpdate() {
        CurrentObjs.transform.position = _leftHand.position;
    }




}
