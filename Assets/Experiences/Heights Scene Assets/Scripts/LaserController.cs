using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LaserController : MonoBehaviour {

    float speed = 3;

    public Vector3 direction { get; set; }

    void Update() {
        transform.position = transform.position + direction * Time.deltaTime * speed;
    }
}
