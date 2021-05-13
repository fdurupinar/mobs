using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///Navmeshagent stops during fight, so we can use the whole body fight animations
/// </summary>
 
public class FightBehavior : MonoBehaviour {
	public GameObject Opponent;

    Appraisal _appraisal;
	AgentComponent _agentComponent;
	AgentComponent _oAgentComponent;
	public float BeginTime;
	public float EndTime;
    public List<GameObject> Watchers = new List<GameObject>();
  //  public NavMeshObstacle FightObstacle;
    public bool IsOver { get; set; }
    private AnimationSelector _animationSelector;


    public void Init(GameObject o) {
     
        _appraisal = GetComponent<Appraisal>();
        _agentComponent = GetComponent<AgentComponent>();
        BeginTime = Time.time;
        InitAppraisalStatus();
	
		Opponent = o;
		_oAgentComponent = Opponent.GetComponent<AgentComponent>();
        _animationSelector = GetComponent<AnimationSelector>();


        _animationSelector.SelectAction("");
       // _agentComponent.CurrAction[2] = _agentComponent.CurrAction[3] = "";
       // _oAgentComponent.CurrAction[2] = _oAgentComponent.CurrAction[3] = "";

        
        GetComponent<UnityEngine.AI.NavMeshAgent>().updateRotation = false; //rotation is updated according to opponent's direction

        //FightObstacle = new NavMeshObstacle();
    }
	
    UnityEngine.AI.NavMeshAgent _navMeshAgent;

    public FightBehavior() {
        IsOver = false;
    }

    void Start(){
        _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

    }


	void Update () {	
		
		if(!_oAgentComponent.IsFighting() || _agentComponent.IsWounded () || _oAgentComponent.IsWounded() || _agentComponent.IsFallen || _oAgentComponent.IsFallen) {            
			UpdateAppraisalStatus();
            EndTime = Time.time;
            FinishFight();		    
            if (_agentComponent.IsDead() || _agentComponent.IsFallen)
                _animationSelector.SelectAction("WRITHING");
                //_agentComponent.CurrAction[0] = "writhingInPain";
                //_agentComponent.CurrAction[2] = _agentComponent.CurrAction[3] = "";

		}
		else {

		    _agentComponent.AddDamage(_oAgentComponent.IsPolice() ? 0.4f : 0.05f);

		    if (Vector3.Distance(transform.position, Opponent.transform.position) >1) {
              //  _navMeshAgent.Resume();

                 
                _navMeshAgent.updatePosition = true;
                _navMeshAgent.updateRotation = true;


                _agentComponent.SteerTo(Opponent.transform.position);
              //  _agentComponent.CurrAction[0] = "BTMoveForward";

                

            }
            else {
                

                //fighting
               // _navMeshAgent.Stop();
                _navMeshAgent.updatePosition = false;
                _navMeshAgent.updateRotation = false;


		        //FightObstacle.transform.position = (_agentComponent.transform.position + _oAgentComponent.transform.position)/2f;
		        //FightObstacle.radius = (_agentComponent.transform.position - _oAgentComponent.transform.position).magnitude/2f;

		    }

		   // transform.rotation = Quaternion.LookRotation(Opponent.transform.position - transform.position, Vector3.up);							
            _agentComponent.LookAt(Opponent.transform.position, 0.01f);

            /*
             * NO WATCHERS 
            //Witness a fight and be scared if neurotic
            foreach(GameObject c in _agentComponent.CollidingAgents) {
                if (!Watchers.Contains(c) && (c.transform.position - transform.position).magnitude < 5f && c.GetComponent<AgentComponent>().IsVisible(this.gameObject, Mathf.PI / 2f) && !c.GetComponent<AgentComponent>().IsFighting() ) {
                    Watchers.Add(c);
                }                    
            }

            */
            /* NOONE WATCHING IN A SHOP
             //Send message to others to watch this fight        
            foreach(GameObject c in _agentComponent.CollidingAgents) {
                if ( c.GetComponent<AgentComponent>().IsVisible(this.gameObject, Mathf.PI / 2f)
                    && c.GetComponent<AgentComponent>().IsFighting() == false && c.GetComponent<AgentComponent>().IsWatchingFight == false) {            
                    if(c.GetComponent<AffectComponent>().Personality[(int)OCEAN.E] > 0){  //intervene if assertive
                        c.GetComponent<AgentComponent>().SteerTo((transform.position + Opponent.transform.position) / 2f);                                    
                        Watchers.Add(c);
                        c.GetComponent<AgentComponent>().IsWatchingFight = true;
                    }
                    else if(c.GetComponent<AffectComponent>().Personality[(int)OCEAN.O] > 0) { //watch if curious
                        c.GetComponent<NavMeshAgent>().Stop();
                        c.GetComponent<AgentComponent>().Watch((transform.position + Opponent.transform.position) / 2f);                     
                        Watchers.Add(c);
                        c.GetComponent<AgentComponent>().IsWatchingFight = true;
                    }
                }
            }
             */
		}
		
       
	}	
	
