using System.Collections;
using UnityEngine;

public class PersonalLightFlicker : MonoBehaviour {

    private void OnEnable() {
        StartCoroutine(FlickerLight());
    }

    IEnumerator FlickerLight() {
        int count = 0;

        while (count < 2) {

            yield return new WaitForSeconds(8);

            if (GetComponent<Light>().enabled == true) {
                float flickerTime = Random.Range(0.1f, 0.5f);

                this.GetComponent<Light>().enabled = false;
                yield return new WaitForSeconds(flickerTime);
                this.GetComponent<Light>().enabled = true;
                flickerTime = Random.Range(0.1f, 0.5f);
                yield return new WaitForSeconds(flickerTime);
                this.GetComponent<Light>().enabled = false;
                flickerTime = Random.Range(0.1f, 0.5f);
                yield return new WaitForSeconds(flickerTime);
                this.GetComponent<Light>().enabled = true;
            }

            count++;
        }

        this.GetComponent<Light>().enabled = false;
    }
}
