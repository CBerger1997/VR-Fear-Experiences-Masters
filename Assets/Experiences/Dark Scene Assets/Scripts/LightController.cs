using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour {
    public GameObject playerLight;
    public List<GameObject> Lights;
    int lightCounter = 0;

    int offTimer = 60;

    bool isLightsOff = false;

    private void Start() {
        foreach(Transform child in this.transform) {
            Lights.Add(child.gameObject);
        }

        StartCoroutine(TurnOffLights());
    }

    IEnumerator TurnOffLights() {
        while (lightCounter < Lights.Count) {
            yield return new WaitForSeconds(offTimer);
            offTimer = 30;
            Lights[lightCounter].GetComponent<LightAudio>().ExternalAudio.Play();
            Lights[lightCounter].SetActive(false);
            lightCounter++;
        }

        yield return new WaitForSeconds(5);
        playerLight.SetActive(true);
        playerLight.GetComponent<PersonalLightFlicker>().enabled = true;
    }

    public void AllLightsOn() {
        foreach(GameObject light in Lights) {
            light.GetComponent<Light>().enabled = true;
        }
    }   
    
    public void AllLightsOff() {
        foreach(GameObject light in Lights) {
            light.GetComponent<Light>().enabled = false;
        }
    }

    public void TriggerLightsOff() {
        if (!isLightsOff) {
            StartCoroutine(TurnOffAllLights());
        }
    }

    IEnumerator TurnOffAllLights() {
        isLightsOff = true;
        Lights[Lights.Count - 1].GetComponent<LightAudio>().ExternalAudio.Play();
        AllLightsOff();

        yield return new WaitForSeconds(5);

        isLightsOff = false;
        AllLightsOn();
    }
}
