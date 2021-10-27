//#define ESCAPES
#define ASCRIBE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent (typeof (AffectComponent))]
public class AgentComponent: MonoBehaviour, GeneralStateComponent {
    public MeshCreator VisibilityMesh;
    ParticleSystem _particleSystem;
    UnityEngine.AI.NavMeshAgent _navMeshAgent;
    Appraisal _appraisal;
    AffectComponent _affectComponent;
    private AnimationSelector _animationSelector;

    public GameObject IndicatorAgent;
    public GameObject IndicatorParticle;
    public GameObject IndicatorCircle;
    
    public int Id;
	public float WalkingSpeed = 1f; //Computed according to Personality
    private Vector3 _initialPos;
    public float Damage;
    public int GroupId;
	public float PanicThreshold = 0.5f; //between 0 and 1 computed according to Personality ( Initial Personality 0)
    public float WaitingThreshold = 3f;
	public float PanicLevel;
	public ArrayList CollidingAgents;
    public float TimeLastFight = 0f; //end time of the last fight
    public float TimeLastFall = 0f; //end time of the last fight
    public float WaitDuration; //Duration to wait until a new path is set. Proportional to patience
    private  float VisibilityAngle = 60;//Mathf.PI/ 3f; //Mathf.PI * 2f; //Mathf.PI / 3f;
    float _visibilityDist;
    
    Color _expressionColor;
    private float _colorTime;

    Vector3 _prevPos; //position in a previous frame
    private float _posChange;
    private float _timeLastPosChange;
    public bool WaitedTooLong = false;
    public bool WaitingOnPurpose = false;
    public bool IsFightStarter = false;
    public bool IsWatchingFight = false; //is the agent watching a fight. updated in fightbehavior
    public Vector3 Impact; //character momentum
    //DEBUG
    public bool IsPushed = false;
    public bool IsFallen = false;
    public bool StartedWaiting = false;
    public bool FinishedWaiting = false;
    public Vector3 HandPos; //for picking up objects
    public bool NavMeshAgentWorking;

	// Use this for initialization
	void Awake () {
        VisibilityMesh = null;
        CollidingAgents = new ArrayList(); //agents that are colliding with me

        if(!GetComponent<UnityEngine.AI.NavMeshAgent>())
            gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();

        _animationSelector = GetComponent<AnimationSelector>();
        _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _appraisal = GetComponent<Appraisal>();
        _affectComponent = GetComponent<AffectComponent>();
        _navMeshAgent.speed = WalkingSpeed; //maximum speed
        _navMeshAgent.angularSpeed = 360;
        _navMeshAgent.updatePosition = true; //false ? for resetting the position
        GetComponent<PersonalityMapper>().PersonalityToSteering(); //initialize steering parameters according to Personality
        _initialPos = transform.position;
        
        //Set collider size :WARNING: Temporary
        SphereCollider col = (SphereCollider)this.GetComponent<Collider>();
        col.radius = 2f; //to test escapes and ascribe --normally 4f
#if ASCRIBE
	    col.radius = 10f;
	    VisibilityAngle = 360;
#endif
        _visibilityDist = col.radius; //* 2f;
        _prevPos = transform.position;
        _navMeshAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;//ObstacleAvoidanceType.NoObstacleAvoidance; 
        _timeLastPosChange = Time.time;

	    Impact = Vector3.zero;
	}
       
    void Start() {
        IndicatorAgent = (GameObject)Instantiate(Resources.Load("Indicator"), transform.position + Vector3.up, transform.rotation);
        IndicatorParticle = (GameObject)Instantiate(Resources.Load("IndicatorParticle"), transform.position + Vector3.up *2.2f, transform.rotation);
        IndicatorCircle = (GameObject)Instantiate(Resources.Load("IndicatorCircle"), transform.position + Vector3.up * 2.3f , transform.rotation);
        IndicatorCircle.transform.parent = transform;
        IndicatorCircle.transform.localScale = new Vector3(_navMeshAgent.radius * 2, 0, _navMeshAgent.radius * 2);
        
        IndicatorParticle.transform.Rotate(-90, 0, 0); //for the particle system to be parallel to the table plane
        IndicatorAgent.transform.parent = transform;        
        IndicatorParticle.transform.parent = transform;
        IndicatorAgent.transform.localScale = new Vector3(_navMeshAgent.radius, 1f, _navMeshAgent.radius); //height for seeing indicator
        _particleSystem = new ParticleSystem();
        
        _particleSystem = IndicatorParticle.GetComponent<ParticleSystem>();
        IndicatorParticle.GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, 0f); //always invisible 
        IndicatorParticle.SetActive(false);
        _particleSystem.enableEmission = false;
        _expressionColor = new Color(1,1,1);
        _timeLastPosChange = Time.time;

