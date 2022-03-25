using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserManager : MonoBehaviour {

    public GameObject Laser;

    public bool IsLeft;
    public bool IsRight;
    public bool IsForward;
    public bool IsBack;

    Vector3 direction;

    private void Start() {

        if (IsLeft) {
            direction = Vector3.left;
        } else if (IsRight) {
            direction = Vector3.right;
        } else if (IsForward) {
            direction = Vector3.forward;
        } else if (IsBack) {
            direction = Vector3.back;
        }

        StartCoroutine(GenerateLaser());
    }

    IEnumerator GenerateLaser() {
        while (true) {
            int randomWaitTime = Random.Range(10, 20);
            yield return new WaitForSeconds(randomWaitTime);

            GameObject laser = Instantiate(Laser, this.transform);
            laser.transform.localPosition = new Vector3(0, 0, 0);
            laser.GetComponent<LaserController>().direction = direction;
        }
    }
}
