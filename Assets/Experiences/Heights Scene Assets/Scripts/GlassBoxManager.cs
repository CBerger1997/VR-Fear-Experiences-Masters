using System.Collections.Generic;
using UnityEngine;

public class GlassBoxManager : MonoBehaviour {
    public List<GlassBoxController> GlassBoxes;

    public void ActivateGlassBoxes() {
        foreach(GlassBoxController glassBox in GlassBoxes) {
            glassBox.IsMoving = true;
        }
    }
}
