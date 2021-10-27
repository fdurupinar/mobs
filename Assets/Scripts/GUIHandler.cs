using UnityEngine;
using System.Collections;
using Debug = System.Diagnostics.Debug;

public class GUIHandler : MonoBehaviour {
	
	static public Vector3 PickedPoint;
    RaycastHit _hit;
    private bool _getScreenShot = false;

    public bool ContagionMode;

 
	void OnGUI () {

       
        GUIStyle style = new GUIStyle();
        GUILayout.BeginArea(new Rect(5, 10,150, 250));
        style.fontSize = 18;
        style.normal.textColor = Color.black;

	   //GUILayout.Label("Time: "+ Time.time + " s.");
        if (GUILayout.Button("Reset")) {
            GameObject.Find("Crowd").GetComponent<CrowdManager>().Restart();
            Application.LoadLevel(0);       
        }

	    if (GUILayout.Button("Agent")) {
            AgentComponent[] agentComponents = FindObjectsOfType(typeof(AgentComponent)) as AgentComponent[];
            Debug.Assert(agentComponents != null, "agentComponents != null");
            foreach (AgentComponent a in agentComponents) {
                if(a.enabled)
                    a.IndicatorAgent.SetActive(!a.IndicatorAgent.activeInHierarchy);
            }
        }
        if (GUILayout.Button("Dominant emotion")) {
            AgentComponent[] agentComponents = FindObjectsOfType(typeof(AgentComponent)) as AgentComponent[];
            Debug.Assert(agentComponents != null, "agentComponents != null");
            foreach (AgentComponent a in agentComponents) {
                if (a.enabled)
                    a.IndicatorCircle.SetActive(!a.IndicatorCircle.activeInHierarchy);

            }
        }
        if (GUILayout.Button("All emotions")) {
            AgentComponent[] agentComponents = FindObjectsOfType(typeof (AgentComponent)) as AgentComponent[];
            Debug.Assert(agentComponents != null, "agentComponents != null");
            foreach (AgentComponent a in agentComponents)
                if (a.enabled)
                    a.IndicatorParticle.SetActive(!a.IndicatorParticle.activeInHierarchy);
        }
	 /*   if (GUILayout.Button("Visibility")) {
            AgentComponent[] agentComponents = FindObjectsOfType(typeof(AgentComponent)) as AgentComponent[];
            Debug.Assert(agentComponents != null, "agentComponents != null");
            foreach (AgentComponent a in agentComponents) {
                if (a.VisibilityMesh == null)
                    a.VisibilityMesh = new MeshCreator("Visibility");
                else {
                    a.VisibilityMesh.ClearMeshes();
                    a.VisibilityMesh = null;
                }
            }            
       }*/
        /*
        if (GUILayout.Button("Save")) { //save to a video
            string videoFileName;
            int videoCnt = 0;
            do {
                videoCnt++;
                videoFileName = "VIDEOS\\video" + videoCnt + ".avi";

            } while (System.IO.File.Exists(videoFileName));
            EZReplayManager.get.saveToFile(videoFileName);
        }
        if (GUILayout.Button("Load")) {//open the last video
            string videoFileName;
            int videoCnt = 0;
            do {
                videoCnt++;
                videoFileName = "VIDEOS\\video" + videoCnt + ".avi";

            } while (System.IO.File.Exists(videoFileName));
            EZReplayManager.get.loadFromFile(videoFileName); 
        }
        */

        bool changed = GUI.changed;
	    ContagionMode = GUILayout.Toggle(ContagionMode, "Contagion");
        
        if(GUI.changed && changed == false) {
              AffectComponent[] affectComponents = FindObjectsOfType(typeof(AffectComponent)) as AffectComponent[];
            Debug.Assert(affectComponents != null, "affectComponents != null");
            foreach (AffectComponent a in affectComponents)
                a.ContagionMode = ContagionMode;            
        }



        _getScreenShot = GUILayout.Toggle(_getScreenShot, "Screenshot");
        GetComponent<Screenshot>().enabled = _getScreenShot;
        GUILayout.EndArea();
      
    }
        
