using UnityEngine;
using System.Collections;

public class CrowdManager : MonoBehaviour {


    public void Restart() {
      
        AgentComponent[] agentComponents = FindObjectsOfType(typeof(AgentComponent)) as AgentComponent[];
        foreach(AgentComponent a in agentComponents)
            a.Restart();
    
        AffectComponent[] affectComponents = FindObjectsOfType(typeof(AffectComponent)) as AffectComponent[];
        foreach (AffectComponent a in affectComponents)
            a.Restart();


        Appraisal[] appraisalComponents = FindObjectsOfType(typeof(Appraisal)) as Appraisal[];
        foreach (Appraisal a in appraisalComponents)
            a.Restart();
				
        ProtesterBehavior[] protesterComponents = FindObjectsOfType(typeof(ProtesterBehavior)) as ProtesterBehavior[];
        foreach (ProtesterBehavior p in protesterComponents)
            p.Restart();

        PoliceBehavior[] policeComponents = FindObjectsOfType(typeof(PoliceBehavior)) as PoliceBehavior[];
        foreach (PoliceBehavior p in policeComponents)
            p.Restart();

        ShopperBehavior[] shopperComponents = FindObjectsOfType(typeof(ShopperBehavior)) as ShopperBehavior[];
        foreach (ShopperBehavior s in shopperComponents)
            s.Restart();

        PassengerBehavior[] passengerComponents = FindObjectsOfType(typeof(PassengerBehavior)) as PassengerBehavior[];
        foreach (PassengerBehavior p in passengerComponents)
            p.Restart();

        AudienceBehavior[] audienceComponents = FindObjectsOfType(typeof(AudienceBehavior)) as AudienceBehavior[];
        foreach (AudienceBehavior a in audienceComponents)
            a.Restart();

        AnimationSelector[] animationComponents = FindObjectsOfType(typeof(AnimationSelector)) as AnimationSelector[];
        foreach (AnimationSelector a in animationComponents)
            a.Restart();

        PostureAnimator[] postureComponents = FindObjectsOfType(typeof(PostureAnimator)) as PostureAnimator[];
        foreach (PostureAnimator p in postureComponents)
            p.Restart();



    }
}