	void InitAppraisalStatus() {
		
		//General distress
		_appraisal.AddGoal("fight", 0.2f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectIrrelevant);
				
		//Fear of losing
		_appraisal.AddGoal("fight",  0.1f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);		
		
		//Hope to win
		_appraisal.AddGoal("fight",  0.1f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
		
		//about opponent
		_appraisal.AddStandard(0.8f, AppDef.Disapproving, AppDef.FocusingOnOther, Opponent);		
        
	}
	
	
	void UpdateAppraisalStatus() {
		if(_agentComponent.IsWounded()) {
			//Resentment to opponent
			_appraisal.AddGoal("fight", 0.5f, AppDef.Displeased, AppDef.ConsequenceForOther, Opponent, AppDef.DesirableForOther);
					
			//Change hope to disappointment			
            _appraisal.RemoveGoal("fight", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);            
			_appraisal.AddGoal("fight", 0.5f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed);
					
			//Change fear to fearsConfirmed
			_appraisal.RemoveGoal("fight", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
			_appraisal.AddGoal("fight",0.5f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);
			
					
		}
		if(_oAgentComponent.IsWounded ()) { //If opponent is also wounded
			//Gloating to opponent
			_appraisal.AddGoal("fight", 0.2f,  AppDef.Pleased, AppDef.ConsequenceForOther,Opponent, AppDef.UndesirableForOther);
						
			//Change hope to satisfaction
			_appraisal.RemoveGoal("fight", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
			_appraisal.AddGoal("fight", 0.5f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);
		
            //Change fear to relief
			_appraisal.RemoveGoal("fight", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
			_appraisal.AddGoal("fight",0.2f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed);
			
		}			
	}

    //Update other person's goals about me if they have standards about me
    public void UpdateAppraisalStatusOfOther(GameObject other) {
        if (other.GetComponent<Appraisal>().DoesGoalExist("fight", this.gameObject) || other.GetComponent<Appraisal>().DoesGoalExist("fight", Opponent)) //already updated
            return;
        foreach(Standard s in other.GetComponent<Appraisal>().Standards) {
            if(s.subject!= null && (s.subject.Equals(this.gameObject) || s.subject.Equals(transform.parent.gameObject))) {
                
                if (s.approving)  {                    
                    if (_agentComponent.IsWounded()) {
                        //Pity for this agent
                            //if goal does not already exist                        
                        other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Displeased, AppDef.ConsequenceForOther, this.gameObject, AppDef.UndesirableForOther);
                        //Resentment to the opponent
                        other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Displeased, AppDef.ConsequenceForOther, Opponent, AppDef.DesirableForOther);
                    }
                    if (_oAgentComponent.IsWounded()) {
                        //Happy-for  this agent
                        other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Pleased, AppDef.ConsequenceForOther, this.gameObject, AppDef.DesirableForOther);
                        //Gloating to the opponent
                        other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Pleased, AppDef.ConsequenceForOther, Opponent, AppDef.UndesirableForOther);
                    }
                }
                else {//disapproving
                    if (_agentComponent.IsWounded()) {
                        //Gloating for this agent
                        other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Pleased, AppDef.ConsequenceForOther, this.gameObject, AppDef.UndesirableForOther);
                        //Happy-for  the opponent
                        other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Pleased, AppDef.ConsequenceForOther, Opponent, AppDef.DesirableForOther);
                    }
                    if (_oAgentComponent.IsWounded()) {
                        //Resentment for this agent
                        other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Displeased, AppDef.ConsequenceForOther, this.gameObject, AppDef.DesirableForOther);
                        //Pity for the opponent
                        other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Displeased, AppDef.ConsequenceForOther, Opponent, AppDef.UndesirableForOther);
                    }
                }
            }
        }
            
   }
   	
	public void FinishFight() {

        if (!_agentComponent.IsFallen) {
            _navMeshAgent.Resume();
            GetComponent<UnityEngine.AI.NavMeshAgent>().updatePosition = true; //rotation is updated by navmeshagent
            GetComponent<UnityEngine.AI.NavMeshAgent>().updateRotation = true; //rotation is updated by navmeshagent
        }

	   // if (_agentComponent.IsWounded())
         //   GetComponent<AgentComponent>().IncreaseSpeed(10);
        
		//Remove related goals and standards
	//	_appraisal.RemoveGoal ("fight");
		//_appraisal.RemoveStandard (Opponent);

       



        foreach (GameObject c in Watchers) {
            UpdateAppraisalStatusOfOther(c);
            c.GetComponent<AgentComponent>().IsWatchingFight = false;
            //GetComponent<NavMeshAgent>().updatePosition = true; //rotation is updated by navmeshagent
            //GetComponent<NavMeshAgent>().updateRotation = true; //rotation is updated by navmeshagent
            c.GetComponent<UnityEngine.AI.NavMeshAgent>().Resume();
        }

        _agentComponent.TimeLastFight = Time.time;
        DestroyImmediate(this);

        
        //IsOver = true;

        //S
        
	}
	

		
}
