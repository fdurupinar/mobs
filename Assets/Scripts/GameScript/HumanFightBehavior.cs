using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumanFightBehavior : MonoBehaviour
{
    AgentComponent opponentComponent; // opponent
	HumanComponent humanComponent;
	public GameObject Opponent;
	public float BeginTime;
	public float EndTime;
	private Inventory inventory;
	//private Inventory oInventory;

	public void Init(GameObject o) {
		Opponent = o;
        humanComponent = GetComponent<HumanComponent>();
		opponentComponent = Opponent.GetComponent<AgentComponent>();
        BeginTime = Time.time;
    }

	private void Start() {
		inventory = GameObject.FindGameObjectWithTag("RealPlayer").GetComponent<Inventory>();
		//oInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
	}
	void Update () { // grab products or lost products
		if(!opponentComponent.IsFighting() || opponentComponent.IsWounded()  || opponentComponent.HasFallen()) {            
            EndTime = Time.time;
			FinishFight();
		}
		else {
			if (Input.GetKey(KeyCode.F)) { 
				humanComponent.AddDamage(opponentComponent.IsFighting() ? 0.4f : 0f); // add damage to itself
				//opponentComponent.AddDamage(0.4f); // no need add damage to AI agent, they'll add damage by themself
				Debug.Log("Real Player's damage points: " + humanComponent.Damage);
				Debug.Log("AI's damage  points: " + opponentComponent.Damage);

				if (opponentComponent.isLost()) {
					GrabProdcut();
				}

				if (humanComponent.isLost()) {
					LostProduct();
				}
			}
		} 
	}

	private void LostProduct() {
		for (int i = inventory.slots.Length - 1; i >= 0; i--) {
			if (inventory.isFull[i]) {
                inventory.slots[i].GetComponent<Image>().sprite = inventory.box;
                inventory.isFull[i] = false;
                break;
            }
		}

	}

	private void GrabProdcut() { 
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (inventory.isFull[i] == false)
            {
                int idx = Random.Range(0, 3);
                inventory.slots[i].GetComponent<Image>().sprite = inventory.sprites[idx];
                inventory.isFull[i] = true;
                break;
            }

        }
    }

	public void FinishFight() {
        opponentComponent.TimeLastFight = Time.time;
        DestroyImmediate(this);
	}
}
