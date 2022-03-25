using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLightController : MonoBehaviour {

    public Material RedLight;

    public void ChangeLightToRed() {
        GetComponent<MeshRenderer>().material = RedLight;
        GetComponentInChildren<Light>().color = Color.red;
    }
}
