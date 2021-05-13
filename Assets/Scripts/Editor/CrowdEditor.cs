using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

public class CrowdEditor : EditorWindow {
		
	int agentCnt = 0;	//# of agents to be added
	static int totalAgentCnt = 0;
	int agentRole = 0; //audience
	string[] roleNames = {"Audience", "Shopper", "Protester", "Police", "Passenger"};
	int[] roleInds = {0,1,2,3,4,5,6,7,8, 9};
	
	[MenuItem ("MOBS/Crowd")]
	 static void Init () {
        // Get existing open window or if none, make a new one:
        CrowdEditor window = (CrowdEditor)EditorWindow.GetWindow(typeof (CrowdEditor));		
		
	}

    void OnGUI() {

        
        GUILayout.Label("Create New Group", EditorStyles.largeLabel);

        //EditorGUILayout.BeginHorizontal();
        agentCnt = EditorGUILayout.IntField("Agent Count", agentCnt, GUILayout.ExpandWidth(true));

        agentRole = EditorGUILayout.IntPopup("Role: ", agentRole, roleNames, roleInds);
        if (GUILayout.Button("Add Group", GUILayout.ExpandWidth(false))) {

            int groupId = ComputeNewGroupId();
            GameObject go = new GameObject("CrowdGroup" + (groupId));
            GameObject crowd = GameObject.Find("Crowd");
            if (crowd == null) {
                crowd = new GameObject("Crowd");
                crowd.AddComponent<CrowdManager>();
            }

            go.transform.parent = GameObject.Find("Crowd").transform;
            //	if(selected == true) //Add to the selected location on screen
            //		go.transform.position = selectedPoint;
            //	else //add to the parent gameobject's location
            go.transform.position = go.transform.parent.transform.position;
            go.AddComponent<GroupBuilder>();
            go.GetComponent<GroupBuilder>().Init(agentRole, agentCnt, groupId, totalAgentCnt);
            totalAgentCnt += agentCnt;
        }

        if (GUILayout.Button("Update Animation Clips", GUILayout.ExpandWidth(false))) {

            GameObject crowd = GameObject.Find("Crowd");
            if (crowd == null) {
                return;
            }

            List<AnimationClip> clips = new List<AnimationClip>();
            List<string> animationNames = new List<string>();
            animationNames.Add("run");
            animationNames.Add("walk");
            animationNames.Add("idle");
            animationNames.Add("clap");
            animationNames.Add("fight");
            animationNames.Add("protest");
            animationNames.Add("yelling");
            animationNames.Add("pick");

            foreach (string t in animationNames) {
// Load the animation file.
                string path = "Assets/Revision/Animation Clips/" + t +".anim" ;
                //Animation anim = AssetDatabase.LoadAssetAtPath(path, typeof(Animation)) as Animation;
                //Object[] anim = Resources.LoadAll(path);//, typeof(GameObject)) as GameObject;

                AnimationClip anim = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;

                if (anim == null) 
                    EditorUtility.DisplayDialog("Error!", "Could not load animation clip.\n\nAttempted path was: " + path , "Ok");                
                else
                    clips.Add(anim);
            }

           
            foreach (Transform group in crowd.transform) {
                foreach (Transform agent in group) {                    
                    AnimationUtility.SetAnimationClips(agent.GetComponent<Animation>(), clips.ToArray());
           
                }
            }
           
        }

        EditorGUILayout.BeginHorizontal();	
        if (GUILayout.Button("Enable Animation and Rendering", GUILayout.ExpandWidth(false))) {
            GameObject crowd = GameObject.Find("Crowd");
            if (crowd == null) {
                return;
            }

            foreach (Transform group in crowd.transform) {
                foreach (Transform agent in group) {
                    if (agent.GetComponentInChildren<AnimationSelector>())
                        agent.GetComponent<AnimationSelector>().enabled = true;
                    else
                        Debug.Log("AnimationSelector component not found!");

                    if (agent.GetComponentInChildren<PostureAnimator>())
                        agent.GetComponent<PostureAnimator>().enabled = true;
                    
                    if (agent.GetComponentInChildren<Animator>())
                        agent.GetComponent<Animator>().enabled = true;

                    foreach (Transform child in agent) {
                        foreach (SkinnedMeshRenderer sm in child.GetComponentsInChildren<SkinnedMeshRenderer>())
                            sm.enabled = true;

                        foreach (MeshRenderer mr in child.GetComponentsInChildren<MeshRenderer>())
                            mr.enabled = true;
                    }
                }
            }
        }
        if (GUILayout.Button("Disable Animation and Rendering", GUILayout.ExpandWidth(false))) {
            GameObject crowd = GameObject.Find("Crowd");
            if (crowd == null) {
                return;
            }

            foreach (Transform group in crowd.transform) {
                foreach (Transform agent in group) {
                    if (agent.GetComponentInChildren<AnimationSelector>())
                        agent.GetComponent<AnimationSelector>().enabled = false;
                    else
                        Debug.Log("AnimationSelector component not found!");
                    

                    if (agent.GetComponentInChildren<Animator>())
                        agent.GetComponent<Animator>().enabled = false;

                    if (agent.GetComponentInChildren<PostureAnimator>())
                        agent.GetComponent<PostureAnimator>().enabled = false;

                    foreach(Transform child in agent)
                        if(child.name.Equals("GUI Text"))
                            child.gameObject.SetActive(false);
                        

                   
                    foreach (Transform child in agent) {
                        foreach (SkinnedMeshRenderer sm in child.GetComponentsInChildren<SkinnedMeshRenderer>())
                            sm.enabled = false;

                        foreach (MeshRenderer mr in child.GetComponentsInChildren<MeshRenderer>())
                            mr.enabled = false;
                    }
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Assign random personality to crowd", GUILayout.ExpandWidth(false))) {
            GameObject crowd = GameObject.Find("Crowd");
            float[] persMean = { 0f, 0f, 0f, 0f, 0f };
            float[] persStd = { 0.35f, 0.35f, 0.35f, 0.35f, 0.35f };

            foreach (Transform group in crowd.transform) {
                group.gameObject.GetComponent<GroupBuilder>().AssignAgents();
                group.gameObject.GetComponent<GroupBuilder>().UpdatePersonalityAndBehavior(persMean, persStd);
            }

        }
        if (GUILayout.Button("Disable ezreplay", GUILayout.ExpandWidth(false))) {
            GameObject crowd = GameObject.Find("Crowd");
            if (crowd == null) {
                return;
            }

            //foreach (Transform group in crowd.transform) {
            //    foreach (Transform agent in group) {
            //        if (agent.GetComponentInChildren<AnimationSelector>())
            //            agent.GetComponent<Object2Record>().enabled = false;
            //    }
            //}
        }
        //if (GUILayout.Button("Enable ezreplay", GUILayout.ExpandWidth(false))) {
        //    GameObject crowd = GameObject.Find("Crowd");
        //    if (crowd == null) {
        //        return;
        //    }

        //    //foreach (Transform group in crowd.transform) {
        //    //    foreach (Transform agent in group) {
        //    //        if (agent.GetComponentInChildren<AnimationSelector>())
        //    //            agent.GetComponent<Object2Record>().enabled = true;
        //    //    }
        //    //}
        //}
       
        
        //As shortcuts to settings
        GUILayout.FlexibleSpace();
        string[] names = QualitySettings.names;
        GUILayout.BeginVertical();
        int i = 0;
        while (i < names.Length) {
            if (GUILayout.Button(names[i]))
                QualitySettings.SetQualityLevel(i, true);

            i++;
        }
        GUILayout.EndVertical();

        Time.timeScale = EditorGUILayout.FloatField("Time Scale", Time.timeScale, GUILayout.ExpandWidth(true));

        

        /*
        // TO BE DELETED
        if (GUILayout.Button("AddComp", GUILayout.ExpandWidth(false))) {
              GameObject crowd = GameObject.Find("Crowd");
            if (crowd == null) {
                return;
            }

            foreach (Transform group in crowd.transform) {
                foreach (Transform agent in group) {
                    DestroyImmediate(agent.gameObject.GetComponent<NavMeshObstacle>());
                    //agent.gameObject.AddComponent<NavMeshObstacle>();
                   // agent.gameObject.GetComponent<NavMeshObstacle>().radius = agent.gameObject.GetComponent<NavMeshAgent>().radius;
                }
            }
        }
        */
       	/*
        // TO BE DELETED
        if (GUILayout.Button("AddComp", GUILayout.ExpandWidth(false))) {
            foreach (GameObject ipad in GameObject.FindObjectsOfType(typeof(GameObject))) {
                if (ipad.name.Equals("Ipad")){// && ipad.transform.parent.Equals(GameObject.Find("Objects").transform)) {
                    if (ipad.transform.position.x >= -23 && ipad.transform.position.x <=-15)
                        ipad.transform.parent = GameObject.Find("Objects0").transform;
                    else if (ipad.transform.position.x >= -12 && ipad.transform.position.x < -6)
                        ipad.transform.parent = GameObject.Find("Objects1").transform;
                    else if (ipad.transform.position.x >= -2 && ipad.transform.position.x < 3)
                        ipad.transform.parent = GameObject.Find("Objects2").transform;
                    else if (ipad.transform.position.x >= 7&& ipad.transform.position.x < 13)
                        ipad.transform.parent = GameObject.Find("Objects3").transform;
                    else if (ipad.transform.position.x >= 15)
                        ipad.transform.parent = GameObject.Find("Objects4").transform;
                   
                    //   ipad.AddComponent("SphereCollider");
                    // ipad.collider.isTrigger = true;
                    ipad.GetComponent<SphereCollider>().radius= 2.5f;

                }
            }
        }
        */
      	
    }

    int ComputeNewGroupId() {
        int id;
        int maxId = 0;

        foreach (GameObject g in UnityEngine.Object.FindObjectsOfType(typeof(GameObject))) {
            if (g.name.Contains("CrowdGroup") && g.transform.parent.name.Equals("Crowd")) {                
                id = Int32.Parse(g.name.Substring(10));                
                if (id > maxId)
                    maxId = id;
            }
        }
        return maxId + 1;       
    }
	
	
}
