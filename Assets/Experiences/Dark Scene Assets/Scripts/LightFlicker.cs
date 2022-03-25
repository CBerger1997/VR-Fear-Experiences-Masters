using System.Collections;
using UnityEngine;

public class LightFlicker : MonoBehaviour {

    bool isLightActive = true;

    void Start() {
        StartCoroutine(FlickerLight());
    }

    IEnumerator FlickerLight() {
        yield return new WaitForSeconds(30);

        while (isLightActive) {
            int timeToFlicker = Random.Range(10, 20);

            yield return new WaitForSeconds(timeToFlicker);

            if (GetComponent<Light>().enabled == true) {

                int isLightFlicker = Random.Range(0, 2);

                if (isLightFlicker == 1) {
                    float flickerTime = Random.Range(0.1f, 1);

                    this.GetComponent<Light>().enabled = false;
                    yield return new WaitForSeconds(flickerTime);
                    this.GetComponent<Light>().enabled = true;
                    flickerTime = Random.Range(0.1f, 1);
                    yield return new WaitForSeconds(flickerTime);
                    this.GetComponent<Light>().enabled = false;
                    flickerTime = Random.Range(0.1f, 1);
                    yield return new WaitForSeconds(flickerTime);
                    this.GetComponent<Light>().enabled = true;
                }
            }
        }
    }
}
