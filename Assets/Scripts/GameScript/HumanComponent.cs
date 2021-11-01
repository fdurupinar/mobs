using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanComponent : MonoBehaviour, GeneralStateComponent{
    public float Damage;
    public float TimeLastFight = 0f;
    public bool isFighting = false;
    public bool IsFightStarter = false;
    public Vector3 HandPos; //for picking up objects
    public bool StartedWaiting = false;
    public bool FinishedWaiting = false;


    void Start() {
        SphereCollider col = this.GetComponent<SphereCollider>();
        col.radius = 0.5f; 
    }

    public void Restart() {
        Damage = 0f;
        if(IsFighting()) {
            DestroyImmediate(GetComponent<HumanFightBehavior>());
        }
    }
    void Update() {
        if (IsFighting() == false) {
            if (TimeSinceLastFight() > 10) //start healing after 10 seconds
                Heal();
        }
    }

    public IEnumerator WaitAWhile(int seconds) {
        StartedWaiting = true;
        FinishedWaiting = false;
        yield return new WaitForSeconds(seconds);

        FinishedWaiting = true;

        //Dont't forget to set started waiting to false before state change
    }

    public bool IsFighting() {
        return (GetComponent<HumanFightBehavior>() != null);
    }
    public bool IsWounded() {
        return Damage > 1;
    }
    public bool IsPolice() {
        return true;
    }
    public bool HasFallen() {
        return false;
    }
    public bool IsVisible(GameObject other, float viewAngle) {
        return true; 
	}
    public bool CanFight() {
        if (IsFighting()) //already fighting
            return false;
        return true;
    }
    public void StartFight(GameObject other, bool isStarter) {
        IsFightStarter = isStarter;
        if (other == null) {
            Debug.LogError("Opponent is null in fight");
            return;
        }
		if(GetComponent("HumanFightBehavior") == null) {
			this.gameObject.AddComponent<HumanFightBehavior>();            
		    GetComponent<HumanFightBehavior>().Init(other);
            IsFightStarter = isStarter;
		}				
    }
    public float TimeSinceLastFight() {
        return Time.time - TimeLastFight;
    }
    public void AddDamage(float amount) {
        Damage += amount*Random.Range(0, 2) * Time.deltaTime;
    }

    public void Heal() {
        if (IsWounded()) {
            Damage -= 0.1f * Time.deltaTime;
        }         
	}
    public bool IsShopper() {
        return false;
    }

	//void OnControllerColliderHit() {

	//}
}