        if(GetComponentInChildren<AnimationSelector>() &&  GetComponent<AnimationSelector>().enabled) {
            IndicatorAgent.SetActive(false);
            IndicatorCircle.SetActive(false);
        }
        else {
            IndicatorAgent.SetActive(true);
            IndicatorCircle.SetActive(false);
        }
        Random.seed = Id;
    }

	public void Restart() {      
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = WalkingSpeed; //maximum speed
        GetComponent<UnityEngine.AI.NavMeshAgent>().angularSpeed = 360;
        GetComponent<UnityEngine.AI.NavMeshAgent>().updatePosition = false; //for resetting the position
        GetComponent<PersonalityMapper>().PersonalityToSteering(); //update steering parameters in navmesh according to Personality
        transform.position = _initialPos;
        Damage = 0f;
        _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        
        if(IsFighting()) 
        {
            DestroyImmediate(GetComponent<FightBehavior>());
        }

        _timeLastPosChange = Time.time;
        IsWatchingFight = false;
        Impact = Vector3.zero;
        GetComponent<UnityEngine.AI.NavMeshAgent>().updatePosition = true;
    }

    void RenderViewingZone() {
        int quality = 20;
        float distMin = 0.0f;
        float distMax = _visibilityDist;
        Vector3 pos = IndicatorAgent.transform.position;
       // float angle_lookat = (float)Math.Acos(Vector3.Dot(new Vector3(1, 0, 0), GetComponent<NavMeshAgent>().velocity)) * Mathf.Deg2Rad;
        float angleLookat = MathDefs.VectorAngle(new Vector3(1,0,0),_navMeshAgent.velocity)* Mathf.Deg2Rad; //initial looking angle is in the x direction
        float angleFov = VisibilityAngle; 
        float angleStart = angleLookat - angleFov;
        float angleEnd = angleLookat + angleFov;
        float angleDelta = (angleEnd - angleStart) / (float)quality;
        float angleCurr = angleStart;
        float angleNext = angleStart + angleDelta;
        List <Vector3> nodePositions = new List<Vector3>();

        for (int i = 0; i < quality - 1; i++) {
            Vector3 sphereCurr = new Vector3(0,0,0);
            sphereCurr.x = Mathf.Cos(angleCurr);
            sphereCurr.z = Mathf.Sin(angleCurr);
            sphereCurr.y = pos.y;
            Vector3 sphereNext;
            sphereNext.x = Mathf.Cos(angleNext);
            sphereNext.z = Mathf.Sin(angleNext);
            sphereNext.y = pos.y;
            Vector3 posCurrMin = pos + sphereCurr * distMin;
            Vector3 posCurrMax = pos + sphereCurr * distMax;
            Vector3 posNextMin = pos + sphereNext * distMin;
            Vector3 posNextMax = pos + sphereNext * distMax;
            posCurrMin.y = posCurrMax.y = posNextMin.y = posNextMax.y = pos.y;
            nodePositions.Add(posCurrMin);
            nodePositions.Add(posCurrMax);
            nodePositions.Add(posNextMax);
            angleCurr += angleDelta;
            angleNext += angleDelta;
        }
        VisibilityMesh.UpdatePolygon(nodePositions);
        VisibilityMesh.Mat.color = new Color(_expressionColor.r, _expressionColor.g, _expressionColor.b, 0.2f);

    }

    void UpdateWaitingTime(){
        if ((Time.time - _timeLastPosChange) >= 5) {
            _posChange = Vector3.Distance(transform.position, _prevPos);
            _timeLastPosChange = Time.time;
            if (WaitingOnPurpose || _posChange >= WaitingThreshold) {//(Time.deltaTime * _navMeshAgent.speed) / 1.5f) {
                WaitedTooLong = false;
            }
            else {
                WaitedTooLong = true;       
            }
            _prevPos = transform.position;
        }
    }

    void LateUpdate() {
        if (IsFallen) {
           IndicatorAgent.transform.Rotate(90, 0, 0);
        }

        if (Impact.magnitude > 0.2) {
            SteerTo(transform.position + Impact * Time.deltaTime);
            _navMeshAgent.updateRotation = false; 
            _navMeshAgent.updateRotation = false; 
            _navMeshAgent.updateRotation = false; 
        }
        else {
            _navMeshAgent.updateRotation = true;
        }

        // impact vanishes to zero over time
        Impact = Vector3.Lerp(Impact, Vector3.zero, 2 * Time.deltaTime);
    }
    void Update() {
        AffectUpdate(); //our method
        _expressionColor = _affectComponent.GetExpressionColor();
        UpdateIndicator();

        if (IsFighting() == false) {
            if (TimeSinceLastFight() > 10) //start healing after 10 seconds
                Heal();
        }
        if (IsDead()) {
           // _navMeshAgent.Stop();
            _navMeshAgent.updatePosition = false;
            _navMeshAgent.updateRotation = false;
            return;
        }
       
       //FALLING TURNED OFF
       if(Time.timeSinceLevelLoad > 3) //skip the beginning where agents collide
            CheckFallingStandingUp();
       
        if (IsFallen) {
            _navMeshAgent.updatePosition = false;
            _navMeshAgent.updateRotation = false;
        }
            
        NavMeshAgentWorking = _navMeshAgent.updatePosition;

        if(VisibilityMesh != null)
            RenderViewingZone();
#if ESCAPES
        EmotionalBehaviorUpdateEscapes();
      
#else
           EmotionalBehaviorUpdate();
#endif
    }

    void EmotionalBehaviorUpdateEscapes() {
        _navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, 3.8f, _affectComponent.Emotion[(int)EType.Fear]);
    }

    void EmotionalBehaviorUpdateAscribe() {
        _navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, 3.8f, _affectComponent.Emotion[(int)EType.Fear]);
    }

    //Select behaviors based on emotions
    //Agent component script is executed before role components, so current actions in role components overwrite the ones here
    void EmotionalBehaviorUpdate() {
        switch (_affectComponent.GetCurrMoodOctant()) {            
            case (int)MType.Hostile:
                if (!IsPolice()) {
                    if (_affectComponent.GetExpressionRange() == EmotionRange.High) {
                        _navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, 3.8f, Time.deltaTime);
                        foreach (GameObject c in CollidingAgents) {
                            if (IsGoodToFight(c)) {
                                StartFight(c, true);
                                c.GetComponent<GeneralStateComponent>().StartFight(gameObject, false);                                
                            }
                        }
                    }
                }

                //If still not fighting
                if (!IsFighting() && !IsPolice()) { 
                    int val = Random.Range(0, 2);
                    if (val == 0)
                        _animationSelector.SelectAction("YELL0");
                    //CurrAction[3] = "yelling0";
                    else
                        _animationSelector.SelectAction("YELL1");
                        //CurrAction[3] = "yelling1";
                }
                else if (_affectComponent.GetExpressionRange() == EmotionRange.Moderate) {
                    //If not found someone to fight, just yell
                    if (!IsFighting() && !IsPolice()) { 
                        int val = Random.Range(0, 2);
                        if (val == 0)
                            _animationSelector.SelectAction("YELLG0");
                            //CurrAction[3] = "yelling0";
                        else
                            _animationSelector.SelectAction("YELL1");
                            //CurrAction[3] = "yelling1";
                    }
                }
                else //low anger
                    _navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, WalkingSpeed, Time.deltaTime);
                break;
            case (int)MType.Exuberant:
                if (IsProtester())
                    _animationSelector.SelectAction("CLAPPING");
                else if(IsShopper()) {
                     _navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, 3f, Time.deltaTime);                  
                }                              
                break;
            case (int)MType.Docile:
                _navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, WalkingSpeed, Time.deltaTime);
                break;
            case (int)MType.Relaxed:
                _navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, WalkingSpeed, Time.deltaTime);
                break;   
            case (int)MType.Anxious:
                if (_affectComponent.GetExpressionRange() == EmotionRange.High) {
                    _navMeshAgent.speed =  Mathf.Lerp(_navMeshAgent.speed, 3.8f, Time.deltaTime);
                }
                else if (_affectComponent.GetExpressionRange() == EmotionRange.Moderate)
                    _navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, 3.8f, Time.deltaTime);
                else
                    _navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, WalkingSpeed, Time.deltaTime);
                break;
            case (int)MType.Bored:
                if (_affectComponent.GetExpressionRange() == EmotionRange.High) {
                    _navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, WalkingSpeed, Time.deltaTime);
                    if(MathDefs.GetRandomNumber(5) > 3)
                        _animationSelector.SelectAction("DISAPPOINTED");
                }
                break;
          
            case (int)MType.Disdainful:
                     if (_affectComponent.GetExpressionRange() == EmotionRange.High)
                         _animationSelector.SelectAction("DISAPPOINTED");
                     break;
        }

        if (!IsFighting()) {
            if (IsProtester() && GetComponent<ProtesterBehavior>().BannerCarrier &&
                GetComponent<ProtesterBehavior>().Banner.activeInHierarchy) {
                    _animationSelector.SelectAction("HOLDBANNER");
            }
        }
    }

    void AffectUpdateAscribe() {
        float qR = _affectComponent.Emotion[(int) EType.Fear]; //my fear level
        foreach (GameObject c in CollidingAgents) {}
    }

    void AffectUpdateEscapes() {
        _navMeshAgent.updatePosition = true;
        _navMeshAgent.updateRotation = true;
        float maxFear = -1000;
        foreach (GameObject c in CollidingAgents) {
            //Ensures that agents within a radius are considered
            if (!c.CompareTag("RealPlayer")) {
                float fear = c.GetComponent<AffectComponent>().Emotion[(int) EType.Fear];
                if (fear >= maxFear) {
                    maxFear = fear;
                }
            }
        }

        if(maxFear > -1000)
        _affectComponent.Emotion[(int)EType.Fear] = maxFear;
        _affectComponent.ComputeMood();
    }
  
	void AffectUpdate () {
        if (!IsFighting()) {
            _navMeshAgent.updatePosition = true;
            _navMeshAgent.updateRotation = true;
        }
	    float [] eventFactor = _appraisal.ComputeEventFactor();             
        //only if susceptible 
        
        //Emotion contagion
        Dictionary<int, float> lambdaList = new Dictionary<int, float>();//indices of dominant emotions around me
        //List<IndexValuePair> lambdaList = new List<IndexValuePair>(); //indices of dominant emotions around us
        if (_affectComponent.ContagionMode) {
            foreach (GameObject c in CollidingAgents) {
                    //Ensures that agents within a radius are considered
                    if (!c.CompareTag("RealPlayer") && IsVisible(c, VisibilityAngle)) {
                        //if c is in my visual cone and within a certain proximity                
                        for (int eInd = 0; eInd < c.GetComponent<AffectComponent>().Emotion.Length; eInd++) {
                            if (c.GetComponent<AffectComponent>().CanExpress(eInd)) {                  
                                if (lambdaList.ContainsKey(eInd)) {
                                    lambdaList[eInd] += c.GetComponent<AffectComponent>().Emotion[eInd];
                                    lambdaList[eInd] /= 2; //for normalization
                                }
                                else
                                    lambdaList.Add(eInd, c.GetComponent<AffectComponent>().Emotion[eInd]);
                            }
                        }
                    }
            }
        }
	    _affectComponent.ComputeEmotion(eventFactor, lambdaList);
	    lambdaList.Clear();
	    _affectComponent.ComputeMood();
	}

    void UpdateIndicator() {
        if (IndicatorAgent.activeInHierarchy) {
            IndicatorAgent.GetComponent<Renderer>().material.color = Color.Lerp(IndicatorAgent.GetComponent<Renderer>().material.color, _expressionColor, _colorTime);
            _colorTime +=  0.1f*Time.deltaTime;
            if (_colorTime >= 1) _colorTime = 0f;
        }

        if (IndicatorCircle.activeInHierarchy) {
            IndicatorCircle.GetComponent<Renderer>().material.SetColor("_Emission", _expressionColor);
        }

        if (IndicatorParticle.activeInHierarchy) {
            _particleSystem.enableEmission = true;
            _particleSystem.startColor = new Color(_expressionColor.r, _expressionColor.g, _expressionColor.b, 1f);
            _particleSystem.emissionRate = 100f;       
            _particleSystem.startSize = 0.1f;
            _particleSystem.startSpeed = 0.1f * +_navMeshAgent.radius; //0.1f *2* ((SphereCollider)collider).radius;//  //determines the size of the radius
            _particleSystem.startDelay = 0f;
            ParticleSystem.Particle[] p = new ParticleSystem.Particle[_particleSystem.particleCount + 1];
            int pCnt = _particleSystem.GetParticles(p);
            int [] padRatios = new int[3];
            float moodCnt = (Mathf.Abs(_affectComponent.Mood.x) + Mathf.Abs(_affectComponent.Mood.y) + Mathf.Abs(_affectComponent.Mood.z));
            padRatios[0] = (int)((Mathf.Abs(_affectComponent.Mood.x) * pCnt) / moodCnt);
            padRatios[1] =(int)((Mathf.Abs(_affectComponent.Mood.y) * pCnt) / moodCnt);
            padRatios[2] =(int)((Mathf.Abs(_affectComponent.Mood.z) * pCnt) / moodCnt);
            int k = 0;
            for (int i = 0; i < padRatios.Length; i++) {
                for (int j = 0; j < padRatios[i]; j++) {
                    Color ec = _affectComponent.GetPADColor(i);
                    p[k++].color = new Color(ec.r, ec.g, ec.b, 1f);
                }
            }
            _particleSystem.SetParticles(p, pCnt);
        }
        else {
            _particleSystem.enableEmission = false;
        }        
    }

    public void SteerTo(Vector3 pos) 
    {
        _navMeshAgent.SetDestination(pos);        
	}
	
	///Steer away from the location pos
	public void SteerFrom(Vector3 pos) 
    {        
        Vector3 dir = transform.position - pos;
        dir.y = 0f;
        dir.Normalize();
        _navMeshAgent.SetDestination(transform.position + dir);			
	}
	
	/// Finds the agents around me
	void OnTriggerEnter(Collider collider) {              
		if(collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("RealPlayer")) {		    
            if(!CollidingAgents.Contains(collider))
				CollidingAgents.Add(collider.gameObject);          
		}		
	}
	
	void OnTriggerExit(Collider collider) {	
		if(collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("RealPlayer"))
        {					
			CollidingAgents.Remove(collider.gameObject);            
		}		
	}
	
	public void AddDamage(float amount) {
	    Damage += amount*Random.Range(0, 2) * Time.deltaTime;
	}

    public bool isLost() {
        return Damage > 0.1;
    }

	public void Heal() {
        if (IsWounded()) {
            Damage -= 0.1f * Time.deltaTime;
            if (!IsWounded())
                ResetSpeed();
        }         
	}
	public bool IsDead() {
	    return Damage > 3;
	}
	
	public bool IsWounded() {
	    return (!IsDead() && Damage > 1);
	}
		
	public void AdjustEmotionalSpeed() {
	    
	}

	public void IncreaseSpeed(int c) {
		float maxSpeed = 3.8f;
		_navMeshAgent.speed += c * 0.2f*Time.deltaTime;
        if (_navMeshAgent.speed > maxSpeed)
            _navMeshAgent.speed = maxSpeed; 		
	}

	public void DecreaseSpeed(int c) {
        _navMeshAgent.speed -= c * 0.2f * Time.deltaTime;
        if (_navMeshAgent.speed <= WalkingSpeed)
            _navMeshAgent.speed = WalkingSpeed; 
	}
    
    public void ResetSpeed() {
        _navMeshAgent.speed = WalkingSpeed; 		
    }
	
	public void IncreasePanic() {
		PanicLevel += 0.1f*Time.deltaTime;
		if(PanicLevel > 1f)
			PanicLevel = 1f;		
		
		if(IsPanicking())
			IncreaseSpeed(2);
		
	}
	public void DecreasePanic() {	
		PanicLevel -=0.1f*Time.deltaTime;					
		if(PanicLevel < 0f)
			PanicLevel = 0f;

        if (IsPanicking() && _navMeshAgent.speed > WalkingSpeed) 
			DecreaseSpeed(1);
		else
            _navMeshAgent.speed = WalkingSpeed;			
	}

    public void CalmDown() {
        PanicLevel = 0f;
        _navMeshAgent.speed = WalkingSpeed;			
    }
	
	public bool IsPanicking() {
		return PanicLevel > PanicThreshold;			
	}
	
	/// If other is in my visual cone 
	public bool IsVisible(GameObject other, float viewAngle) {
        Vector3 orientation = transform.forward;//GetComponent<NavMeshAgent>().velocity;
		Vector3 distVec = other.transform.position - transform.position ;
        orientation.y = distVec. y  = 0f;        
		orientation.Normalize();
		distVec.Normalize ();
        float angle = Vector3.Angle(orientation, distVec);//Mathf.Acos(Vector3.Dot(distVec, orientation));
        if (angle <= VisibilityAngle)
			return true;
		return false; 
	}

    public void LookAt(Vector3 dest, float speed) {
        Vector3 lookDir = dest - transform.position;
        lookDir.y = 0;
        _navMeshAgent.updateRotation = false; //rotate towards the desired object

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), Time.time * speed);
    }

    public void Watch(GameObject other) 
    {
        Quaternion wantedRotation = Quaternion.LookRotation(other.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, wantedRotation, Time.deltaTime);
    }
    public void Watch(Vector3 pos) 
    {
        Quaternion wantedRotation = Quaternion.LookRotation(pos - transform.position, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, wantedRotation, Time.deltaTime);
    }
    public void Face(GameObject other) 
    {
        if (other.CompareTag("Player")) {
            SteerTo(other.transform.position + other.GetComponent<UnityEngine.AI.NavMeshAgent>().desiredVelocity.normalized);
        }
        else if (other.CompareTag("RealPlayer")){
            SteerTo(other.transform.position);
        }
    }

    public bool IsFacing(GameObject other) {
        //(other.transform.position - transform.position).magnitude < 2f &&
        return (IsVisible(other, Mathf.PI / 3f) && other.GetComponent<GeneralStateComponent>().IsVisible(this.gameObject, Mathf.PI / 3f));
    }
    
    public void AddImpact(Vector3 force) {

        Impact += force; //dir.normalized * force.magnitude / 2f; //vel/ mass

    }
    public bool IsBeingPushed() {
        float pushCoef = Mathf.Abs(_navMeshAgent.velocity.magnitude/_navMeshAgent.speed);
        IsPushed = pushCoef > 5  && pushCoef < 10;
        return IsPushed;
    }

    void Fall() {
        if (!IsPolice()) {
            if (!IsFallen) {
                IsFallen = IsBeingPushed() && Random.Range(0, 40) < 1;
                if (IsFallen) {
                    _navMeshAgent.updatePosition = false;
                    _navMeshAgent.updateRotation = false;
                    _navMeshAgent.avoidancePriority = 0;
                    _navMeshAgent.angularSpeed = 0;
                    _navMeshAgent.speed = 0;
                    _navMeshAgent.radius = 1.2f;
                    IndicatorAgent.transform.Translate(0, -1.5f, 0);
                    UpdateAppraisalStatus();
                    TimeLastFall = Time.time;
                }
            }
        }

    }

    void StandUp() { //stand up from falling position
        if (IsFallen) {  //if enough time has passed
            //if(Random.Range(0, 10) < 1) {            
            if(Time.time - TimeLastFall > 10){
                _navMeshAgent.updatePosition = true;
                _navMeshAgent.updateRotation = true;
            //    _navMeshAgent.Resume();
                gameObject.GetComponent<PersonalityMapper>().PersonalityToSteering(); //reset to default values
                IsFallen = false;
                IndicatorAgent.transform.Translate(0, 1.5f, 0);
            }
        }
    }

    void CheckFallingStandingUp() {        
        Fall();
        StandUp();
        if (IsFallen) {                        
            AddDamage(0.1f); //add damage  
        }

    }

    public bool IsProtester() {
        if (GetComponent<ProtesterBehavior>() != null)
            return true;
        return false;
        
    }
    
    public bool IsPolice() {
        return GetComponent<PoliceBehavior>() != null;
    }

    public bool IsShopper() {
        return GetComponent<ShopperBehavior>() != null;
    }

    public bool IsFighting() {
        return (GetComponent<FightBehavior>() != null);        
    }
    public bool HasFallen() {
        return IsFallen;
    }
    public bool CanFight() {
        if (IsFighting()) //already fighting
            return false;
        if (IsWounded())
            return false;
        if (IsFallen)
            return false;
        return true;
    }

    public IEnumerator WaitAWhile(int seconds) {
        StartedWaiting = true;
        FinishedWaiting = false;
        yield return new WaitForSeconds(seconds);
        
        FinishedWaiting = true;
        
        //Dont't forget to set started waiting to false before state change
    }
    
    ///Returns true if a fight conditions are met
    public bool IsGoodToFight(GameObject other) {
        //dist < 5f seklindeydi
        if (Vector3.Distance(other.transform.position, transform.position) < 3f && CanFight() && other.GetComponent<GeneralStateComponent>().CanFight())
            // only fight with real player ?
            if (other.CompareTag("RealPlayer") && Input.GetKey(KeyCode.F)) {
                return true;
            }
            // if (other == null) {
            //     Debug.LogError("Opponent is null in fight");
            //     return false;
            // }   
            if (other.CompareTag("Player") && (_appraisal.DoesStandardExist(other, AppDef.Disapproving) || _appraisal.DoesStandardExist(other.transform.parent.gameObject, AppDef.Disapproving))) 
                return true;
        return false;
    }

    public bool IsGoodToAttack(GameObject other) {
        if (Vector3.Distance(other.transform.position, transform.position) < 5f && CanFight() && other.GetComponent<GeneralStateComponent>().CanFight())
            return true;
        else
            return false;
    }
     
	/// Start a fight with the other agent	
	public void StartFight(GameObject other, bool isStarter) {
        IsFightStarter = isStarter;
        if (other == null) {
            Debug.LogError("Opponent is null in fight");
            return;
        }
		if(GetComponent("FightBehavior") == null) {
			this.gameObject.AddComponent<FightBehavior>();            
		    GetComponent<FightBehavior>().Init(other);
            IsFightStarter = isStarter;
		}				
	}

    public float TimeSinceLastFight() {
        return Time.time - TimeLastFight;
    }
	
	public void DeactivateOtherBehaviors(){
        if (GetComponent<ProtesterBehavior>()) {
            GetComponent<ProtesterBehavior>().enabled = false;
            if (GetComponent<ProtesterBehavior>().BannerCarrier)
                GetComponent<ProtesterBehavior>().Banner.SetActive(false);
        }
        if (GetComponent<PoliceBehavior>()) {
            GetComponent<PoliceBehavior>().enabled = false;
            GetComponent<PoliceBehavior>().Shield.SetActive(false);
            GetComponent<PoliceBehavior>().NightStick.SetActive(false);
        }

        //Stop fighting    
        if (GetComponent<FightBehavior>()) {
            GetComponent<FightBehavior>().FinishFight();           
        }

        GetComponent<UnityEngine.AI.NavMeshAgent>().updateRotation = true; //in case police vigiling is stopped in the middle
        
    }
    public void ReactivateOtherBehaviors()
    {
        if (GetComponent<ProtesterBehavior>()) {
            GetComponent<ProtesterBehavior>().enabled = true;
            if(GetComponent<ProtesterBehavior>().BannerCarrier && !IsFallen && !IsDead()){
                GetComponent<ProtesterBehavior>().Banner.SetActive(true);
                GetComponent<ProtesterBehavior>().Enable();
            }
        }
        if (GetComponent<PoliceBehavior>()) {
            GetComponent<PoliceBehavior>().enabled = true;
            if( !IsFallen && !IsDead())
                GetComponent<PoliceBehavior>().Shield.SetActive(true);
            GetComponent<PoliceBehavior>().Enable();
        }
        
    }

    //Appraisal status for falling
    void UpdateAppraisalStatus() {
        if (IsFallen)
            _appraisal.AddGoal("falling", 0.5f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);
        else {
            float wt = _appraisal.RemoveGoal("falling", AppDef.Displeased, AppDef.ConsequenceForSelf,
                                                AppDef.ProspectRelevant, AppDef.Confirmed);
            if(wt != 0) {
                _appraisal.AddGoal("falling", 0.5f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed);
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.magenta;
        if (_navMeshAgent)
            Gizmos.DrawLine(transform.position, transform.position + Impact);
    }
}

	

