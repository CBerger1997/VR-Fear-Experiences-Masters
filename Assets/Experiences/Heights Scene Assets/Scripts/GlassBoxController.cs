using System.Collections;
using UnityEngine;

public class GlassBoxController : MonoBehaviour {

    Vector3 CoveredPoint;
    Vector3 OpenPoint;

    bool isUncovered = false;
    public bool IsMoving { get; set; } = false;


    private void Start() {
        CoveredPoint = new Vector3(0, 0.7f, 0);
        OpenPoint = new Vector3(0, 0.3f, 0);
    }

    private void Update() {
        if (IsMoving) {
            if (isUncovered) {
                transform.localPosition = Vector3.Lerp(transform.localPosition, CoveredPoint, Time.deltaTime * 2);
                if (Vector3.Distance(transform.localPosition, CoveredPoint) < 0.01f) {
                    isUncovered = false;
                    IsMoving = false;
                }
            } else {
                transform.localPosition = Vector3.Lerp(transform.localPosition, OpenPoint, Time.deltaTime * 2);
                if (Vector3.Distance(transform.localPosition, OpenPoint) < 0.01f) {
                    isUncovered = true;
                    IsMoving = false;
                }
            }
        }
    }
}
