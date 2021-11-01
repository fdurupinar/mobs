using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanShoppingBehavior : MonoBehaviour
{
    public GameObject CurrentObjs;
    public Transform _rightHand, _leftHand;
    
    Animator _animator;

    HumanComponent _humanComponent;
    List<Collider> _ipadColliders;

    Transform _desiredObj;
    // Start is called before the first frame update
    void Start()
    {
        CurrentObjs = new GameObject("AchievedObjects");
        CurrentObjs.transform.parent = this.transform;
        _animator = GetComponent<Animator>();

        _ipadColliders = new List<Collider>();
        _humanComponent = GetComponent<HumanComponent>();
 
        if (!_leftHand) {

            _leftHand = transform.Find("mixamorig7:Hips/mixamorig7:Spine/mixamorig7:Spine1/mixamorig7:Spine2/mixamorig7:LeftShoulder/mixamorig7:LeftArm/mixamorig7:LeftForeArm/mixamorig7:LeftHand/mixamorig7:LeftHandIndex1");
            _rightHand = transform.Find("mixamorig7:Hips/mixamorig7:Spine/mixamorig7:Spine1/mixamorig7:Spine2/mixamorig7:RightShoulder/mixamorig7:RightArm/mixamorig7:RightForeArm/mixamorig7:RightHand/mixamorig7:RightHandIndex1");

        }
    }



    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.X)) {
            float minDist = 10000;
            foreach (Collider col in _ipadColliders) {
				if (!col.GetComponent<ObjComponent>().Achieved) { 
                float dist = Vector3.Distance(col.transform.position, transform.position);
                    if (dist < minDist) {
                        minDist = dist;
                        _desiredObj = col.transform;
                    }

                }
            }
        }

        //human is inside the object's trigger zone
        if (_desiredObj) {


			if (_humanComponent.StartedWaiting == false) {

				GetComponent<Animator>().SetTrigger("PickUp");
                

                //_humanComponent.HandPos = _desiredObj.position;

                _humanComponent.StartedWaiting = true;
				_humanComponent.FinishedWaiting = false;

                
                

			}
            else if (_humanComponent.StartedWaiting && !_humanComponent.FinishedWaiting && _desiredObj.GetComponent<ObjComponent>().Achieved == false) { //started waiting
				if (_desiredObj.GetComponent<ObjComponent>().ClosestAgent.Equals(this.gameObject)) {
					_desiredObj.position = _rightHand.position;
				}

				//_humanComponent.HandPos = _desiredObj.position;//+ Vector3.up * 0.1f;
                

			}

			if (_humanComponent.FinishedWaiting) {
                

                if (_desiredObj.GetComponent<ObjComponent>().Achieved == false) { //make sure someone else didn't pick it before me                
                                          
					_desiredObj.parent = CurrentObjs.transform;
					_desiredObj.position = CurrentObjs.transform.position - 0.05f * Vector3.up + (CurrentObjs.transform.childCount - 1) * 0.05f * Vector3.up;
					
					_desiredObj.GetComponent<ObjComponent>().AchievingAgent = this.gameObject;
					_desiredObj.GetComponent<ObjComponent>().Achieved = true;



                    _ipadColliders.Remove(_desiredObj.GetComponent<Collider>());
                }


				_humanComponent.StartedWaiting = false;

				_desiredObj = null;
			}

		}

	}



    //Not working properly for human
	//void OnAnimatorIK(int layerIndex) {

	//	if (_animator.GetCurrentAnimatorStateInfo(3).IsName("pickingUp")) {
	//		float reach = _animator.GetFloat("RightHandReach");  //param is a curve set in pickingUp animation           
	//		_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, reach);
	//		_animator.SetIKPosition(AvatarIKGoal.RightHand, GetComponent<HumanComponent>().HandPos);
            

 //       }

  

	//}

	//Called as an animation event when the picking up animation ends
	void PickedObject() {
        _humanComponent.FinishedWaiting = true;
     
    }

	private void OnTriggerEnter(Collider other) {
        //Assign the closest ipad
        if (other.CompareTag("Ipad") ){
            _ipadColliders.Add(other);
        }

    }
    private void OnTriggerExit(Collider other) {
        //Assign the closest ipad
        if (_ipadColliders.Contains(other)) {
            _ipadColliders.Remove(other);
        }
    }


    //private void OnTriggerStay(Collider other) {

        
        //if (other.CompareTag("Ipad") && Input.GetKey(KeyCode.X)) {

        //    Debug.Log(other);
        //    ObjComponent objComp = other.GetComponent<ObjComponent>();
            
        //    if (!objComp.Achieved) {

        //        _desiredObj = other.transform;
                

        //    }             
            
        //}

    //}

	//private void OnTriggerExit(Collider other) {
 //       if(_desiredObj == other.transform && _desiredObj.GetComponent<ObjComponent>().Achieved)
	//	    _desiredObj = null;

	//}

	private void LateUpdate() {
        CurrentObjs.transform.position = _leftHand.position;
    }

}
