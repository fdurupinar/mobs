using System.Linq;
using UnityEngine;
using System.Collections;


public class PoliceBehavior : MonoBehaviour {
    
    public GameObject Shield;
    public GameObject NightStick;    
    public Bounds Zone;   //protection zone assigned in GroupBuilder

    private Vector3 _post;
    public GameObject Intruder = null;    
    private AgentComponent _agentComponent;
    private GameObject _protestCenter;
    private bool _isBeating = false;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private Transform _sT, _nT;
    private AffectComponent _affectComponent;
    Appraisal _appraisal;
	void Start () {	
		Shield = (GameObject)Instantiate(UnityEngine.Resources.Load("shield"));
	    Shield.transform.parent = this.transform;
		NightStick = (GameObject)Instantiate(UnityEngine.Resources.Load("nightStick"));
        NightStick.transform.parent = this.transform;

		_agentComponent = GetComponent<AgentComponent>();
        _affectComponent = GetComponent<AffectComponent>();
        _appraisal = GetComponent<Appraisal>();
        _post = transform.position;
        _protestCenter = GameObject.Find("ProtestCenter");
        if(!_protestCenter)
            _protestCenter = new GameObject();
        _protestCenter.transform.position = Vector3.zero;

        //_sT = transform.Find("Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm/LeftForeArm/LeftHand");
        _nT = transform.Find("Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/RightHandRing1");
        _sT = transform.Find("Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/RightHandRing1");
	    _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

	    _navMeshAgent.radius = 0.4f;
		
        GetComponent<UnityEngine.AI.NavMeshAgent>().updateRotation = false;
        InitAppraisalStatus();
	}
    public void Enable() {
        //Transform sT = transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/Neck/LeftShoulder/LeftArm/LeftForeArm/LeftHand");
        Shield.transform.position = _sT.position;
        Shield.transform.rotation = _sT.transform.rotation;
        Shield.transform.Rotate(-90, 0, 0, Space.Self);
        //Shield.transform.Rotate(0, 180, 0, Space.Self);
        Shield.transform.Translate(0.06f, 0, 0.07f);
        Shield.SetActive(true);

        //Transform nT = transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/Neck/RightShoulder/RightArm/RightForeArm/RightHand/RightHandRing1");
        NightStick.transform.position = _nT.position;
        NightStick.transform.rotation = _nT.transform.rotation;
        NightStick.SetActive(true);
    }
	public void Restart() {
        Intruder = null;
    }
	// Update is called once per frame
	void Update () {
        if (_agentComponent.IsDead() || _agentComponent.IsFallen) {
        //    _agentComponent.CurrAction[0] = "writhingInPain";
        //    _agentComponent.CurrAction[2] = _agentComponent.CurrAction[3] = "";
            Shield.SetActive(false);          
            return;
        }

        
        UpdateAction();

        if (!_agentComponent.IsFighting()) {

            if (Intruder == null && (_post - transform.position).magnitude > 10f) //if no one to watch && stop jittering
                _agentComponent.SteerTo(_post);
            else {
                _navMeshAgent.Stop();
            }
            
			Vigil();            
         }
	}
	void LateUpdate() {
        //Transform sT = transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/Neck/LeftShoulder/LeftArm/LeftForeArm/LeftHand");
        Shield.transform.position = _sT.position;
        Shield.transform.rotation = _sT.transform.rotation;
        Shield.transform.Rotate(0, 0, -90, Space.Self);
        Shield.transform.Rotate(90, 0, 0, Space.Self);
     //   Shield.transform.Rotate(0, 180, 0, Space.Self);
        Shield.transform.Translate(0.06f, 0, 0.08f);

        //Transform nT = transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/Neck/RightShoulder/RightArm/RightForeArm/RightHand/RightHandRing1");
        NightStick.transform.position = _nT.position;
        NightStick.transform.rotation = _nT.transform.rotation;        
 
    }
	void UpdateAction() {
        if (_agentComponent.IsFighting()) {
			//_agentComponent.CurrAction[0] = "fight0";
            Shield.SetActive(false);            
           
		}
		else if(_isBeating) {
            NightStick.SetActive(true);
		    Shield.SetActive(false);
       //     _agentComponent.CurrAction[3] = "beating";
        }
        else {
            Shield.SetActive(true);
            NightStick.SetActive(false);
		//	_agentComponent.CurrAction[2] = "holdShield";	
		}			
	}
	
  
    //Check all the police in my group to see whether a specific protester is being beaten, faced, fought etc.
    bool IsProtesterBeingAttended(GameObject protester) {
        //Check all the police around who attend this protester

        PoliceBehavior[] pbs = transform.parent.GetComponentsInChildren<PoliceBehavior>();
        return pbs.Any(p => !p.gameObject.Equals(gameObject) && p.Intruder == protester);
    }

