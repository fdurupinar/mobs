using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fight : MonoBehaviour{
	public float Damage;
	private Inventory inventory;
	public AgentComponent agentComponent;
	//public FightBehavior fightBehavior;

	private void Start() {
		agentComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<AgentComponent>();
		//fightBehavior = GameObject.FindGameObjectWithTag("Player").GetComponent<FightBehavior>();
	}

	public void Restart() {
		Damage = 0f;
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			Debug.Log("Collide with Agent!");
			if (Input.GetKey(KeyCode.F)) {
				Debug.Log("Fighting!");
				agentComponent.AddDamage(0.2f);
				Debug.Log("Agent damage: " + agentComponent.Damage);
			}
			
			if (agentComponent.Damage > 1)
			{
				// get product
				Debug.Log("Get Product!");
				for (int i = 0; i < inventory.slots.Length; i++)
				{
					if (inventory.isFull[i] == false)
					{
						int idx = Random.Range(0,3);
						inventory.slots[i].GetComponent<Image>().sprite = inventory.sprites[idx];
						inventory.isFull[i] = true;
						break;
					}

				}
			}
			if (this.Damage > 1) {
				// lost product
				Debug.Log("Lost Product!");
				for (int i = inventory.slots.Length - 1; i >= 0; i--)
				{
					if (inventory.isFull[i] == true)
					{
						inventory.slots[i].GetComponent<Image>().sprite = null;
						inventory.isFull[i] = false;
						break;
					}
				}
			}
		}

	}
}
