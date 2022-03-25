using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpawnerWaypoints : MonoBehaviour {

    public List<Vector3> Waypoints;

    public bool IsOnLeftWall;
    public bool IsOnRightWall;
    public bool IsOnRoof;

    public Vector3 RotationMod { get; set; }

    private void Start() {
        foreach (Transform child in this.transform) {
            Waypoints.Add(child.position);
        }

        if (IsOnLeftWall) {
            RotationMod = new Vector3(0, 0, -90);
        } else if (IsOnRightWall) {
            RotationMod = new Vector3(-90, 0, 90);
        } else if (IsOnRoof) {
            RotationMod = new Vector3(180, 0, 180);
        } else {
            RotationMod = Vector3.zero;
        }

    }
}
