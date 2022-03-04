using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emotion : MonoBehaviour
{
    Mesh thisMesh;
    SkinnedMeshRenderer smr;

    float[] anger;
    float[] sadness;
    float[] disgust;
    float[] joy;
    float[] fear;
    float[] surprise;

    void Start()
    {
        smr = this.GetComponent<SkinnedMeshRenderer>();
        thisMesh = smr.sharedMesh;

        anger = new float[thisMesh.blendShapeCount];
        sadness = new float[thisMesh.blendShapeCount];
        disgust = new float[thisMesh.blendShapeCount];
        joy = new float[thisMesh.blendShapeCount];
        fear = new float[thisMesh.blendShapeCount];
        surprise = new float[thisMesh.blendShapeCount];

        anger[thisMesh.GetBlendShapeIndex("Anger")] = 100;
        joy[thisMesh.GetBlendShapeIndex("Joy")] = 100;
        sadness[thisMesh.GetBlendShapeIndex("Sadness")] = 100;
        disgust[thisMesh.GetBlendShapeIndex("Disgust")] = 100;
        fear[thisMesh.GetBlendShapeIndex("Fear")] = 100;
        surprise[thisMesh.GetBlendShapeIndex("Surprise")] = 100;

        SetDefaulte();
    }

    void LateUpdate()
    {
        
    }

    //https://www.youtube.com/watch?v=Jj2czsz3s9Y&ab_channel=incern
    void BlendEmotion() {

    }

    void SetDefaulte()
    {
        for (int i = 0; i < thisMesh.blendShapeCount; i++) {
            smr.SetBlendShapeWeight(i, 0);
        }
    }

    void SetAnger() {
        for (int i = 0; i < thisMesh.blendShapeCount; i++)
        {
            smr.SetBlendShapeWeight(i, anger[i]);
        }
    }

    void SetJoy()
    {
        for (int i = 0; i < thisMesh.blendShapeCount; i++)
        {
            smr.SetBlendShapeWeight(i, joy[i]);
        }
    }

    void SetSadness()
    {
        for (int i = 0; i < thisMesh.blendShapeCount; i++)
        {
            smr.SetBlendShapeWeight(i, sadness[i]);
        }
    }

    void SetDisgust()
    {
        for (int i = 0; i < thisMesh.blendShapeCount; i++)
        {
            smr.SetBlendShapeWeight(i, disgust[i]);
        }
    }

    void SetFear()
    {
        for (int i = 0; i < thisMesh.blendShapeCount; i++)
        {
            smr.SetBlendShapeWeight(i, fear[i]);
        }
    }

    void SetSurprise()
    {
        for (int i = 0; i < thisMesh.blendShapeCount; i++)
        {
            smr.SetBlendShapeWeight(i, surprise[i]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            SetDefaulte();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetAnger();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetJoy();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetSadness();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetDisgust();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetFear();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SetSurprise();
        }
    }
}
