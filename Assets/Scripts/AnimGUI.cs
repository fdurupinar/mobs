using UnityEngine;
using System.Collections.Generic;

//Attached to the agent
public class AnimGUI : MonoBehaviour {
    private AgentComponent _agent;
    public GameObject Opponent;
    private Vector2 _scrollPosition;
    private List<ActionType> _actions = new List<ActionType>();
	// Use this for initialization
	void Start () {
	    _agent = GetComponent<AgentComponent>();
        _actions = GetComponent<AnimationSelector>().Actions;
	}
	
	void OnGUI () {
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(100), GUILayout.Height(Screen.height * 0.98f));

	    GUILayout.Label("Base");


        foreach (ActionType t in _actions) {


            if (GUILayout.Button(t.Name)) {
             //   _agent.CurrAction[t.Layer] = t.Name;
               // if (_agent.CurrAction[0].Equals("fight0"))
                 //   _agent.StartFight(Opponent, true);
            }
        }


	    GUILayout.EndScrollView();
	}	

}
