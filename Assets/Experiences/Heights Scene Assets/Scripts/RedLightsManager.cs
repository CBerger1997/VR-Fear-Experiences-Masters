using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLightsManager : MonoBehaviour {

    public List<GameObject> lights;

    void Start() {
        lights = new List<GameObject>();

        foreach(Transform child in transform) {
            lights.Add(child.gameObject);
        }
    }
    
    public void TurnLightsRed() {
        foreach(GameObject light in lights) {
            light.GetComponent<RedLightController>().ChangeLightToRed();
        }
    }
}