	void Awake() {
        GetComponent<Screenshot>().enabled = _getScreenShot;
	}

    
	void Update() {
       
        if (Input.GetMouseButtonDown(0)) {
         /*   GameObject crowd = GameObject.Find("Crowd");
            //Picking an agent is a problem because of the large collider volume that it occupies
            //Shrink its collider temporarily
            foreach (Transform group in crowd.transform) {
                foreach (Transform agent in group) {
                    SphereCollider col = (SphereCollider) agent.collider;
                    col.radius = 1.2f;

                }
            }
            if (Camera.main != null) {
                
                //It selects the wrong agent most of the time
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out _hit);
                if (_hit.collider && _hit.collider.CompareTag("Player")) {
                    GameObject.Find("Arrow").GetComponent<ArrowBehavior>().FollowedAgent = _hit.collider.gameObject;
                    //Record that player
                    GameObject.Find("Arrow").GetComponent<ArrowBehavior>().RecordSignal = true;
                }

            }

            //Set collider back to original size
            foreach (Transform group in crowd.transform) {
                foreach (Transform agent in group) {
                    SphereCollider col = (SphereCollider)agent.collider;
                    col.radius = 4f;

                }
            }

          
            */

            if (Input.GetKey("e")) {
                if (Camera.main != null) {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Physics.Raycast(ray, out _hit, Mathf.Infinity);
                }

                PickedPoint = _hit.point; //gives the gameobject's position     
                Instantiate(Resources.Load("Explosion"), PickedPoint, new Quaternion());
            }


        }

	    if (Input.GetKey("s")) {
            _getScreenShot = !_getScreenShot;

            GetComponent<Screenshot>().enabled = _getScreenShot;
            
        }

        if (Input.GetKey("a")) {

            ScreenCapture.CaptureScreenshot("SCREENSHOTS\\screenshot" + Time.time + ".bmp");

        }

        
        // if(Input.GetKey("r")) {

        //     if(! GameObject.Find("Arrow").GetComponent<ArrowBehavior>().RecordSignal)
        //         GameObject.Find("Arrow").GetComponent<ArrowBehavior>().RecordSignal = true;
        // }

        if (Input.GetKey("1")) {
            Time.timeScale = 1;
            /*   GameObject cam1 = GameObject.Find("Camera1");
            GameObject cam2 = GameObject.Find("Camera2");
            if (cam1 != null) {
                cam1.camera.rect = new Rect(0, 0, 1, 1);
                cam1.GetComponent<Screenshot>().enabled = _getScreenShot;
            }
            if (cam2 != null) {
                cam2.camera.rect = new Rect(0, 0, 0, 0);
                cam2.GetComponent<Screenshot>().enabled = !_getScreenShot;
            }*/
        }
        else if (Input.GetKey("2")) {
            Time.timeScale = 2;
            /*GameObject cam1 = GameObject.Find("Camera1");
            GameObject cam2 = GameObject.Find("Camera2");
            if (cam1 != null) {
                cam1.camera.rect = new Rect(0, 0, 0, 0);
                cam1.GetComponent<Screenshot>().enabled = !_getScreenShot;
            }
            if (cam2 != null) {
                cam2.camera.rect = new Rect(0, 0, 1, 1);
                cam2.GetComponent<Screenshot>().enabled = _getScreenShot;
            }*/
        }
        else if (Input.GetKey("3")) {
            Time.timeScale = 3;
           /* GameObject cam1 = GameObject.Find("Camera1");
            GameObject cam2 = GameObject.Find("Camera2");
            if (cam1 != null) {
                cam1.camera.rect = new Rect(0, 0, 0.5f, 1);
                cam1.GetComponent<Screenshot>().enabled = _getScreenShot;
            }
            if (cam2 != null) {
                cam2.camera.rect = new Rect(0.5f, 0, 0.5f, 1);
                cam2.GetComponent<Screenshot>().enabled = !_getScreenShot;
            }*/
        }
           
        else if (Input.GetKey("4")) {
            Time.timeScale = 4;
        }
        else if (Input.GetKey("5")) {
            Time.timeScale = 0.5f;
        }
        else if (Input.GetKey("6")) {
            Time.timeScale = 0.25f;
        }
        else if (Input.GetKey("7")) {
            Time.timeScale = 0.1f;
        }

        
	}
    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(PickedPoint, PickedPoint+new Vector3(0,5,0));
    }
  
    
}

