using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Random = System.Random;

public class ExplosionBehavior : MonoBehaviour {
    
    private Appraisal _appraisal;
    private AgentComponent _agentComponent;
    const float EscapeDist = 10f;
    private bool _isOver = false; //if explosion is over
    private AnimationSelector _animationSelector;
	void Start () {
       
        _appraisal = GetComponent<Appraisal>();
        _agentComponent = GetComponent<AgentComponent>();
        InitAppraisalStatus();

        _animationSelector = GetComponent<AnimationSelector>();
        GameObject[] shelters = GameObject.FindGameObjectsWithTag("Shelter");
    
	}
		
	void Update () {
        const float fearThreshold = 0.1f;
        GameObject[] explosions = GameObject.FindGameObjectsWithTag("Explosion");
        
	
        if ( _agentComponent.IsDead()) {            
                Destroy(this);
                _animationSelector.SelectAction("WRITHING");
            //_agentComponent.CurrAction[0] = "writhingInPain";
           // _agentComponent.CurrAction[2] = _agentComponent.CurrAction[3] = "";
        }

        else if (explosions.Length > 0) {
            //Find the closest explosion and run from it 
            float minDist = 100000;
            GameObject closestExplosion = null;
            foreach (GameObject x in explosions) {
                if (x != null) {
                    float dist = (x.transform.position - this.transform.position).magnitude;
                    if (dist < minDist) {
                        minDist = dist;
                        closestExplosion = x;
                    }
                }
            }
            if (minDist < 5f) //add even more damage
                _agentComponent.AddDamage(0.15f);
            else if (minDist < 10f) //add more damage
                _agentComponent.AddDamage(0.1f);
            else if (minDist < 15f)
                _agentComponent.AddDamage(0.05f);
            
            //Wait until fear is above some threshold before reacting
            if(GetComponent<AffectComponent>().Emotion[(int)EType.Fear] > fearThreshold) {
                _agentComponent.DeactivateOtherBehaviors();                
                //_agentComponent.CurrAction[0] = "BTMoveForward";
               // _agentComponent.CurrAction[2] = "";
                //_agentComponent.CurrAction[3] = "";
                if (GetComponent<AffectComponent>().Emotion[(int)EType.Fear] > 0.5f || GetComponent<AffectComponent>().Personality[(int)OCEAN.N] > 0)                
                    _agentComponent.IncreasePanic();
                if (closestExplosion)
                    _agentComponent.SteerFrom(closestExplosion.transform.position);

                else {
                    
                    _agentComponent.SteerTo(ClosestShelter().transform.position);
                }
                
             }
        }
        //Explosion is over
        else {
            _agentComponent.SteerTo(ClosestShelter().transform.position);

            if (_isOver == false) {
                _isOver = true;                
                if (_agentComponent.IsWounded())  {
                    //change fear to fearsconfirmed   
                    _appraisal.RemoveGoal("explosion", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant);
                    _appraisal.AddGoal("explosion", 1.0f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);
                }
                else {
                    //change fear to relief
                    _appraisal.RemoveGoal("explosion", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant);
                    _appraisal.AddGoal("explosion", 0.7f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed);
                }                
             }
            _agentComponent.DecreasePanic();   

            //wait until fear is below some threshold
            if(GetComponent<AffectComponent>().Emotion[(int)EType.Fear] <  fearThreshold ) {
                _agentComponent.ReactivateOtherBehaviors();
                _agentComponent.CalmDown();
                Destroy(this);
            }            
        }       	
	}

    void InitAppraisalStatus() {
        
        //Remove other goals
        //FUNDA??? appraisal.RemoveGoal("");
        //Fear proportional to distance
        
        _appraisal.AddGoal("explosion", 0.5f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);        
    }


    GameObject ClosestShelter() {
        GameObject[] shelters = GameObject.FindGameObjectsWithTag("Shelter");
        GameObject closestShelter = null;
        float minDist = 10000;
        foreach(GameObject g in shelters) {
            float dist = (_agentComponent.transform.position - g.transform.position).magnitude;
            if (dist < minDist) {
                minDist = dist;
                closestShelter = g;
            }
        }
        return closestShelter;
    
    }
}
