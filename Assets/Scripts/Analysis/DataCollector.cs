/*
 * Attached to Crowd gameobject
 *
 * */

using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DataCollector : MonoBehaviour {

    private int[] _k = new[] { 10}; //
    private float[] _mean = new [] {0.1f};
    //private int[] _k = new [] {1, 10, 100}; //
    //private float[] _mean = new [] {0.01f, 0.1f, 1.0f};
   // private int[] _k = new [] {10}; //
   // private float[] _mean = new [] {1f};
    private List<float>[] _avgEmotion = new List<float>[1]; //combinations of k and mean + 0 contagion
    private List<float>[][] _emotion = new List<float>[1][]; //combinations of k and mean + 0 contagion for each agent
    int _agentCnt = 0;
    private bool _ended = false; //for turning recording on/off
    private int _maxFrameCnt =50;
    private int _kInd = 0;
    private int _meanInd = 0;
    private float _threshold = 0.5f;
    private float _decayCoef = 75f;
    private int _runInd {
        get {   
            return _meanInd * _k.Length + _kInd;
        }
    }
    /*
   private int _runInd {
       get {
           return _expInd * _doseThresholds.Length + _dtInd;
       }
   }
*/

    private int _simInd = 0;

    
    private float[] _doseThresholds = new[] {0f, 0.25f, 0.5f, 0.75f, 1f};
    private float[] _expThresholds = new[] { 0f, 0.25f, 0.5f, 0.75f, 1f };

    private int _dtInd = 0;
    private int _expInd = 0;
   

    // Use this for initialization
	void Start () {
        _agentCnt = 0;
        foreach (Transform group in transform) {                
            foreach (Transform agent in group) {
                if(agent.gameObject.activeInHierarchy)
                    _agentCnt++;
            }
        }

        for(int i  = 0; i < _avgEmotion.Length; i++)
            _avgEmotion[i] = new List<float>();

        for (int i = 0; i < _emotion.Length; i++) {
            _emotion[i] = new List<float> [_agentCnt];
            for(int  j = 0; j < _agentCnt; j++) {
                _emotion[i][j] = new List <float>();
            }
        }
 
        //Add fear randomly to one agent

        //bool breaked = false;
        //foreach (Transform group in transform) {
        //    foreach (Transform agent in group) {
        //        if (agent.gameObject.activeInHierarchy) {
       
        //            agent.GetComponent<AffectComponent>().Emotion[(int) EType.Fear] = 0.75f;
        //            breaked = true;
        //            break;
        //        }                
        //    }
        //    if (breaked) break;
        //}

   
        int id = 0;
        foreach (Transform group in transform) {
            foreach (Transform agent in group) {
                if (agent.gameObject.activeInHierarchy) {
                    if (id == 0) 
                        agent.GetComponent<AffectComponent>().Emotion[(int) EType.Fear] = 0.1f;                        
                    else if (id == 1) 
                        agent.GetComponent<AffectComponent>().Emotion[(int)EType.Fear] = 0.4f;
                    else if (id == 2)
                        agent.GetComponent<AffectComponent>().Emotion[(int)EType.Fear] = 0.9f;
                    id++;
                }
            }
         
        }

        AssignValues((int)EType.Fear, _kInd, _meanInd);
        AssignDecayCoef(_decayCoef);
         AssignThresholds(_threshold, _threshold, (int)EType.Anger);

	    //AssignValues((int)EType.Anger, 0,0);
   //      RecordAvgThresholds("AvgThresholds.txt", (int)EType.Anger);


	   // RecordPersonalities("Personalities,txt", (int) OCEAN.O);
         
        
         



      //  AssignThresholds(0.5f, 0.5f, (int)EType.Anger);     
    //     AssignEmotions((int)EType.Anger, 0.2f, 0.9f); // 20% of agents are 0.9 angry
        // AssignValues((int)EType.Anger, 0, 0);


//         AssignEmotions((int)EType.Fear, 0.2f, 0.75f); // 20% of agents are 0.9 angry
        

         

/*
        AssignEmotionsManually(0, (int)EType.Anger, 0.1f);
        AssignEmotionsManually(1, (int)EType.Anger, 0.9f);
        AssignEmotionsManually(2, (int)EType.Anger, 0.3f);
  */    //  AssignEmotionsManually(3, (int)EType.Anger, 0.3f);
      //  AssignEmotionsManually(4, (int)EType.Anger, 0.5f); 


	}

    // Update for ascribe
    void FixedUpdate() {
        if (_ended) {
            Application.Quit();
            return;
        }



        if (Time.fixedTime == Mathf.Floor(Time.fixedTime)) {
            ComputeAvgEmotion((int) EType.Fear, _runInd);
            ComputeEmotionAtTime((int) EType.Fear, _runInd);


        }

            
            
        if (_emotion[_runInd][0].Count == _maxFrameCnt) {
            Debug.Log("Recorded");
            RecordEmotionForAll("avgFear.txt"); //record

        }
    }




    //Update for thresholds
    /*
    void FixedUpdate() {
        if (_ended) {
            Application.Quit();
            return;
        }

        AssignValues((int)EType.Anger, _dtInd, _expInd);

        if (Time.fixedTime == Mathf.Floor(Time.fixedTime)) {
            ComputeAvgEmotion((int)EType.Anger, _runInd);
            ComputeEmotionAtTime((int)EType.Anger, _runInd);


        }
        

        //if (_avgEmotion[_runInd].Count == _maxFrameCnt) {
        if (_emotion[_runInd][0].Count == _maxFrameCnt) {
            Debug.Log("dt: " + _dtInd + " exp: " + _expInd);
            //     if (_runInd == _k.Length * _mean.Length) { //including all 0

            if (_runInd == _doseThresholds.Length * _expThresholds.Length - 1) { //including all 0
                //RecordEmotionForAll("angerAll.txt"); //record
                RecordAvgEmotion("avgAnger.txt"); //record
                Debug.Log("Recorded");


                _ended = true;
                return;
            }

            RecordAvgEmotion("avgAnger" + _dtInd + " " + _expInd + ".txt"); //record
            _dtInd++;
            if (_dtInd == _doseThresholds.Length) {
                _expInd++;
                _dtInd = 0;
            }


            GetComponent<CrowdManager>().Restart();
            //     if(_runInd == _k.Length * _mean.Length) //last run for 0 contagion
            //        AssignZero((int)EType.Anger);
            //    else
            
            AssignEmotions((int)EType.Anger, 0.2f, 0.9f); // 20% of agents are 0.9 angry
        }
    }
     */
    //Update for k and mean
    /*
	// Update is called once per frame
	void FixedUpdate () {
        if (_ended) {
            Application.Quit();
            return;
        }


	    
        if (Time.fixedTime == Mathf.Floor(Time.fixedTime)) {
            ComputeAvgEmotion((int)EType.Anger, _runInd);
            ComputeEmotionAtTime((int)EType.Anger, _runInd);
            

        }
            //ComputeHighEmotionPercentage((int)EType.Anger, runInd);

        //if (_avgEmotion[_runInd].Count == _maxFrameCnt) {
        if (_emotion[_runInd][0].Count == _maxFrameCnt) {
            Debug.Log("k: " + _kInd + " mean: " + _meanInd);
        //     if (_runInd == _k.Length * _mean.Length) { //including all 0
            
            if (_runInd == _k.Length * _mean.Length -1) { //including all 0
                //RecordEmotionForAll("angerAll.txt"); //record
                RecordAvgEmotion("avgAnger.txt"); //record
                Debug.Log("Recorded");
                Application.Quit();
                _kInd = 0;
                _meanInd = 0;
                GetComponent<CrowdManager>().Restart();
                //     if(_runInd == _k.Length * _mean.Length) //last run for 0 contagion
                //        AssignZero((int)EType.Anger);
                //    else
                AssignValues((int)EType.Anger, _kInd, _meanInd);
                AssignEmotions((int)EType.Anger, 0.2f, 0.9f); // 20% of agents are 0.9 angry
                _simInd++;        
                
                return;
            }
            _kInd++;
            if (_kInd == _k.Length) {
                _meanInd++;
                _kInd = 0;                        
            }
                            
            GetComponent<CrowdManager>().Restart(); 
       //     if(_runInd == _k.Length * _mean.Length) //last run for 0 contagion
        //        AssignZero((int)EType.Anger);
        //    else
                AssignValues((int)EType.Anger, _kInd, _meanInd);
                AssignEmotions((int)EType.Anger, 0.2f, 0.9f); // 20% of agents are 0.9 angry
        }


	    

        if(_simInd > 10) {
       
            _ended = true;
            Application.Quit();
            return;        
        }

	}*/
   
    void AssignZero(int eType) {
        foreach (Transform group in transform) {
            foreach (Transform agent in group) {
                if (agent.gameObject.activeInHierarchy) {
                    agent.GetComponent<AffectComponent>().LambdaE[eType].DoseMean = 0f;
                }
            }
        }
    }

    void AssignDecayCoef(float decayCoef) {
        foreach (Transform group in transform) {
            foreach (Transform agent in group) {
                if (agent.gameObject.activeInHierarchy) {
                    agent.GetComponent<AffectComponent>().DecayCoef = decayCoef;

                }
            }
        }
    }

    void AssignValues(int eType, int kInd, int meanInd) {
        foreach (Transform group in transform) {
            foreach (Transform agent in group) {
                if (agent.gameObject.activeInHierarchy) {
                    agent.GetComponent<AffectComponent>().LambdaE[eType].K = _k[kInd];
                    agent.GetComponent<AffectComponent>().LambdaE[eType].DoseMean = _mean[meanInd];
                }
            }
        }
    }
    /*
    //Assign thresholds
    void AssignValues(int eType, int dtInd, int expInd) {
        foreach (Transform group in transform) {
            foreach (Transform agent in group) {
                if (agent.gameObject.activeInHierarchy) {
                    agent.GetComponent<AffectComponent>().LambdaE[eType].DoseThreshold = _doseThresholds[dtInd];
                    agent.GetComponent<AffectComponent>().ExpressibilityThreshold = _expThresholds[expInd];
                }
            }
        }
    }*/
    //expressibility and susceptibility thresholds
    void AssignThresholds(float susT, float expT, int eType) {
        foreach (Transform group in transform) {
            foreach (Transform agent in group) {
                if (agent.gameObject.activeInHierarchy) {
                    agent.GetComponent<AffectComponent>().LambdaE[eType].DoseThreshold = susT;
                    agent.GetComponent<AffectComponent>().ExpressibilityThreshold = expT;
                }
            }
        }
    }

    
    void AssignEmotionsManually(int id, int eType, float eVal) {
        foreach (Transform group in transform) {
            foreach (Transform agent in group) {
                if (agent.gameObject.activeInHierarchy) {
                    if (agent.GetComponent<AgentComponent>().Id == id)
                        agent.GetComponent<AffectComponent>().Emotion[eType] = eVal;
                }
            }
        }

    }

    //change this in the code 
    void AssignEmotions(int eType, float ratio, float eVal) {
        int agentInd = 0;
        foreach (Transform group in transform) {
            foreach (Transform agent in group) {
                if (agent.gameObject.activeInHierarchy) {
                    //if (agentInd % 200 == 0)                    
                    if ((int)(ratio * _agentCnt) == 0 || agentInd % (int)(ratio * _agentCnt) == 0)
                        agent.GetComponent<AffectComponent>().Emotion[eType] = eVal;//0.9f;
                    else
                        agent.GetComponent<AffectComponent>().Emotion[eType] = 1 - eVal; // 0.1f;


                    agentInd++;
                }
            }
        }

    }

    void ComputeAvgEmotion(int eType, int runInd)  {
        int ind = _avgEmotion[runInd].Count;
        _avgEmotion[runInd].Add(0);        
        foreach (Transform group in transform) {
                foreach (Transform agent in group) {
                    if (agent.gameObject.activeInHierarchy) {
                        _avgEmotion[runInd][ind] += agent.GetComponent<AffectComponent>().Emotion[eType];
                    }
	        }
            
        }
        _avgEmotion[runInd][ind] /= _agentCnt;
    }

    
    void ComputeHighEmotionPercentage(int eType, int runInd) {
        int ind = _avgEmotion[runInd].Count;
        _avgEmotion[runInd].Add(0);
        foreach (Transform group in transform) {
            foreach (Transform agent in group) {
                if (agent.gameObject.activeInHierarchy) {
                    if (agent.GetComponent<AffectComponent>().Emotion[eType] > 0.5f)
                        _avgEmotion[runInd][ind] += 0.01f;
                }
            }

        }        
    }

    //Compute Emotion for each agent
    void ComputeEmotionAtTime(int eType, int runInd) {               
        int agentId = 0;
        foreach (Transform group in transform) {
            foreach (Transform agent in group) {
                if (!agent.gameObject.activeInHierarchy) continue;
                int ind = _emotion[runInd][agentId].Count;
                _emotion[runInd][agentId].Add(0);
                _emotion[runInd][agentId][ind] = agent.GetComponent<AffectComponent>().Emotion[eType];
                agentId++;
            }
        }     
    }


    void RecordPersonalities(string fileName, int pType) {
        StreamWriter sw = new StreamWriter(fileName);
        //float dt = 0, et = 0;
        foreach (Transform group in transform) {
            foreach (Transform agent in group) {
                if (!agent.gameObject.activeInHierarchy) continue;
                sw.WriteLine(agent.GetComponent<AffectComponent>().Personality[pType]);
            }
        }
        
        sw.Close();

    }

    void RecordAvgThresholds(string fileName, int eType) {
        StreamWriter sw = new StreamWriter(fileName);
        float dt = 0, et = 0;
        foreach (Transform group in transform) {
            foreach (Transform agent in group) {
                if (!agent.gameObject.activeInHierarchy) continue;
                dt += agent.GetComponent<AffectComponent>().LambdaE[eType].DoseThreshold;
                et += agent.GetComponent<AffectComponent>().ExpressibilityThreshold;
            }
        }

        dt /= _agentCnt;
        et /= _agentCnt;
        sw.WriteLine(dt);
        sw.WriteLine(et);
        sw.Close();

    }


    void RecordEmotionForAll(string fileName) {
        StreamWriter sw = new StreamWriter(fileName);
        //for (int i = 0; i < _emotion.Length - 1; i++) {  //for zero

        for (int i = 0; i < _emotion.Length; i++) {
            for (int j = 0; j < _emotion[i].Length; j++) {
          //      sw.Write("K: " + _k[i % 3] + " Mean: " + _mean[i / 3] + " Agent id: " + j + "\t");
            //    sw.Write("T = " + _threshold + " Agent id: " + j);
                if (i < _emotion.Length - 1 || j < _emotion[i].Length - 1)
                    sw.Write("\t");
            }
        }

        sw.WriteLine();

        for (int k = 0; k < _emotion[0][0].Count; k++) {
            for (int i = 0; i < _emotion.Length; i++) {
                for (int j = 0; j < _emotion[i].Length; j++) {                
                    sw.Write(_emotion[i][j][k]);
                    if(j < _emotion[i].Length - 1)
                        sw.Write("\t");
                        
                }

            }
            sw.WriteLine();
        }
        //last run for contagion = 0
        /*sw.Write("K: 0" + " Mean: 0\t");
        for (int j = 0; j < _emotion[_emotion.Length - 1].Length ; j++) {
            sw.WriteLine("Agent id " + j);
            for (int k = 0; k < _emotion[_emotion.Length - 1][j].Count; k++) {
                sw.Write(_emotion[_emotion.Length - 1][j][k] + "\t");
            }
            sw.WriteLine();
        }
        */
        sw.Close();
    }




    void RecordAvgEmotion(string fileName) {
        
        StreamWriter sw = new StreamWriter(fileName);
        for (int i = 0; i < _avgEmotion.Length; i++ ) {
            sw.Write("K: " + _k[i % 3] + " Mean: " + _mean[i / 3] + "\t");
            for (int j = 0; j < _avgEmotion[i].Count; j++) {
                sw.Write(_avgEmotion[i][j] + "\t");
            }
            sw.WriteLine();
        }
        /*
        //last run for contagion = 0
        sw.Write("K: 0" + " Mean: 0\t");
        for (int j = 0; j < _avgEmotion[_avgEmotion.Length-1].Count; j++) {
            sw.Write(_avgEmotion[_avgEmotion.Length-1][j] + "\t");
        }
        */
        sw.Close();
        
    }
    /*
   void RecordAvgEmotion(string fileName) {

       StreamWriter sw = new StreamWriter(fileName);
       for (int i = 0; i < _avgEmotion.Length - 1; i++) {
           sw.Write("dt: " + _doseThresholds[i%5] + " Exp: " + _expThresholds[i / 5] + "\t");
           for (int j = 0; j < _avgEmotion[i].Count; j++) {
               sw.Write(_avgEmotion[i][j] + "\t");
           }
           sw.WriteLine();
       }
       //last run for contagion = 0
       sw.Write("dt: 1" + " exp: 1\t");
       for (int j = 0; j < _avgEmotion[_avgEmotion.Length - 1].Count; j++) {
           sw.Write(_avgEmotion[_avgEmotion.Length - 1][j] + "\t");
       }

       sw.Close();

   } 
    */
}

