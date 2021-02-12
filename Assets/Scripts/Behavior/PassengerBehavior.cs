using UnityEngine;
using System.Collections;

public class PassengerBehavior : MonoBehaviour {
    UnityEngine.AI.NavMeshAgent _navMeshAgent;
    GameObject _targetSeat, _targetExitPoint, _seats, _train;
    Vector3 _targetPos;
    bool _isInsideTrain = false;
    bool _outraged = false;
    Appraisal _appraisal;
    Aim _aim;
    AgentComponent _agentComponent;
    AffectComponent _affectComponent;

    float _waitDuration, _waitCounter;

    public enum Aim {
        GetOff,
        GetOn
    }

	void Start()  {

        _navMeshAgent 	= GetComponent<UnityEngine.AI.NavMeshAgent>();
        _agentComponent 	= GetComponent<AgentComponent>();
        _affectComponent = GetComponent<AffectComponent>();
        _appraisal = GetComponent<Appraisal>();

        _seats = GameObject.Find("Seats");
		_train = GameObject.Find("Train");


        if (IsInsideTrain())
            _aim = Aim.GetOff;
        else
            _aim = Aim.GetOn;

        //_navMeshAgent.radius *= 0.25f; //because otherwise agents get stuck

        //FUNDA: Both Should be in the Personality mapper
        _agentComponent.WalkingSpeed = _navMeshAgent.speed = 2.5f  + _affectComponent.Personality[(int)OCEAN.E];  
       _waitDuration = -_affectComponent.Personality[(int)OCEAN.E] * 2.5f + 2.5f;

        if (_aim == Aim.GetOn)
		{
            Invoke("SetTarget", _waitDuration);
			InvokeRepeating("HandlePanic", _waitDuration + 2, _waitDuration * 0.25f);
            transform.rotation = Quaternion.Euler(0, Random.Range(-20, 20), 0);
		}
        else
            Invoke("SetTarget", 1);

        InitAppraisalStatus();

	}

    public void Restart() {
        InitAppraisalStatus();

        _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _navMeshAgent.radius *= 0.25f;

       // _agentComponent.CurrAction[0] = "BTMoveForward";
        if (_aim == Aim.GetOn) {
            Invoke("SetTarget", _waitDuration);
            InvokeRepeating("HandlePanic", _waitDuration + 2, _waitDuration * 0.25f);
            transform.rotation = Quaternion.Euler(0, Random.Range(-20, 20), 0);
        }
        else
            Invoke("SetTarget", 1);


          for (int i = 0; i < _seats.transform.childCount; i++) 
                _seats.transform.GetChild(i).GetComponent<SeatComponent>().IsPicked =false;


    }
    void InitAppraisalStatus() {
        //Distress
        //appraisal.AddGoal("protest", 0.2f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectIrrelevant);
        //Standard about other passengers
        if (_affectComponent.Personality[(int)OCEAN.C] < 0f) {
            PassengerBehavior[] passengerComponents = FindObjectsOfType(typeof(PassengerBehavior))  as PassengerBehavior[];
            _appraisal.AddStandard(0.5f, AppDef.Disapproving, AppDef.FocusingOnOther, passengerComponents[0].transform.parent.gameObject);	//passengers in general --no specific passenger	            
        }

        //add general distress 
        _appraisal.AddGoal("commute", 0.2f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectIrrelevant);				
    }
    void UpdateAction() {
    }
	void HandlePanic() {
		if(!IsInsideTrain())
		{
			_agentComponent.IncreasePanic();
		}
	}

    void SetTarget() {
        if (_aim == Aim.GetOn)
        {
            AssignClosestEmptySeat();
            //FUNDA _navMeshAgent.SetDestination(targetSeat.transform.position);
            _agentComponent.SteerTo(_targetSeat.transform.position);
        }
        else
        {
            AssignRandomExitPoint();
            //FUNDA _navMeshAgent.SetDestination(targetExitPoint.transform.position);
            _agentComponent.SteerTo(_targetExitPoint.transform.position);
            _targetPos = _targetExitPoint.transform.position;
        }
    }

	void FixedUpdate ()  {
        if (_aim == Aim.GetOn)
        {
            foreach (GameObject g in _agentComponent.CollidingAgents)
            {
                if (g.GetComponent<PassengerBehavior>()._aim == Aim.GetOff) {
              // FUNDA SIMDILIK     if (!appraisal.DoesStandardExist(g,AppDef.Disapproving))
               //         appraisal.AddStandard(0.5f, AppDef.Disapproving, AppDef.FocusingOnOther, g);	
                    
                    //g.GetComponent<AffectComponent>().Emotion[(int)EType.Anger] += 0.000001f * (10 - waitDuration) * 0.1f;
                    g.GetComponent<AffectComponent>().Emotion[(int)EType.Anger] += 0.001f * (10 - _waitDuration) * 0.1f;
                }
            }
        }
        else
        {
            if (GetComponent<AffectComponent>().Emotion[(int)EType.Anger] > 0.35)
            {
                if (!_outraged)
                {
                    _outraged= true;
                    Invoke("Shout", 3);
                }
            }
        }
	}

    void Shout() {
        //_navMeshAgent.destination = train.transform.position;
        _agentComponent.SteerTo(_train.transform.position);
        Invoke("Stop", 0.5f);
        //transform.LookAt(train.transform);

        
        int val = Random.Range(0, 2);

        if (val == 0) 
            //_agentComponent.CurrAction[3] = "yelling0";

        if (val == 0)
            //_agentComponent.CurrAction[3] = "yelling1";


        Invoke("Leave", 3);
    }

    void Stop() {
        _navMeshAgent.Stop();
    }

    void Leave() {
        _navMeshAgent.destination = _targetExitPoint.transform.position;
        _navMeshAgent.Resume();

        //_agentComponent.CurrAction[0] = "BTMoveForward";
    }

    bool IsInsideTrain() {
        return _train.GetComponent<Collider>().bounds.Contains(transform.position + Vector3.up);
    }

    void AssignRandomSeat()  {
        int index = Random.Range(0, _seats.transform.childCount);

        _targetSeat = _seats.transform.GetChild(index).gameObject;
    }

    void AssignClosestEmptySeat() {
        int minIndex = -1;

        float minDist = 1000000f;

        for (int i = 0; i < _seats.transform.childCount; i++) {
            float dist = (_seats.transform.GetChild(i).position - transform.position).magnitude;
            if (dist < minDist && _seats.transform.GetChild(i).GetComponent<SeatComponent>().IsPicked == false) {
                minIndex = i;
                minDist = dist;
            }
        }

        if (minIndex > -1) {
            _targetSeat = _seats.transform.GetChild(minIndex).gameObject;
            _targetSeat.GetComponent<SeatComponent>().IsPicked = true;
        }
        else {
            Debug.Log("Not enoguh seats");
        }
    }

    void AssignRandomExitPoint() {
        GameObject exitPoints = GameObject.Find("ExitPoints");

        int index = Random.Range(0, exitPoints.transform.childCount);

        _targetExitPoint = exitPoints.transform.GetChild(index).gameObject;
    }

    void AssignClosestExitPoint() {

    }

    void OnTriggerStay(Collider collider) {
        if(collider.gameObject.name.Equals("Train"))
            _isInsideTrain = true;
    }
}