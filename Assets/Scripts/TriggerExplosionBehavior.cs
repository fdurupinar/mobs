using UnityEngine;
using System.Collections;

public class TriggerExplosionBehavior : MonoBehaviour {
   
	void Start () {
        GameObject[] agents = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject a in agents) {
            if ( a.GetComponent<PoliceBehavior>()==null && a.GetComponent<ExplosionBehavior>()==null) {                
                a.AddComponent<ExplosionBehavior>(); //being scared of explosion and running away
                //ExplosionBehavior[] exps = a.GetComponents<ExplosionBehavior>();
                //exps[exps.Length - 1].Explosion = this.gameObject;
            }
        }

        
	  }	
}
