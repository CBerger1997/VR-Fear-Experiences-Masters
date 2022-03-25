using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPickUp : MonoBehaviour {

    public Transform PickUpPosition;

    void Update() {
        if(GetComponent<OVRGrabbable>().isGrabbed) {
            transform.position = PickUpPosition.position;
            transform.rotation = PickUpPosition.rotation;

            GetComponent<CameraFlash>().enabled = true;
        } else {
            GetComponent<CameraFlash>().enabled = false;
        }
    }
}
