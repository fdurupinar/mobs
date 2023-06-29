using UnityEngine;
public class HealthBar : MonoBehaviour {
    //RectTransform is the 2D counterpart of Transform
    private RectTransform ThisTransform = null;

    
    public AgentComponent Agent;
    

    //Catch up speed
    public float MaxSpeed = 10f;
    void Awake() {
        //Get transform component
        ThisTransform = GetComponent<RectTransform>();        
    }


    void Start() {
        //Set Start Health
        if(Agent != null)
            //SizeDelta is the size of this RectTransform relative to the distances between anchors. Same as size if anchors are together
            ThisTransform.sizeDelta = new Vector2(Mathf.Clamp((1- Agent.GetDamage()) *100, 0, 100), ThisTransform.sizeDelta.y);
    }
    void Update() {
        //Update health property
        float HealthUpdate = 0f;
        if(Agent != null)
            //If damage is big, MoveTowards makes sure that health appears to decrease gradually
            HealthUpdate = Mathf.MoveTowards(ThisTransform.rect.width, (1- Agent.GetDamage())*100, MaxSpeed);
        ThisTransform.sizeDelta = new Vector2(Mathf.Clamp(HealthUpdate, 0, 100), ThisTransform.sizeDelta.y);
    }
}