using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour {
    public List<GameObject> Masks;
    public GlassBoxManager GlassManager;
    public OVRGrabber Left;
    public OVRGrabber Right;

    void Start() {
        foreach (GameObject mask in Masks) {
            if (mask != null) {
                Left.enabled = false;
                Right.enabled = false;
                mask.GetComponent<MaskObject>().CanBePickedUp = false;
                mask.GetComponent<OVRGrabbable>().enabled = false;
            }
        }
    }

    public void SetMasksAsInteractable() {
        foreach(GameObject mask in Masks) {
            if (mask != null) {
                Left.enabled = true;
                Right.enabled = true;
                mask.GetComponent<MaskObject>().CanBePickedUp = true;
                mask.GetComponent<OVRGrabbable>().enabled = true;
                GlassManager.ActivateGlassBoxes();
            }
        }
    }

    public void SetMasksAsDeactivated() {
        foreach (GameObject mask in Masks) {
            if (mask != null) {
                Left.enabled = false;
                Right.enabled = false;
                mask.GetComponent<MaskObject>().CanBePickedUp = false;
                mask.GetComponent<OVRGrabbable>().enabled = false;
                GlassManager.ActivateGlassBoxes();
            }
        }
    }

    public void MaskDelay() {
        StartCoroutine(HideMasksForTime());
    }

    IEnumerator HideMasksForTime() {
        SetMasksAsDeactivated();
        yield return new WaitForSeconds(20);
        SetMasksAsInteractable();
    }
}
