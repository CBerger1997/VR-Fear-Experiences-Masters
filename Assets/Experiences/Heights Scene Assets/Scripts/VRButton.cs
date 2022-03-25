using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRButton : MonoBehaviour {
    Vector3 startPos;

    public RedLightsManager LightsManager;
    public AudioSource SirenAudio;
    public MaskManager MasksManager;
    public GameObject LaserParent;

    public float yEndPos;

    public bool isPressed { get; set; }

    void Start() {
        startPos = transform.position;
        LaserParent.SetActive(false);
    }

    void Update() {
        if (transform.position.y > startPos.y) {
            transform.position = startPos;
        } else if (transform.position.y <= yEndPos) {
            transform.position = new Vector3(transform.position.x, yEndPos, transform.position.z);
            GetComponent<AudioSource>().Play();
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = false;
            LightsManager.TurnLightsRed();
            SirenAudio.Play();
            MasksManager.SetMasksAsInteractable();
            LaserParent.SetActive(true);
            GetComponent<VRButton>().enabled = false;
        }
    }
}
