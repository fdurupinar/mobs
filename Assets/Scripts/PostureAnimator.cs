using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PostureAnimator : MonoBehaviour {
    //public List<Transform> BodyChain;
    public Quaternion[][] BodyAngle  = new Quaternion[5][];

    
    
    public Transform Spine;
    public Transform Spine1;    
    public Transform Neck;
    public Transform ShoulderL;
    public Transform ShoulderR;

    private enum BPart {
        Spine,
        Spine1,
        Neck,
        ShoulderL,
        ShoulderR
    };

	// Use this for initialization
	void Awake () {


        for (int i = 0; i < BodyAngle.Length; i++)
            BodyAngle[i] = new Quaternion[5];

	    Restart();
	}

    public void Restart() {
        
        //Attach bones automatically
        foreach (Transform child in transform) {
            AssignBones(child);
        }
        

        ReadAngles(); //Assign initial angles just once
    }

    void AssignBones(Transform child) {

        if (child.name.Contains("Spine1") || child.name.Contains("spine1") || child.name.Contains("chest"))
            Spine1 = child.transform;
        else if ((child.name.Contains("Spine") || child.name.Contains("spine") || child.name.Contains("abdomen")) &&
                 !child.name.Contains("1") && !child.name.Contains("2") && !child.name.Contains("3") &&
                 !child.name.Contains("4"))
            Spine = child.transform;
        else if (child.name.Contains("Neck") || child.name.Contains("neck"))
            Neck = child.transform;
        else if (child.name.Contains("RightShoulder") || child.name.Contains("rCollar"))
            ShoulderR = child.transform;
        else if (child.name.Contains("LeftShoulder") || child.name.Contains("lCollar"))
            ShoulderL = child.transform;



        foreach (Transform gc in child) {
            AssignBones(gc);
        }
    }

    public void ReadAngles() {
        for(int i = 0; i < 5; i++) {
            string[] content = File.ReadAllLines(GetComponent<AffectComponent>().EkmanStr[i] + "Posture.txt");
            for (int j = 0; j < content.Length; j++) {
                string[] tokens = content[j].Split('\t');
                BodyAngle[i][j] = new Quaternion(float.Parse(tokens[0]), float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]));
            }

        }

    }
	// Update is called once per frame
    void LateUpdate() {
        if (GetComponent<AgentComponent>().IsDead()) 
            return;

        int expressionInd = GetComponent<AffectComponent>().GetExpressionInd();
        float expressionVal = GetComponent<AffectComponent>().GetExpressionValue();
        Quaternion rot = new Quaternion();

        rot = Quaternion.Slerp(BodyAngle[(int)EkmanType.Neutral][(int)BPart.Spine], BodyAngle[expressionInd][(int)BPart.Spine], expressionVal); //weighted posture of expression
        Spine.localRotation = Quaternion.Slerp(Spine.localRotation, rot, 0.5f); //gradual posture change

        rot = Quaternion.Slerp(BodyAngle[(int)EkmanType.Neutral][(int)BPart.Spine1], BodyAngle[expressionInd][(int)BPart.Spine1], expressionVal); //weighted posture of expression
        Spine1.localRotation = Quaternion.Slerp(Spine.localRotation, rot, 0.5f); //gradual posture change

        rot = Quaternion.Slerp(BodyAngle[(int)EkmanType.Neutral][(int)BPart.Neck], BodyAngle[expressionInd][(int)BPart.Neck], expressionVal); //weighted posture of expression
        Neck.localRotation = Quaternion.Slerp(Neck.localRotation, rot, 0.5f); //gradual posture change

        rot = Quaternion.Slerp(BodyAngle[(int)EkmanType.Neutral][(int)BPart.ShoulderL], BodyAngle[expressionInd][(int)BPart.ShoulderL], expressionVal); //weighted posture of expression
        ShoulderL.localRotation = Quaternion.Slerp(ShoulderL.localRotation, rot, 0.5f); //gradual posture change

        rot = Quaternion.Slerp(BodyAngle[(int)EkmanType.Neutral][(int)BPart.ShoulderR], BodyAngle[expressionInd][(int)BPart.ShoulderR], expressionVal); //weighted posture of expression
        ShoulderR.localRotation = Quaternion.Slerp(ShoulderR.localRotation, rot, 0.5f); //gradual posture change
       

    }

    

}
