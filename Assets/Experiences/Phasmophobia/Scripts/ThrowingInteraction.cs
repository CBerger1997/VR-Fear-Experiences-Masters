using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingInteraction : MonoBehaviour {

    Rigidbody rb;

    public Mesh objectChange;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    public void ThrowObject() {
        float randX = Random.Range(-1.5f, 1.5f);
        float randY = Random.Range(0.5f, 1);
        float randZ = Random.Range(-1.5f, 1.5f);

        rb.AddForce(new Vector3(randX, randY, randZ) * 5, ForceMode.Impulse);

        if(objectChange != null) {
            GetComponent<MeshFilter>().mesh = objectChange;
        }
    }
}
