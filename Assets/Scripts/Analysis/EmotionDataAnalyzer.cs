/*
 * Attached to Crowd gameobject
 *
 * */

using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class EmotionDataAnalyzer : MonoBehaviour {

    public float[] Ekman = new[] {0f, 0f, 0f, 0f, 0f};
    public float[] OCC = new float[22];
    public Vector3 PAD;
    public int[] PADOctants = new int[8];
    private static int _callNum = 0;
    private void Start() {

        

        StreamWriter sw = new StreamWriter("emotionHistogram.txt");
        sw.Close();

   //     sw = new StreamWriter("padHistogram.txt");
    //    sw.Close();

        sw = new StreamWriter("padOctants.txt");
        sw.Close();

        //InvokeRepeating("ComputePADHistogram", 0, 2f);

        InvokeRepeating("ComputePADOctants", 0, 2f);

        InvokeRepeating("ComputeOCCHistogram", 0, 2f);

        //InvokeRepeating("WriteOCCWeights", 0, 2f);
        
    }

   
    private void ComputeEkmanHistogram() {
        AffectComponent[] affectComponents = FindObjectsOfType(typeof (AffectComponent)) as AffectComponent[];
        for (int i = 0; i < Ekman.Length; i++) {
            Ekman[i] = 0;
            foreach (AffectComponent ac in affectComponents)                
                Ekman[i] += ac.Ekman[i] / affectComponents.Length;
            

        }       
        WriteEkmanEmotions();
    }

    private void ComputeOCCHistogram() {
        int agentCnt = 0;
        AffectComponent[] affectComponents = FindObjectsOfType(typeof(AffectComponent)) as AffectComponent[];
        for (int i = 0; i < OCC.Length; i++) {
            OCC[i] = 0;
            agentCnt = 0;
            foreach (AffectComponent ac in affectComponents) {
                if (ac.GetComponent<PoliceBehavior>() != null)
                    continue;
                agentCnt++;
                OCC[i] += ac.Emotion[i];
            }
            OCC[i] /= agentCnt;
        }
           

        WriteOCCEmotions();
    }

    private void ComputePADHistogram() {
        AffectComponent[] affectComponents = FindObjectsOfType(typeof(AffectComponent)) as AffectComponent[];
        PAD = Vector3.zero;
            foreach (AffectComponent ac in affectComponents)
                PAD += ac.Mood / affectComponents.Length;

        
        WritePAD();
        _callNum++;
        if (_callNum > 60)
            Application.Quit();
    }


    private void ComputePADOctants() {
        AffectComponent[] affectComponents = FindObjectsOfType(typeof(AffectComponent)) as AffectComponent[];
        for (int i = 0; i < PADOctants.Length; i++)
            PADOctants[i] = 0;
        int agentCnt = 0;
        foreach (AffectComponent ac in affectComponents) {
            if (ac.GetComponent<PoliceBehavior>() != null)
                continue;
            agentCnt++;
                PADOctants[ac.GetCurrMoodOctant()]++;            
        }


        WritePADOCtants(agentCnt);
    }

    private void WriteEkmanEmotions() {
        using (FileStream fs = new FileStream("emotionHistogram.txt", FileMode.Append, FileAccess.Write)) {
            using (StreamWriter sw = new StreamWriter(fs)) {
                sw.WriteLine(Ekman[0] + "\t" + Ekman[1] + "\t" + Ekman[2] + "\t" + Ekman[3] + "\t" + Ekman[4] + "\t");
            }
        }
    }
    private void WriteOCCEmotions() {
        using (FileStream fs = new FileStream("emotionHistogram.txt", FileMode.Append, FileAccess.Write)) {
            using (StreamWriter sw = new StreamWriter(fs)) {
                foreach (float e in OCC)
                    sw.Write(e + "\t");
                sw.WriteLine();
            }
        }
    }
    private void WritePAD() {
        using (FileStream fs = new FileStream("padHistogram.txt", FileMode.Append, FileAccess.Write)) {
            using (StreamWriter sw = new StreamWriter(fs)) {                
                    sw.WriteLine(PAD.x  +"\t" + PAD.y  +"\t" + PAD.z);
            }
        }
    }

    private void WritePADOCtants(int length) {
        using (FileStream fs = new FileStream("padOctants.txt", FileMode.Append, FileAccess.Write)) {
            using (StreamWriter sw = new StreamWriter(fs)) {
                foreach(int p in PADOctants)
                    sw.Write((float)p/length + "\t");
                sw.WriteLine();
            }
        }
    }

     private void WriteOCCWeights() {
        AffectComponent[] affectComponents = FindObjectsOfType(typeof (AffectComponent)) as AffectComponent[];    

            using (StreamWriter sw = new StreamWriter("personalityToOCC.txt")) {

                foreach (AffectComponent ac in affectComponents) {
                    foreach (float p in ac.Personality)
                        sw.Write(p + "\t");
                    foreach (float ew in ac.EmotionWeight)
                        sw.Write(ew + "\t");
                    sw.WriteLine();
                }

            }
    }
}


