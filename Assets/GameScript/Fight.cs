using UnityEngine;
using System.Collections;


//------------------------------------------
public class Fight : MonoBehaviour
{
	//------------------------------------------
	// Add FLEE state
	public enum ENEMY_STATE { PATROL, ATTACK};
	//------------------------------------------
	public ENEMY_STATE CurrentState
	{
		get { return CurrentState; }

		set
		{
			//Update current state
			currentstate = value;

			//Stop all running coroutines
			StopAllCoroutines();

			switch (currentstate)
			{
				case ENEMY_STATE.PATROL:
					StartCoroutine(AIPatrol());
					break;

				case ENEMY_STATE.ATTACK:
					StartCoroutine(AIAttack());
					break;
			}
		}
	}
	//------------------------------------------
	[SerializeField]
	private ENEMY_STATE currentstate = ENEMY_STATE.PATROL;

	//Reference to line of sight component
	private LineSight ThisLineSight = null;

	//Reference to nav mesh agent
	private UnityEngine.AI.NavMeshAgent ThisAgent = null;

	//Reference to player health
	private Health PlayerHealth = null;

	//Reference to player transform
	private Transform PlayerTransform = null;

	//Enemy health
	private Health EnemyHealth = null;
	public Transform EnemyTransform = null;

	//Damage amount per second
	private float MaxDamage = 10f;

	// Enemy distance
	private float EnemyDistanceRun = 5.0f;

	private Vector3 RandomDest;


	//------------------------------------------
	void Awake()
	{
		ThisLineSight = GetComponent<LineSight>();
		ThisAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		PlayerHealth = GameObject.FindGameObjectWithTag("RealPlayer").GetComponent<Health>();
		PlayerTransform = PlayerHealth.GetComponent<Transform>();

		//Get enemy health
		EnemyHealth = ThisAgent.GetComponent<Health>();
		EnemyTransform = EnemyHealth.GetComponent<Transform>();

	}
	//------------------------------------------
	void Start()
	{
		//Get random destination
		GameObject[] Destinations = GameObject.FindGameObjectsWithTag("Dest");


		//Configure starting state
		CurrentState = ENEMY_STATE.PATROL;

		RandomDest = new Vector3(Random.Range(10, 80), 0f, Random.Range(10, 80));
		ThisAgent.SetDestination(RandomDest);

	}
	//------------------------------------------
	public IEnumerator AIPatrol()
	{
		//Loop while patrolling
		while (currentstate == ENEMY_STATE.PATROL)
		{
			//Set strict search
			ThisLineSight.Sensitity = LineSight.SightSensitivity.STRICT;

			Vector3 tp = transform.position;
			tp.y = 0;
			if (Vector3.Distance(tp, RandomDest) < 2f)
			{
				RandomDest = new Vector3(Random.Range(0, 80), 0f, Random.Range(0, 80));
				ThisAgent.SetDestination(RandomDest);
			}


			//Wait until path is computed
			while (ThisAgent.pathPending)
				yield return null;


			//If we can see the target then start chasing or flee
			//if (ThisLineSight.CanSeeTarget)
			//{
			//	if (EnemyHealth.HealthPoints < 10f && Vector3.Distance(EnemyTransform.position, PlayerTransform.position) <= EnemyDistanceRun)
			//	{
			//		ThisAgent.isStopped = true;
			//		CurrentState = ENEMY_STATE.FLEE;
			//	}
			//	else
			//	{
			//		ThisAgent.isStopped = true;
			//		CurrentState = ENEMY_STATE.CHASE;
			//	}
			//	yield break;
			//}

			//Wait until next frame
			yield return null;
		}
	}
	
	//------------------------------------------
	public IEnumerator AIAttack()
	{
		//Loop while chasing and attacking
		while (currentstate == ENEMY_STATE.ATTACK)
		{
			//handle heart missing
			//if (EnemyHealth.HealthPoints < 10f)
			//{
			//	CurrentState = ENEMY_STATE.FLEE;
			//	Debug.Log("change to FLEE state");
			//	yield break;
			//}


			//Has player run away?
			if (ThisAgent.remainingDistance > ThisAgent.stoppingDistance)
			{
				if (EnemyHealth.HealthPoints < 10f)
				{
					CurrentState = ENEMY_STATE.PATROL;
					yield break;
				}

				//Change back to chase
				//CurrentState = ENEMY_STATE.CHASE;

				//yield break;
			}
			else
			{
				//if (EnemyHealth.HealthPoints < 10f)
				//{
				//	CurrentState = ENEMY_STATE.FLEE;
				//	yield break;
				//}

				//Attack
				PlayerHealth.HealthPoints -= MaxDamage * Time.deltaTime;

				// Player attack
				if (Input.GetKeyDown(KeyCode.X))
				{
					EnemyHealth.HealthPoints -= 30 * Time.deltaTime;
					Debug.Log("Enemy health: " + EnemyHealth.HealthPoints);
				}
			}

			//Wait until next frame
			yield return null;
		}

		yield break;
	}
	//------------------------------------------
}
//------------------------------------------