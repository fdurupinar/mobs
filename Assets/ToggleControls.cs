using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleControls : MonoBehaviour
{
    public GameObject Controls;
    private bool _activeStatus;

    private void Start() {
        _activeStatus = false;
        Controls.SetActive(_activeStatus);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T)) {
            _activeStatus = !_activeStatus;
            Controls.SetActive(_activeStatus);
        }
        
    }
}
