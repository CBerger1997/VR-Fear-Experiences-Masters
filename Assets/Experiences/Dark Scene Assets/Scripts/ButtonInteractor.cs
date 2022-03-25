using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class ButtonInteractor : MonoBehaviour {
    Vector3 startPos;

    public float zEndPos = -4.3011f;

    public bool isButtonOne = false;
    public bool isButtonTwo = false;

    void Start() {
        startPos = transform.position;
    }

    void Update() {
        if (transform.position.z > startPos.z) {
            transform.position = startPos;
        } else if (transform.position.z <= zEndPos) {
            transform.position = new Vector3(transform.position.x, transform.position.y, zEndPos);
            GetComponent<AudioSource>().Play();
            if (isButtonOne) {
                GetComponentInParent<ButtonController>().ButtonOnePressed();
            } else if (isButtonTwo) {
                GetComponentInParent<ButtonController>().ButtonTwoPressed();
            }
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<ButtonInteractor>().enabled = false;
        }
    }
}