    /// <summary>
    ///Watch the closest protester in the collider zone 
    ///Prevent him from moving beyond your post
    /// </summary>
	void Vigil() {		
		float minDist = 100000f;
		Vector3 dist = Vector3.zero;
        
        _navMeshAgent.updateRotation = false;
        _agentComponent.ResetSpeed();


        

        _isBeating = false;
        if(_agentComponent.CollidingAgents.Count != 0 && _isBeating == false) 
        {
            foreach (GameObject c in _agentComponent.CollidingAgents) {
                if (c.GetComponent<ProtesterBehavior>() != null && !c.GetComponent<ProtesterBehavior>().IsFleeing() &&
                    !IsProtesterBeingAttended(c.gameObject) && c.GetComponent<AgentComponent>().IsFighting() == false) {
                    //Check if c is visible
                   /* SphereCollider col = (SphereCollider) c.collider;
                    col.radius = 1.2f;

                    RaycastHit hit;

                    if (Physics.Raycast(transform.position, (c.transform.position - transform.position), out hit)) {
                        if (hit.transform == c.transform) {
                            //visible agent
                    */
                    dist = c.transform.position - transform.position;
                    if (dist.magnitude < minDist) {
                        minDist = dist.magnitude;
                        Intruder = c;
                
                    }
                   
                }
                //}
                // col.radius = 4f;                
            }
            
            if(Intruder != null && minDist < 4f ) {
                _agentComponent.LookAt(Intruder.transform.position, 0.01f);

            }

            if (Intruder != null && Intruder.transform.position.z < Zone.center.z && minDist < 1f) { //intruder has not crossed the protection barrier yet
                Vector3 force = new Vector3(0, 0, -1); //should always push out of the protest target
                Intruder.GetComponent<AgentComponent>().AddImpact(force * 1f);/// (minDist + 0.1f));//* 2f);                
                _isBeating = true;
                Intruder.GetComponent<ProtesterBehavior>().GetBeaten(this.gameObject);                        
            }

			else if(Intruder != null &&  Intruder.transform.position.z > Zone.center.z) { //intruder crossed the barrier, follow her
           //     if(!_appraisal.DoesStandardExist(Intruder, AppDef.Disapproving))
             //       _appraisal.AddStandard(0.2f, AppDef.Disapproving, AppDef.FocusingOnOther, Intruder);
                
                _navMeshAgent.speed = Intruder.GetComponent<UnityEngine.AI.NavMeshAgent>().speed * 2f;
                _navMeshAgent.acceleration = 2f;
                _agentComponent.Face(Intruder);

                if (minDist < 1f) {
                    _isBeating = true;
                    Intruder.GetComponent<ProtesterBehavior>().GetBeaten(this.gameObject);
                    //if (_agentComponent.IsFacing(Intruder)) {
                    //PUSH NO MORE
          /*          if(transform.position.z  > Intruder.transform.position.z){  
                        Vector3 force = new Vector3(0, 0, -1); //should always push out of the protest target
                        Intruder.GetComponent<AgentComponent>().AddImpact(force * 1f);///(minDist + 0.1f)); //* 2f);
                    }
            */        //    if(minDist < 1f) { //Intruder.GetComponent<NavMeshAgent>().radius + _navMeshAgent.radius + 0.3f) { //both beat and push
                    //Vector3 force = _navMeshAgent.velocity - Intruder.GetComponent<NavMeshAgent>().velocity; //we want intruder to move in the opposite direction //Intruder.GetComponent<NavMeshAgent>().velocity + _navMeshAgent.velocity;



                   
                    if (_affectComponent.GetCurrMoodOctant() == (int) MType.Hostile &&
                        _affectComponent.GetExpressionRange() == EmotionRange.High && Random.Range(0, 100) < 1)
                        //too overwhelmed                  
                        Instantiate(Resources.Load("Explosion"), Intruder.transform.position,
                                    new Quaternion());
                     
                }



			}    
            else {
                Intruder = null; //all protesters are far away to be a danger
                _agentComponent.Watch(_protestCenter);                
            }
		}
       
        _navMeshAgent.updateRotation = true;
	}

    void InitAppraisalStatus() {
        //General distress
        _appraisal.AddGoal("protest", 0.2f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectIrrelevant);
        //Standard about protesters
        ProtesterBehavior[] protesterComponents = FindObjectsOfType(typeof(ProtesterBehavior)) as ProtesterBehavior[];
        if (protesterComponents != null) { //police in general --no specific police	            
            //Reproach against protesters if neurotic
            if (_affectComponent.Personality[(int)OCEAN.N] > 0f)
                _appraisal.AddStandard(0.2f, AppDef.Disapproving, AppDef.FocusingOnOther, protesterComponents[0].transform.parent.gameObject);
            
        }


    

    }
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        if(Intruder != null){
            Gizmos.DrawLine(transform.position, Intruder.transform.position);
            Debug.DrawRay(Intruder.transform.position, Intruder.GetComponent<UnityEngine.AI.NavMeshAgent>().velocity *3f, Color.white);
        }
    }
}
