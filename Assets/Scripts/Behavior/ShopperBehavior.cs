//#define WAITING_AT_ENTRANCE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ShopperBehavior : MonoBehaviour {
    Appraisal _appraisal;
    AgentComponent _agentComponent;
    AffectComponent _affectComponent;
    UnityEngine.AI.NavMeshAgent _navmeshAgent;
    GameObject _counter;
    GameObject _cashier;
    private Transform _desiredObj;
    //public List<Transform> CurrentObjs = new List<Transform>();
    public GameObject CurrentObjs;
    Vector3 _exit;
    GameObject _objs;
    private GameObject[] _allObjs;

    public int _desiredObjCnt; 
    int _acquiredObjCnt;


    private ShelfComponent _shelfComp;
    int _totalObjCnt = 0;
    bool _allConsumed = false;

    public Transform _rightHand, _leftHand;


    private bool _finishedWaitingAtEntrance;

    private Vector3 _closestShelfPos;
    private int _shelfInd;
    int[] _shelfOrder; //shelf visiting order
    private AnimationSelector _animationSelector;
    private GUIHandler _guiHandler;

    [SerializeField]
    private int _state;    
    public int State {
        get { return _state; }
        set { _state = value; }
    }

    public enum ShoppingState {
        ShelfChanging,
        Paying,
        WaitingInLine,
        GoingToLine,
        GoingToObject,
        PickingUpObject,
        Exiting
    }


    // Use this for initialization
	void Start ()  {
        _appraisal = GetComponent<Appraisal>();
        _agentComponent = GetComponent<AgentComponent>();
        _affectComponent = GetComponent<AffectComponent>();
        _navmeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _animationSelector = GetComponent<AnimationSelector>();
        _shelfComp = GameObject.Find("Shelves").GetComponent<ShelfComponent>();


        _guiHandler = FindObjectOfType(typeof(GUIHandler)) as GUIHandler;

	    _allObjs = new GameObject[_shelfComp.transform.childCount];
        for (int i = 0; i < _allObjs.Length; i++)
            _allObjs[i] = _shelfComp.transform.GetChild(i).gameObject;


     //   _navmeshAgent.radius -= 0.1f; //smaller than regular size
        //_navmeshAgent.speed += 0.6f; //faster than regular speed

        //_navmeshAgent.radius *= 2f;//0.5f;

        _exit = GameObject.Find("Exit").transform.position;

        _counter = GameObject.Find("counter"); 
        _cashier = GameObject.Find("cashier");
       // _objs = GameObject.Find("Objects");
        _leftHand = transform.Find("Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm/LeftForeArm/LeftHand/LeftHandIndex1");
        _rightHand = transform.Find("Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/RightHandIndex1");


        if (!_leftHand) {
            _leftHand = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand/mixamorig:LeftHandIndex1");
            _rightHand = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/mixamorig:RightHandIndex1");

        }

        if (!_leftHand) {
            _leftHand = transform.Find("mixamorig1:Hips/mixamorig1:Spine/mixamorig1:Spine1/mixamorig1:Spine2/mixamorig1:LeftShoulder/mixamorig1:LeftArm/mixamorig1:LeftForeArm/mixamorig1:LeftHand/mixamorig1:LeftHandIndex1");
            _rightHand = transform.Find("mixamorig1:Hips/mixamorig1:Spine/mixamorig1:Spine1/mixamorig1:Spine2/mixamorig1:RightShoulder/mixamorig1:RightArm/mixamorig1:RightForeArm/mixamorig1:RightHand/mixamorig1:RightHandIndex1");

        }

        if (!_leftHand) {
            _leftHand = transform.Find("mixamorig2:Hips/mixamorig2:Spine/mixamorig2:Spine1/mixamorig2:Spine2/mixamorig2:LeftShoulder/mixamorig2:LeftArm/mixamorig2:LeftForeArm/mixamorig2:LeftHand/mixamorig2:LeftHandIndex1");
            _rightHand = transform.Find("mixamorig2:Hips/mixamorig2:Spine/mixamorig2:Spine1/mixamorig2:Spine2/mixamorig2:RightShoulder/mixamorig2:RightArm/mixamorig2:RightForeArm/mixamorig2:RightHand/mixamorig2:RightHandIndex1");

        }

        if (!_leftHand) {
            _leftHand = transform.Find("mixamorig4:Hips/mixamorig4:Spine/mixamorig4:Spine1/mixamorig4:Spine2/mixamorig4:LeftShoulder/mixamorig4:LeftArm/mixamorig4:LeftForeArm/mixamorig4:LeftHand/mixamorig4:LeftHandIndex1");
            _rightHand = transform.Find("mixamorig4:Hips/mixamorig4:Spine/mixamorig4:Spine1/mixamorig4:Spine2/mixamorig4:RightShoulder/mixamorig4:RightArm/mixamorig4:RightForeArm/mixamorig4:RightHand/mixamorig4:RightHandIndex1");

        }
        if (!_leftHand) {
            _leftHand = transform.Find("mixamorig6:Hips/mixamorig6:Spine/mixamorig6:Spine1/mixamorig6:Spine2/mixamorig6:LeftShoulder/mixamorig6:LeftArm/mixamorig6:LeftForeArm/mixamorig6:LeftHand/mixamorig6:LeftHandIndex1");
            _rightHand = transform.Find("mixamorig6:Hips/mixamorig6:Spine/mixamorig6:Spine1/mixamorig6:Spine2/mixamorig6:RightShoulder/mixamorig6:RightArm/mixamorig6:RightForeArm/mixamorig6:RightHand/mixamorig6:RightHandIndex1");

        }
        if (!_leftHand) {
            _leftHand = transform.Find("mixamorig7:Hips/mixamorig7:Spine/mixamorig7:Spine1/mixamorig7:Spine2/mixamorig7:LeftShoulder/mixamorig7:LeftArm/mixamorig7:LeftForeArm/mixamorig7:LeftHand/mixamorig7:LeftHandIndex1");
            _rightHand = transform.Find("mixamorig7:Hips/mixamorig7:Spine/mixamorig7:Spine1/mixamorig7:Spine2/mixamorig7:RightShoulder/mixamorig7:RightArm/mixamorig7:RightForeArm/mixamorig7:RightHand/mixamorig7:RightHandIndex1");

        }
        if (!_leftHand) {
            _leftHand = transform.Find("mixamorig8:Hips/mixamorig8:Spine/mixamorig8:Spine1/mixamorig8:Spine2/mixamorig8:LeftShoulder/mixamorig8:LeftArm/mixamorig8:LeftForeArm/mixamorig8:LeftHand/mixamorig8:LeftHandIndex1");
            _rightHand = transform.Find("mixamorig8:Hips/mixamorig8:Spine/mixamorig8:Spine1/mixamorig8:Spine2/mixamorig8:RightShoulder/mixamorig8:RightArm/mixamorig8:RightForeArm/mixamorig8:RightHand/mixamorig8:RightHandIndex1");

        }

        if (!_leftHand) {
            _leftHand = transform.Find("mixamorig9:Hips/mixamorig9:Spine/mixamorig9:Spine1/mixamorig9:Spine2/mixamorig9:LeftShoulder/mixamorig9:LeftArm/mixamorig9:LeftForeArm/mixamorig9:LeftHand/mixamorig9:LeftHandIndex1");
            _rightHand = transform.Find("mixamorig9:Hips/mixamorig9:Spine/mixamorig9:Spine1/mixamorig9:Spine2/mixamorig9:RightShoulder/mixamorig9:RightArm/mixamorig9:RightForeArm/mixamorig9:RightHand/mixamorig9:RightHandIndex1");

        }
        if (!_leftHand) {
            _leftHand = transform.Find("mixamorig10:Hips/mixamorig10:Spine/mixamorig10:Spine1/mixamorig10:Spine2/mixamorig10:LeftShoulder/mixamorig10:LeftArm/mixamorig10:LeftForeArm/mixamorig10:LeftHand/mixamorig10:LeftHandIndex1");
            _rightHand = transform.Find("mixamorig10:Hips/mixamorig10:Spine/mixamorig10:Spine1/mixamorig10:Spine2/mixamorig10:RightShoulder/mixamorig10:RightArm/mixamorig10:RightForeArm/mixamorig10:RightHand/mixamorig10:RightHandIndex1");

        }
        if (!_leftHand) {
            _leftHand = transform.Find("mixamorig11:Hips/mixamorig11:Spine/mixamorig11:Spine1/mixamorig11:Spine2/mixamorig11:LeftShoulder/mixamorig11:LeftArm/mixamorig11:LeftForeArm/mixamorig11:LeftHand/mixamorig11:LeftHandIndex1");
            _rightHand = transform.Find("mixamorig11:Hips/mixamorig11:Spine/mixamorig11:Spine1/mixamorig11:Spine2/mixamorig11:RightShoulder/mixamorig11:RightArm/mixamorig11:RightForeArm/mixamorig11:RightHand/mixamorig11:RightHandIndex1");

        }
        if (!_leftHand) {
            _leftHand = transform.Find("mixamorig12:Hips/mixamorig12:Spine/mixamorig12:Spine1/mixamorig12:Spine2/mixamorig12:LeftShoulder/mixamorig12:LeftArm/mixamorig12:LeftForeArm/mixamorig12:LeftHand/mixamorig12:LeftHandIndex1");
            _rightHand = transform.Find("mixamorig12:Hips/mixamorig12:Spine/mixamorig12:Spine1/mixamorig12:Spine2/mixamorig12:RightShoulder/mixamorig12:RightArm/mixamorig12:RightForeArm/mixamorig12:RightHand/mixamorig12:RightHandIndex1");

        }



        if (!_leftHand) {
            _leftHand = transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/LeftShoulder/LeftArm/LeftForeArm/LeftHand/LeftHandIndex1");
            _rightHand = transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/RightShoulder/RightArm/RightForeArm/RightHand/RightHandIndex1");
        }

        if (!_leftHand) {
            _leftHand = transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/LeftShoulder/LeftArm/LeftForeArm/LeftHand/LeftHandIndex1");
            _rightHand = transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/RightShoulder/RightArm/RightForeArm/RightHand/RightHandIndex1");
        }
  
        if (!_leftHand) {
            _leftHand = transform.Find("hip/abdomen/chest/lCollar/lShldr/lForeArm/lHand/lIndex1");
            _rightHand = transform.Find("hip/abdomen/chest/rCollar/rShldr/rForeArm/rHand/rIndex1");
        }
       

	    _acquiredObjCnt = 0;
        _desiredObjCnt = (int)(2f * _affectComponent.Personality[(int)OCEAN.E] + 5f); //correlated to extroversion [1 5]
        
        
        InitAppraisalStatus();

        _navmeshAgent.autoRepath = true;
        _navmeshAgent.autoBraking = true;

      


	    Random.seed = _agentComponent.Id;
        int randInd = Random.Range(0, 6);

        _shelfOrder = new int[6];
        if (randInd == 0){
            _shelfOrder[0] = 0; _shelfOrder[1] = 1; _shelfOrder[2] = 2; _shelfOrder[3] = 3; _shelfOrder[4] = 4; _shelfOrder[5] = 5;
        }
        else if (randInd == 1){
            _shelfOrder[0] = 1; _shelfOrder[1] = 2; _shelfOrder[2] = 3; _shelfOrder[3] = 4; _shelfOrder[4] = 5; _shelfOrder[5] = 0;
        }
        else if (randInd == 2){
            _shelfOrder[0] = 2; _shelfOrder[1] = 3; _shelfOrder[2] = 4; _shelfOrder[3] = 5; _shelfOrder[4] = 1; _shelfOrder[5] = 0;
        }
        else if (randInd == 3) {
            _shelfOrder[0] = 3; _shelfOrder[1] = 4; _shelfOrder[2] = 5; _shelfOrder[3] = 2; _shelfOrder[4] = 1; _shelfOrder[5] = 0;
        }
        else if (randInd == 4) {
            _shelfOrder[0] = 4; _shelfOrder[1] = 5; _shelfOrder[2] = 3; _shelfOrder[3] = 2; _shelfOrder[4] = 1; _shelfOrder[5] = 0;
        }
        else if (randInd == 5) {
            _shelfOrder[0] = 5; _shelfOrder[1] = 4; _shelfOrder[2] = 3; _shelfOrder[3] = 2; _shelfOrder[4] = 1; _shelfOrder[5] = 0;
        }
        

        _objs = GameObject.Find("Objects" + _shelfOrder[0]);


        State = (int)ShoppingState.GoingToObject;



	    _navmeshAgent.speed += 0.3f; //faster than usual


	    CurrentObjs = new GameObject("AchievedObjects");
	    CurrentObjs.transform.parent = this.transform;




	}

    public IEnumerator WaitAtEntrance(int seconds) {
        _finishedWaitingAtEntrance = false;
        yield return new WaitForSeconds(seconds);

        _finishedWaitingAtEntrance = true;


        //Dont't forget to set started waiting to false before state change
    }

    public void Restart() {
        InitAppraisalStatus();
        UpdateState();
        _acquiredObjCnt = 0;

        GameObject _objs = GameObject.Find("Objects" + _shelfOrder[0]);

        for (int i = 0; i < _objs.transform.childCount; i++) {
            _objs.transform.GetChild(i).gameObject.SetActive(true);
            _objs.transform.GetChild(i).gameObject.GetComponent<ObjComponent>().Achieved = false;
        }
    }
	


	void Update () {

        if (!_agentComponent.IsFighting()) {
            UpdateState();
            UpdateAppraisalStatus();



            CurrentObjs.SetActive(true);

            //Debug.Log("fighting");
            //Check fighting
            //Fight with disapproved agents
            //Disapproved agents are the ones who achieve my desired object before me
        /*    foreach (GameObject c in _agentComponent.CollidingAgents) {
                
                if (c.GetComponent<ShopperBehavior>() != null && _agentComponent.IsGoodToFight(c)) {                   
                    _agentComponent.StartFight(c, true);
                    c.GetComponent<AgentComponent>().StartFight(this.gameObject, false);
                    
                }

            }
            */
        }
        else if (GetComponent<FightBehavior>().Opponent.CompareTag("Player")) { //Fighting
            CurrentObjs.SetActive(false);
            //foreach (Transform co in CurrentObjs)
              //  co.gameObject.SetActive(false);

            //The winner gets the items
            if (GetComponent<FightBehavior>().Opponent.GetComponent<AgentComponent>().Damage > _agentComponent.Damage) {
                YieldObjects(GetComponent<FightBehavior>().Opponent);
            }
            else
                GetComponent<FightBehavior>().Opponent.GetComponent<ShopperBehavior>().YieldObjects(this.gameObject);

        }
	}

    //Arrange positions of object
    void SortObjects() {
        
        for (int i = 0; i < CurrentObjs.transform.childCount; i++)
            CurrentObjs.transform.GetChild(i).transform.position = CurrentObjs.transform.position - 0.05f * Vector3.up + i * 0.05f * Vector3.up;

        
    }

    //Give all your objects to your opponent
    void YieldObjects(GameObject opponent) {
        for (int i = 0; i < CurrentObjs.transform.childCount; i++) {
            CurrentObjs.transform.GetChild(i).parent = opponent.GetComponent<ShopperBehavior>().CurrentObjs.transform;
        }
        opponent.GetComponent<ShopperBehavior>().SortObjects();      
        
    }


    void PickedObject() {
        _agentComponent.FinishedWaiting = true;
    }

    void UpdateState() {
        float minDist = 100000f;
        _totalObjCnt = 0;
     
        
        switch (State) {
            case (int)ShoppingState.GoingToObject: {
                //Find closest object with the least effective density
                Transform closestObj = null;
                for (int i = 0; i < _objs.transform.childCount; i++) { //find closest object
                    Transform obj = _objs.transform.GetChild(i);
                    if (obj.gameObject.activeInHierarchy == false || obj.GetComponent<ObjComponent>().Achieved == true)
                        continue;
                    _totalObjCnt++;
                    // if the object is too crowded and I am not the closest person wait in this state and I am not hopeful
                    if (_affectComponent.Emotion[(int)EType.Hope] < 0.3f && obj.GetComponent<ObjComponent>().Density > 20 && obj.GetComponent<ObjComponent>().ClosestAgent != this.gameObject) {
                        _agentComponent.LookAt(obj.transform.position, 0.01f); //just look at the object
                        _navmeshAgent.updateRotation = true; 
                        continue;
                    }

                    float dist = Vector3.Distance(obj.transform.position, transform.position);
                    if (dist < minDist) {
                        minDist = dist;
                        closestObj = obj;
                    }
                }
                _desiredObj = closestObj;
                if (_desiredObj != null) {
                    _agentComponent.SteerTo(_desiredObj.transform.position);
                    float dist = Vector2.Distance(new Vector2(_desiredObj.transform.position.x, _desiredObj.transform.position.z), new Vector2(transform.position.x, transform.position.z));
                
                    if (dist < 1f) {//Pick up object                        
                        State = (int)ShoppingState.PickingUpObject;
                        
                    }

                }
                else {
                    if (_totalObjCnt == 0) { //all objects in my shelf are consumed
                        _shelfInd++;
                       // if ( _affectComponent.Ekman[(int)EkmanType.Afraid] > 0.5) {
                        if (_shelfInd >= 5) { //all objects in the store are consumed
                            _allConsumed = true;
                            
                            
                            if (_acquiredObjCnt > 0) {//if I bought something   
                                  if (_affectComponent.Emotion[(int)EType.Reproach] < 0.5f) 
                                     State = (int)ShoppingState.GoingToLine;
                                else
                                      State = (int)ShoppingState.Exiting; //go out without paying                          
                            }
                            else { // I have nothing to buy
                                State = (int)ShoppingState.Exiting;                           
                            }
                        }
                        else {
                            State = (int)ShoppingState.ShelfChanging;                                                   
                        }
                    }

                }

                
            }
            break;
            case (int)ShoppingState.PickingUpObject:

            if (_agentComponent.StartedWaiting == false ) {
                    _agentComponent.LookAt(_desiredObj.transform.position,0.1f);
             //   if(_desiredObj.GetComponent<ObjComponent>().Achieved == false && _desiredObj.GetComponent<ObjComponent>().ClosestAgent.Equals(this.gameObject)) {
                    _animationSelector.SelectAction("PICKUP");
                    _agentComponent.HandPos = _desiredObj.position; //+ Vector3.up * 0.1f;    
               // }
                    
                    
               // StartCoroutine(_agentComponent.WaitAWhile(2));
                    _agentComponent.StartedWaiting = true;
                    _agentComponent.FinishedWaiting = false;
               
            }
                
            else if (_agentComponent.StartedWaiting && !_agentComponent.FinishedWaiting && _desiredObj.GetComponent<ObjComponent>().Achieved == false) { //started waiting
                    if(_desiredObj.GetComponent<ObjComponent>().ClosestAgent.Equals(this.gameObject))
                        _desiredObj.position = _rightHand.position;
                    _agentComponent.HandPos = _desiredObj.position;//+ Vector3.up * 0.1f;
                    PickedObject();
            }
                
            if(_agentComponent.FinishedWaiting) {                            
                    if (_desiredObj.GetComponent<ObjComponent>().Achieved == false) { //make sure someone else didn't pick it before me                
                                                    
                        //   CurrentObjs.Add(_desiredObj);
                        _desiredObj.parent = CurrentObjs.transform;
                        _desiredObj.position = CurrentObjs.transform.position - 0.05f * Vector3.up + (CurrentObjs.transform.childCount - 1) * 0.05f * Vector3.up;
                      //  _desiredObj.position = _leftHand.position;
                        _desiredObj.GetComponent<ObjComponent>().AchievingAgent = this.gameObject;
                        _desiredObj.GetComponent<ObjComponent>().Achieved = true;
                        _acquiredObjCnt++;
                    //    _agentComponent.HandPos = _desiredObj.position + Vector3.up * 0.1f;
                        

                    }
                    else //it is achieved
                        _agentComponent.LookAt(_desiredObj.GetComponent<ObjComponent>().AchievingAgent.transform.position, 0.01f);

                    _navmeshAgent.updateRotation = true;
                    _agentComponent.StartedWaiting = false;
                    if (_acquiredObjCnt >= _desiredObjCnt) {

                    if ( _affectComponent.Emotion[(int)EType.Reproach] < 0.5f)
                            State = (int)ShoppingState.GoingToLine; //go to line
                        else
                            State = (int)ShoppingState.Exiting; //exit without paying

                    }
                    else
                        State = (int)ShoppingState.GoingToObject; //go to another object                        
                        
                    }

                break;
            case (int)ShoppingState.GoingToLine:
                _counter.GetComponent<LineHandler>().GetInLine(this.gameObject); //get in line if not already in
                if(_counter.GetComponent<LineHandler>().IsInLine(this.gameObject))
                    State = (int)ShoppingState.WaitingInLine;
                else
                    _agentComponent.SteerTo(_counter.GetComponent<LineHandler>().LineEnd);
                
                break;
            case (int)ShoppingState.WaitingInLine:
                      
                _agentComponent.SteerTo(_counter.GetComponent<LineHandler>().FindLineEndBeforeAgent(this.gameObject));
                if (_counter.GetComponent<LineHandler>().IsFirst(this.gameObject))
                    State = (int)ShoppingState.Paying;
                        
                break;

            case (int)ShoppingState.Paying:
                _agentComponent.Watch(_cashier);
                //_agentComponent.LookAt(_cashier.transform.position);
                if (_agentComponent.StartedWaiting == false)
                    StartCoroutine(_agentComponent.WaitAWhile(10));
                
                if (_agentComponent.FinishedWaiting) {
                    _navmeshAgent.updateRotation = true; //change after lookat
                    _counter.GetComponent<LineHandler>().GetOutLine(this.gameObject);
                    _agentComponent.StartedWaiting = false;
                    State = (int)ShoppingState.Exiting;    
                }
                break;

            case (int)ShoppingState.Exiting:
                _agentComponent.SteerTo(_exit);
                //if they are outside the store
                  if (Vector3.Distance(transform.position, _exit) < 20) {
                      _appraisal.Restart(); //clear all appraisal components
                      _affectComponent.ContagionMode = false;
                  }
                break;

            case (int)ShoppingState.ShelfChanging:
                _closestShelfPos = _shelfComp.FindClosestShelfPos(transform.position, _shelfOrder[_shelfInd]);
                _agentComponent.SteerTo(_closestShelfPos);
                //_objs = GameObject.Find("Objects" + _shelfOrder[_shelfInd]);

                _objs = _allObjs[_shelfOrder[_shelfInd]];
                    
                    
                if (Vector3.Distance(_closestShelfPos, transform.position) < 2f) //close enough
                    State = (int)ShoppingState.GoingToObject;
                break;
            
        }

    }
    void LateUpdate() {
        //if (_currentObj != null) {
        //for (int i = 0; i < CurrentObjs.Count; i++) {

        CurrentObjs.transform.position = _leftHand.position;
     //   for (int i = 0; i < CurrentObjs.transform.childCount; i++) {
            //CurrentObjs.transform.GetChild(i).position = _hand.position - 0.05f * Vector3.up + i * 0.05f * Vector3.up;
            //CurrentObjs[i].position = _hand.position - 0.05f* Vector3.up + i * 0.05f * Vector3.up;
            //CurrentObjs[i].gameObject.SetActive(true);
            //   _agentComponent.HandPos = _currentObjs[i].transform.position;

     //   }
    }
    
    void InitAppraisalStatus() {

        
        if (_affectComponent.Personality[(int)OCEAN.N] > 0.5f)  //if neurotic feel distress about the crowded scene
            _appraisal.AddGoal("sales", 0.25f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectIrrelevant);
        if (_affectComponent.Personality[(int)OCEAN.N] > 0.85f) { //Fear
            //if *really* neurotic feel fear
            //Hope to get  desired items
            for (int i = 0; i < _desiredObjCnt; i++)
                // Add as many fearful emotions as the number of desired objects which can sum up to 0.1          
                _appraisal.AddGoal("sales", 0.05f/_desiredObjCnt, AppDef.Displeased, AppDef.ConsequenceForSelf,
                                   AppDef.ProspectRelevant, AppDef.Unconfirmed);
        }
        else {
            //Hope to get  desired items
            for (int i = 0; i < _desiredObjCnt; i++)
                // Add as many hopeful emotions as the number of desired objects which can sum up to 0.4          
                _appraisal.AddGoal("sales", 0.4f / _desiredObjCnt, AppDef.Pleased, AppDef.ConsequenceForSelf,
                                   AppDef.ProspectRelevant, AppDef.Unconfirmed);
        }



        //Standard about other shoppers
     //      if (_affectComponent.Personality[(int)OCEAN.N] > 0f)  //if neurotic feel jealousy
       //     _appraisal.AddStandard(0.3f, AppDef.Disapproving, AppDef.FocusingOnOther, transform.parent.gameObject);	//shoppers in general --no specific shopper	            
        //Attitude towards objects in the store
        _appraisal.AddAttitude(null, 0.2f, AppDef.Liking); //General liking
        
    }
    
    
    void UpdateAppraisalStatus() {
        if (State == (int)ShoppingState.PickingUpObject) {
            if (_desiredObj != null  && _desiredObj.GetComponent<ObjComponent>().Achieved && _desiredObj.GetComponent<ObjComponent>().AchievingAgent != this.gameObject) { //someone else achieved it just when i was trying to get it                
                //Change hope to disappointment for 1 object
                float wt = _appraisal.RemoveGoal("sales", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
                if (wt != 0) {
                    _appraisal.AddGoal("sales", wt+0.1f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed); //slightly higher than hope
                    //Resentment towards other shoppers
                    _appraisal.AddGoal("sales", 0.1f, AppDef.Displeased, AppDef.ConsequenceForOther, transform.parent.gameObject, AppDef.DesirableForOther);
                    //High disapproval towards that specific agent who achieved my object before me                   
                    _appraisal.AddStandard(0.5f, AppDef.Disapproving, AppDef.FocusingOnOther, _desiredObj.GetComponent<ObjComponent>().AchievingAgent); //Causes reproach

                }

                //Change fear to fearsconfirmed for 1 object
                wt = _appraisal.RemoveGoal("sales", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
                if (wt != 0)
                    _appraisal.AddGoal("sales", wt/5f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed); 
            }

            else {
                //I achieved                
                //Change hope to satisfaction                  
                float wt = _appraisal.RemoveGoal("sales", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
                if (wt != 0) {
                    _appraisal.AddGoal("sales", wt, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);
                    //If neurotic, gloating towards other shoppers
                    if (_affectComponent.Personality[(int) OCEAN.A] < 0f) 
                        _appraisal.AddGoal("sales", 0.1f, AppDef.Pleased, AppDef.ConsequenceForOther, transform.parent.gameObject, AppDef.UndesirableForOther);
                }

                //Change fear to relief
                wt = _appraisal.RemoveGoal("sales", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
                if (wt != 0)
                    _appraisal.AddGoal("sales", wt, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed); 
            }

        }

        else if (State == (int)ShoppingState.GoingToObject) {
            //if someone else took my desired object
            if(IsDesiredObjectMissed()){            
                bool exists = _appraisal.DoesStandardExist(_desiredObj.GetComponent<ObjComponent>().AchievingAgent, AppDef.Disapproving);
                if (!exists && _affectComponent.Personality[(int)OCEAN.A] < 0f) //if disagreeable add disapproval  towards that specific agent who achieved my object before me  
                    _appraisal.AddStandard(0.4f, AppDef.Disapproving, AppDef.FocusingOnOther,_desiredObj.GetComponent<ObjComponent>().AchievingAgent); //Causes reproach
                
            }
        }

        else if (_acquiredObjCnt >= _desiredObjCnt) {
            //Change hope to satisfaction                  
           float wt =  _appraisal.RemoveGoal("sales", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
            if (wt != 0) {
                _appraisal.AddGoal("sales", 0.6f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);
                //If neurotic, gloating towards other shoppers
                if (_affectComponent.Personality[(int)OCEAN.N] > 0f)
                    _appraisal.AddGoal("sales", 0.3f, AppDef.Pleased, AppDef.ConsequenceForOther, transform.parent.gameObject, AppDef.UndesirableForOther);
            }

            //Change fear to relief

            wt = _appraisal.RemoveGoal("sales", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
            if (wt != 0)
                _appraisal.AddGoal("sales", wt, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed);                    
                

        }
        else {
            
            if (_allConsumed) { // all the objects in the store are consumed

                //Change hope to disappointment
               float wt  = _appraisal.RemoveGoal("sales", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
                if (wt!= 0) {
                    _appraisal.AddGoal("sales", 0.6f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed);
                    //Resentment towards other shoppers
                    _appraisal.AddGoal("sales", 0.3f, AppDef.Displeased, AppDef.ConsequenceForOther, transform.parent.gameObject, AppDef.DesirableForOther);
                }

                //Change fear to fearsconfirmed
                wt = _appraisal.RemoveGoal("sales", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
                if (wt != 0) 
                    _appraisal.AddGoal("sales", wt/5f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);                    
                

                //Add disapproval to the store for not having any more\
                if(!_appraisal.DoesStandardExist(_cashier, AppDef.Disapproving))
                    _appraisal.AddStandard(0.5f, AppDef.Disapproving, AppDef.FocusingOnOther, _cashier); //Causes reproach
            }
            else if (State == (int)ShoppingState.ShelfChanging) {

                //Decrease hope
                float wt = _appraisal.RemoveGoal("sales", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
                if (wt > 0.001f) {
                    _appraisal.AddGoal("sales", wt*3f/4f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed); //less hope                    
                    //Resentment towards other shoppers
                    _appraisal.AddGoal("sales", 0.05f, AppDef.Displeased, AppDef.ConsequenceForOther, transform.parent.gameObject, AppDef.DesirableForOther);
                }

                //Increase fear
                wt = _appraisal.RemoveGoal("sales", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
                if (wt != 0)
                    _appraisal.AddGoal("sales", wt+0.01f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);       
                
            }

        }
    }


    //Check if someone else took my desired object before me
    bool IsDesiredObjectMissed() {
        return (_desiredObj != null && _desiredObj.GetComponent<ObjComponent>().Achieved &&
                _desiredObj.GetComponent<ObjComponent>().AchievingAgent != null &&
                _desiredObj.GetComponent<ObjComponent>().AchievingAgent != this.gameObject);

    }



    void OnDrawGizmosSelected() {
     /*   Gizmos.color = Color.blue;
        if(_navmeshAgent)
            Gizmos.DrawLine(transform.position, _navmeshAgent.destination);

        
        //Draw shelves
        if (_shelves!=null)
            for(int i = 0; i < 6; i ++) {
                Gizmos.DrawLine(_shelves[i].v1, _shelves[i].v2);
                Gizmos.DrawLine(_shelves[i].v2, _shelves[i].v3);
                Gizmos.DrawLine(_shelves[i].v3, _shelves[i].v4);
                Gizmos.DrawLine(_shelves[i].v4, _shelves[i].v1);
            }*/
    }
}
