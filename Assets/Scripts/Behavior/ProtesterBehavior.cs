using UnityEngine;
using System.Collections;

public class ProtesterBehavior : MonoBehaviour {	
	Appraisal _appraisal;
	AgentComponent _agentComponent;
    AffectComponent _affectComponent;
	
	public bool BannerCarrier = false;
	public GameObject Banner;
    UnityEngine.AI.NavMeshAgent _navMeshAgent;
    Transform _bT;
    public GameObject Shelter;
    bool _isFleeing = false;

    private AnimationSelector _animationSelector;

    public GameObject Leader;
    public GameObject _protestTarget;

    // Use this for initialization
	void Start () 
	{
        _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
     //FUNDA    _navMeshAgent.radius *= 0.75f;
        _bT = transform.Find("Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand");

        if(!_bT)
            _bT = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand");
        if (!_bT)
            _bT = transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/RightShoulder/RightArm/RightForeArm/RightHand");
        if (!_bT)
            _bT = transform.Find("hip/abdomen/chest/rCollar/rShldr/rForeArm/rHand");

		_appraisal = GetComponent<Appraisal>();
		_agentComponent = GetComponent<AgentComponent>();
        _affectComponent = GetComponent<AffectComponent>();
        _animationSelector = GetComponent<AnimationSelector>();
		InitAppraisalStatus();

        Leader = null;
		Leader = this.transform.parent.GetComponent<LeaderComponent>().leader;
        	
        
		if(GetComponent<Animator>().enabled && _agentComponent.Id % 4 == 0) {
			BannerCarrier = true;			
			Banner = (GameObject)Instantiate(UnityEngine.Resources.Load("banner"));
            Banner.transform.parent = this.transform; //make it the protester's child
		    _animationSelector.SelectAction("HOLDBANNER");
		    //_agentComponent.CurrAction[3] = "";
		    //_agentComponent.CurrAction[2] = "holdBanner";	
		}
		else {
            _animationSelector.SelectAction("CLAPPING");
            //_agentComponent.CurrAction[3] = "clapping";
            //_agentComponent.CurrAction[2] = "";	
		}
	

	    _protestTarget = GameObject.Find("ProtestTarget");

        if (!_protestTarget) {
            _protestTarget = new GameObject();
            _protestTarget.transform.position = new Vector3(10, 0, 0);
        }


        GameObject[] shelters = GameObject.FindGameObjectsWithTag("Shelter");
	    Shelter = shelters[Random.Range(0, shelters.Length)];

        
         UpdateDestination();
        
	}

    public bool IsFleeing() {
        return _isFleeing;
    }

    public void Enable() {
        if (!BannerCarrier) return;
        Banner.transform.position = _bT.position;
        Banner.transform.rotation = _bT.rotation;
        Banner.transform.Rotate(90, 0, 0);
    }

    public void Restart() {
        InitAppraisalStatus();
        UpdateDestination();
        
    }


    void Update () {

        if (!_agentComponent.IsFighting()) {
            UpdateDestination();
             if (BannerCarrier) 
                Banner.SetActive(true);
        }
        else {
            if (BannerCarrier)
                Banner.SetActive(false);
        }

        if(_agentComponent.IsDead() || _agentComponent.IsFallen) {
            //_agentComponent.CurrAction[0] = "writhingInPain";
            _animationSelector.SelectAction("WRITHING");
            if(BannerCarrier) 
                 Banner.SetActive(false);			
            return;
        }

        
		/*
        if (_agentComponent.IsFighting() == false) {

           // if (affectComponent.Emotion[(int)EType.Anger] > 0.6f)
             //   Instantiate(UnityEngine.Resources.Load("Explosion"), transform.position, new Quaternion());

        }
        */
        
    }
    void LateUpdate(){
        if (BannerCarrier) {
            Banner.transform.position = _bT.position;
            Banner.transform.rotation = _bT.rotation;
            Banner.transform.Rotate(90, 0, 0);
            Banner.transform.Rotate(0, 0, 0);
        }
    }
		
	void UpdateDestination() {

        //Run and hide into a shelter
        if (_affectComponent.GetCurrMoodOctant() == (int)MType.Anxious && _affectComponent.GetExpressionRange() == EmotionRange.High) {
            _agentComponent.SteerTo(Shelter.transform.position);
            _isFleeing = true;
        }
        else {
            _isFleeing = false;
            if (Leader == null || Leader == this.gameObject) //if this agent is the leader himself
                _agentComponent.SteerTo(_protestTarget.transform.position);

            else if (Leader != null)
                _agentComponent.SteerTo(Leader.transform.position);

        }

    }
       
    
	void InitAppraisalStatus() {	
		//Distress
		_appraisal.AddGoal("protest", 0.2f, AppDef.Displeased, AppDef.ConsequenceForSelf,AppDef.ProspectIrrelevant);				
		//Standard about police
		
        PoliceBehavior[] policeComponents = FindObjectsOfType(typeof(PoliceBehavior)) as PoliceBehavior[];
        if (policeComponents != null) { //police in general --no specific police	            
            //Reproach against police if unconscientious
            if (_affectComponent.Personality[(int)OCEAN.A] < 0f) 
                _appraisal.AddStandard(0.2f, AppDef.Disapproving, AppDef.FocusingOnOther, policeComponents[0].transform.parent.gameObject);
            //Fear to be beaten if neurotic
            if (_affectComponent.Personality[(int)OCEAN.N] > 0.5f) 
                _appraisal.AddGoal("beating", 0.3f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
            else if (_affectComponent.Personality[(int)OCEAN.N] > 0f)
                _appraisal.AddGoal("beating", 0.2f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
                    
            }
		

        //About other protesters in the same group as me
        _appraisal.AddStandard(0.15f, AppDef.Approving, AppDef.FocusingOnOther, transform.parent.gameObject);		

        //About myself
        _appraisal.AddStandard(0.15f, AppDef.Approving, AppDef.FocusingOnSelf);		

	}

    public void GetBeaten(GameObject other) {
        if(!_appraisal.DoesStandardExist(other,AppDef.Disapproving)) {
            _appraisal.AddStandard(0.4f, AppDef.Disapproving, AppDef.FocusingOnOther, other);
            _appraisal.AddGoal("protest", 0.3f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectIrrelevant);			            
        }


        //Change fear to fearsconfirmed
         float wt = _appraisal.RemoveGoal("beating", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
        if (wt != 0)
            _appraisal.AddGoal("beating", wt, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);          

        //Add fear about being beaten
        if (_affectComponent.Personality[(int)OCEAN.N] > 0.5f) 
            _appraisal.AddGoal("beating", 0.3f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
        else if (_affectComponent.Personality[(int)OCEAN.N] > 0f)
            _appraisal.AddGoal("beating", 0.2f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);

        _agentComponent.AddDamage(0.01f);

    }
	
	
}
