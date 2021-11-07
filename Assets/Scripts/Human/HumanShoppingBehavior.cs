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
        //_animator = transform.Find("FpsArm2").GetComponent<Animator>();
        _animator = GetComponent<Animator>();


        _ipadColliders = new List<Collider>();
        _humanComponent = GetComponent<HumanComponent>();
 
        if (!_leftHand) {

            _leftHand = transform.Find("mixamorig1:Hips/mixamorig1:Spine/mixamorig1:Spine1/mixamorig1:Spine2/mixamorig1:LeftShoulder/mixamorig1:LeftArm/mixamorig1:LeftForeArm/mixamorig1:LeftHand/mixamorig1:LeftHandIndex1");
            _rightHand = transform.Find("mixamorig1:Hips/mixamorig1:Spine/mixamorig1:Spine1/mixamorig1:Spine2/mixamorig1:RightShoulder/mixamorig1:RightArm/mixamorig1:RightForeArm/mixamorig1:RightHand/mixamorig1:RightHandIndex1");

        }

        if (!_leftHand) {
            _leftHand = transform.Find("mixamorig8:Hips/mixamorig8:Spine/mixamorig8:Spine1/mixamorig8:Spine2/mixamorig8:LeftShoulder/mixamorig8:LeftArm/mixamorig8:LeftForeArm/mixamorig8:LeftHand/mixamorig8:LeftHandIndex1");
            _rightHand = transform.Find("mixamorig8:Hips/mixamorig8:Spine/mixamorig8:Spine1/mixamorig8:Spine2/mixamorig8:RightShoulder/mixamorig8:RightArm/mixamorig8:RightForeArm/mixamorig8:RightHand/mixamorig8:RightHandIndex1");

        }
    }




    // Update is called once per frame
    void Update()
    {
		if (Input.GetKey(KeyCode.X)) {

            if (!_desiredObj) { //update only if the object is not being taken
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
		}

		//human is inside the object's trigger zone
		if (_desiredObj) {
                        
			if (_humanComponent.StartedWaiting == false) {

				_animator.SetTrigger("PickUp");
                
                _humanComponent.HandPos = _desiredObj.position;



                _humanComponent.StartedWaiting = true;
				_humanComponent.FinishedWaiting = false;


            

            }
            else if (_humanComponent.StartedWaiting && !_humanComponent.FinishedWaiting && _desiredObj.GetComponent<ObjComponent>().Achieved == false) { //started waiting -- playing animation
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
	void OnAnimatorIK() {

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("pickingUp")) {
        //if (_desiredObj) { 
            float reach = _animator.GetFloat("RightHandReach");  //param is a curve set in pickingUp animation
            
            
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, reach);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, reach);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, _desiredObj.position);
            _animator.SetIKRotation(AvatarIKGoal.RightHand, _desiredObj.rotation);

		}

	}

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


	private void LateUpdate() {
        CurrentObjs.transform.position = _leftHand.position;
    }

}
