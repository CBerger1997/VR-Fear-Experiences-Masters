using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListObject : MonoBehaviour {

    public Text listText;
    public ListObjectManager manager;

    private void Update() {
        if(this.GetComponent<OVRGrabbable>().isGrabbed) {
            listText.text = "";
            manager.ObjectsToFind.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
    }
}
