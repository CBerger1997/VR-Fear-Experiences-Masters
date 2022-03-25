using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorDestroyer : MonoBehaviour {

    public GameObject Floor;

    private void Start() {
        StartCoroutine(EndLevel());
    }

    IEnumerator EndLevel() {
        yield return new WaitForSeconds(179);
        Floor.SetActive(false);
        GetComponent<AudioSource>().Play();
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
    }
}
