using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpTracker : MonoBehaviour {

    public Transform HeadTracker;

    List<float> heightVals;

    float AvgHeight = 0;

    bool hasJumped = false;
    bool isAvgHeightCalculated = false;

    void Start() {
        heightVals = new List<float>();

        StartCoroutine(AverageHeightCalc());
    }

    void Update() {
        this.transform.position = HeadTracker.position;
        if (isAvgHeightCalculated && !hasJumped) {
            DetectJump();
        } else if (isAvgHeightCalculated && hasJumped) {
            DetectLanding();
        }
    }

    private void DetectJump() {
        if (transform.localPosition.y >= AvgHeight + 0.15f) {
            hasJumped = true;
        }
    }

    private void DetectLanding() {
        if (transform.localPosition.y <= AvgHeight + 0.01f && transform.localPosition.y >= AvgHeight - 0.01f) {
            hasJumped = false;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit)) {
                if (hit.transform.GetComponentInChildren<Image>().enabled == false) {
                    hit.transform.GetComponentInChildren<Image>().enabled = true;
                    hit.transform.GetComponent<AudioSource>().Play();
                }
            }
        }
    }

    IEnumerator AverageHeightCalc() {
        yield return new WaitForSeconds(2);

        int count = 0;

        while (count < 10) {
            heightVals.Add(transform.localPosition.y);
            count++;
            yield return new WaitForSeconds(1);
        }

        foreach (float val in heightVals) {
            AvgHeight += val;
        }

        AvgHeight /= 10;

        isAvgHeightCalculated = true;

        GetComponent<BoxCollider>().size = new Vector3(0.1f, AvgHeight, 0.1f);
        GetComponent<BoxCollider>().center = new Vector3(0, -(AvgHeight / 2), 0);

        Debug.Log("Average Height: " + AvgHeight);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Laser") {
            this.GetComponent<AudioSource>().Play();
        }
    }
}
