using UnityEngine;
using UnityEngine.UI;

public class MaskObject : MonoBehaviour {

    public bool CanBePickedUp { get; set; } = false;

    public GameObject Manager;

    private void Update() {
        if (CanBePickedUp && this.GetComponent<OVRGrabbable>().isGrabbed) {
            Manager.GetComponent<MaskManager>().Masks.Remove(this.gameObject);
            Manager.GetComponent<MaskManager>().MaskDelay();
            Destroy(this.gameObject);
        }
    }
}
