using UnityEngine;
using System.Collections;



public class GroupBuilder : MonoBehaviour {

	private int _agentCnt = 0; //number of agents in the group
    private GameObject[] _agents;

    enum RoleName {
        Audience,
        Shopper,
        Protester,
        Police
        
    }
    public void AssignAgents() {
        AgentComponent[] agentComponents = GetComponentsInChildren<AgentComponent>();
        _agentCnt = agentComponents.Length;
        _agents = new GameObject[_agentCnt];

        for (int i = 0; i < _agentCnt; i++) {
            _agents[i] = agentComponents[i].gameObject;
        }
    }

    public void Init(int role, int cnt, int groupId, int totalAgentCnt) {
		int i;		
		this._agentCnt = cnt;
		_agents = new GameObject[_agentCnt];
        string[] charNames = {

                 "Adam","Andromeda", "Brian", "Bryce","Carl","David",  "Elizabeth","James", "Joan", "Jody", "Joe",  "Josh", "Kate",  "Leonard", "Lewis", "Liam","Louise",  "Malcolm","Martha","Megan","Mia", "Michael",   "Regina", "Remy", "Roth", "Shae", "Sophie",  "Stefani",  "Suzie", "Victoria", 
                };
        
        
		for(i=0; i< _agentCnt; i++) {		
			
			if(role == (int)RoleName.Police)
				_agents[i] = (GameObject)Instantiate(UnityEngine.Resources.Load("Police"), transform.position, transform.rotation);
			
			else {

                _agents[i] = (GameObject)Instantiate(UnityEngine.Resources.Load(charNames[i%charNames.Length]), transform.position, transform.rotation);
            }





            

            _agents[i].AddComponent<AgentComponent>();
			_agents[i].AddComponent<AnimationSelector>();
			_agents[i].AddComponent<Appraisal>();
            _agents[i].AddComponent<AffectComponent>();	
			_agents[i].AddComponent<PersonalityMapper>();
            _agents[i].AddComponent<PostureAnimator>();

            
            //_agents[i].AddComponent<Object2Record>();
            //_agents[i].GetComponent<Object2Record>().enabled = false;
		
			
			_agents[i].GetComponent<AgentComponent>().GroupId = groupId;
			_agents[i].GetComponent<AgentComponent>().Id = totalAgentCnt + i;


            
            _agents[i].tag = "Player";
						
			

            _agents[i].transform.parent = GameObject.Find("CrowdGroup" + groupId).transform;						
			
		
			switch(role) {				
				case (int)RoleName.Protester:					
					_agents[i].AddComponent<ProtesterBehavior>();
                    GameObject protestCenter = GameObject.Find("ProtestCenter");
                    if(protestCenter == null)
                    {
                        protestCenter = new GameObject("ProtestCenter");
                        protestCenter.AddComponent<UpdateProtestCenter>();
                    }
					break;
              
				case (int)RoleName.Police:
					_agents[i].AddComponent<PoliceBehavior>();
                    break;

            	
                case (int)RoleName.Shopper:
                    _agents[i].AddComponent<ShopperBehavior>();
                    break;

                
                case (int)RoleName.Audience:
                    _agents[i].AddComponent<AudienceBehavior>();
                    break;
			}
			
		}

        //Group components
        switch(role){				
				case (int)RoleName.Protester:
                    GameObject grp; 
                    grp = GameObject.Find("CrowdGroup" + groupId);
					if(grp.GetComponent<LeaderComponent>() == null) //Leader should be added manually
						grp.AddComponent<LeaderComponent>();									
                    break;
                case (int)RoleName.Police:
                    grp = GameObject.Find("CrowdGroup" + groupId);
					if(grp.GetComponent<ZoneComponent>() == null) //Protection zone
				    grp.AddComponent<ZoneComponent>();
                    grp.GetComponent<ZoneComponent>().ComputeProtectionZone();
                    break;
         }
				   
	}
	
	public void UpdatePersonalityAndBehavior(float[] persMean, float[] persStd) {

        for (int i = 0; i < _agents.Length; i++) {

            if (_agents[i] != null) {
                _agents[i].GetComponent<AffectComponent>().UpdatePersonality(persMean, persStd);
                //Update steering behaviors according to Personality parameters
                _agents[i].GetComponent<PersonalityMapper>().PersonalityToSteering();
            }
		}		
	}
	
    public void UpdateRegion(float rectX, float rectZ) {
        for (int i = 0; i < _agentCnt; i++){
            //agents[i].transform.Translate(transform.position - agents[i].transform.position);
            _agents[i].transform.position = transform.position + new Vector3(MathDefs.GetRandomNumber(-rectX, rectX), 0, MathDefs.GetRandomNumber(-rectZ, rectZ));               
        }
    }

   
	
}
