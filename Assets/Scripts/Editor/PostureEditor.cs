using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof (PostureAnimator))]
public class PostureEditor : Editor {

    private int _expression;
    private string[] _expressionNames = new string[5];
    private int[] _expressionInds = {0, 1, 2, 3,4};

    private PostureAnimator _postureAnimator;

    private void OnEnable() {


        _postureAnimator = target as PostureAnimator;

        for (int i = 0; i < _postureAnimator.BodyAngle.Length; i++)
            _postureAnimator.BodyAngle[i] = new Quaternion[5];

    //    _postureAnimator.ReadAngles();

        _postureAnimator.Restart();
        for (int i = 0; i < 5; i++)
            _expressionNames[i] = _postureAnimator.GetComponent<AffectComponent>().EkmanStr[i];

    }

    
    public override void OnInspectorGUI() {
        GUILayout.Label(_postureAnimator.Spine.name);
        GUILayout.Label(_postureAnimator.Spine1.name);        
        GUILayout.Label(_postureAnimator.Neck.name);
        GUILayout.Label(_postureAnimator.ShoulderL.name);
        GUILayout.Label(_postureAnimator.ShoulderR.name);


        _expression = EditorGUILayout.IntPopup("Expression: ", _expression, _expressionNames, _expressionInds);
        //WriteToFile
        if (GUILayout.Button("RecordPosture", GUILayout.ExpandWidth(false))) {
            StreamWriter sw = new StreamWriter(_expressionNames[_expression] + "Posture.txt");

            sw.WriteLine(_postureAnimator.Spine.localRotation.x + "\t" + _postureAnimator.Spine.localRotation.y + "\t" + _postureAnimator.Spine.localRotation.z + "\t" + _postureAnimator.Spine.localRotation.w);
            sw.WriteLine(_postureAnimator.Spine1.localRotation.x + "\t" + _postureAnimator.Spine1.localRotation.y + "\t" + _postureAnimator.Spine1.localRotation.z + "\t" + _postureAnimator.Spine1.localRotation.w);
            sw.WriteLine(_postureAnimator.Neck.localRotation.x + "\t" + _postureAnimator.Neck.localRotation.y + "\t" + _postureAnimator.Neck.localRotation.z + "\t" + _postureAnimator.Neck.localRotation.w);
            sw.WriteLine(_postureAnimator.ShoulderL.localRotation.x + "\t" + _postureAnimator.ShoulderL.localRotation.y + "\t" + _postureAnimator.ShoulderL.localRotation.z + "\t" + _postureAnimator.ShoulderL.localRotation.w);
            sw.WriteLine(_postureAnimator.ShoulderR.localRotation.x + "\t" + _postureAnimator.ShoulderR.localRotation.y + "\t" + _postureAnimator.ShoulderR.localRotation.z + "\t" + _postureAnimator.ShoulderR.localRotation.w);
            /*
            foreach (Transform t in _postureAnimator.BodyChain) {
                sw.WriteLine(t.localRotation.eulerAngles.x + "\t" + t.localRotation.eulerAngles.y + "\t" +
                             t.localRotation.eulerAngles.z);
             * 
            }*/
            sw.Close();
        }

        if (GUILayout.Button("LoadPosture", GUILayout.ExpandWidth(false))) {
            string[] content = File.ReadAllLines(_expressionNames[_expression] + "Posture.txt");
            
                string[] tokens = content[0].Split('\t');
                _postureAnimator.Spine.localRotation = new Quaternion(float.Parse(tokens[0]), float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]));
                tokens = content[1].Split('\t');
                _postureAnimator.Spine1.localRotation = new Quaternion(float.Parse(tokens[0]), float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]));
                tokens = content[2].Split('\t');
                _postureAnimator.Neck.localRotation = new Quaternion(float.Parse(tokens[0]), float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]));
                tokens = content[3].Split('\t');
                _postureAnimator.ShoulderL.localRotation = new Quaternion(float.Parse(tokens[0]), float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]));
                tokens = content[4].Split('\t');
                _postureAnimator.ShoulderR.localRotation = new Quaternion(float.Parse(tokens[0]), float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]));

           /* for (int j = 0; j < content.Length; j++) {
                string[] tokens = content[j].Split('\t');
                _postureAnimator.BodyChain[j].eulerAngles = new Vector3(float.Parse(tokens[0]), float.Parse(tokens[1]),
                                                                        float.Parse(tokens[2]));
            }
            */
        }
    }
}
