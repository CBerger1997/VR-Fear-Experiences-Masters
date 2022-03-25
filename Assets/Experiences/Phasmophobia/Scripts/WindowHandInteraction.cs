using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WindowHandInteraction : MonoBehaviour {

    public Image HandLeft;
    public Image HandRight;

    void Start() {
        HandLeft.enabled = false;
        HandRight.enabled = false;
        StartCoroutine(TriggerHandInteraction());
    }

    IEnumerator TriggerHandInteraction() {
        int randomWaitTime = Random.Range(100, 120);
        yield return new WaitForSeconds(randomWaitTime);
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.1f);
        HandLeft.enabled = true;
        HandRight.enabled = true;
    }
}
