using System.Collections;
using UnityEngine;

public class CameraFlash : MonoBehaviour {

    Light cameraLight;

    bool isFlashing = false;

    void Start() {
        cameraLight = GetComponentInChildren<Light>();

        cameraLight.enabled = false;
    }

    void Update() {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) {
            if (!isFlashing) {
                StartCoroutine(Flash());
            }
        }
    }

    IEnumerator Flash() {
        isFlashing = true;
        GetComponent<AudioSource>().Play();

        cameraLight.enabled = true;
        yield return new WaitForSeconds(0.25f);
        cameraLight.enabled = false;
        yield return new WaitForSeconds(0.5f);
        isFlashing = false;
    }
}
