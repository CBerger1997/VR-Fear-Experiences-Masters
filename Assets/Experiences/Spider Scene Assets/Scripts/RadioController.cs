using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioController : MonoBehaviour {
    public List<GameObject> Radios;

    int radioStage = 0;

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter(Collider other) {
        if (Radios.Count > 1) {
            if (other.tag == "Dial") {
                GameObject newRadio = Instantiate(Radios[0], transform.parent);
                newRadio.transform.position = transform.position;
                Destroy(other.gameObject);
                Destroy(this.gameObject);
            } else if (other.tag == "Antenna") {
                GameObject newRadio = Instantiate(Radios[1], transform.parent);
                newRadio.transform.position = transform.position; Destroy(other.gameObject);
                Destroy(this.gameObject);
            }
        } else if (other.tag == "Dial" || other.tag == "Antenna") {
            GameObject newRadio = Instantiate(Radios[0], transform.parent);
            newRadio.transform.position = transform.position;
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }
}
