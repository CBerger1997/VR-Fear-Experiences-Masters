using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class SpiderController : MonoBehaviour {

    public GameObject waypoint { get; set; }
    int curWaypointCount = 0;
    public Vector3 RotationMod { get; set; }

    void Update() {

        //var newRotation = Quaternion.LookRotation(transform.position - target, Vector3.forward);
        //newRotation.x = 0.0f;
        //newRotation.y = 0.0f;
        //transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 20f);

        //transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * speed);

        //if (Vector3.Distance(transform.position, target) < 0.1f) {
        //    curWaypointCount++;
        //    target = waypoint.GetComponent<SpawnerWaypoints>().Waypoints[curWaypointCount];
        //    if (curWaypointCount >= waypoint.GetComponent<SpawnerWaypoints>().Waypoints.Count) {
        //        Destroy(this.gameObject);
        //    }
        //}

        Vector3 distanceToMove = waypoint.GetComponent<SpawnerWaypoints>().Waypoints[curWaypointCount] - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(distanceToMove), Time.deltaTime);

        transform.position = transform.position + transform.forward * Time.deltaTime;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(distanceToMove), Time.deltaTime * 5);

        /*if (RotationMod.z == 180) {
            Quaternion fixRotation = transform.rotation;
            Debug.Log("1: " + transform.rotation);
            fixRotation.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, RotationMod.z);
            Debug.Log("2: " + fixRotation);
            transform.rotation = fixRotation;
        } else */ 
        if (RotationMod != Vector3.zero) {
            Quaternion fixRotation = transform.rotation;
            fixRotation.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, 0, RotationMod.z);
            transform.rotation = fixRotation;
        }

        if (distanceToMove.magnitude < 0.1f) {
            curWaypointCount++;
            if (curWaypointCount >= waypoint.GetComponent<SpawnerWaypoints>().Waypoints.Count) {
                Destroy(this.gameObject);
            }
        }
    }
}
