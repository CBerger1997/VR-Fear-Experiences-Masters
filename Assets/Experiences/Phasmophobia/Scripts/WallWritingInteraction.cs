using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallWritingInteraction : MonoBehaviour {

    public List<GameObject> walls;
    public Light VisitRoomLight;
    public Light PlayerRoomLight;

    void Start() {
        foreach (GameObject wall in walls) {
            wall.SetActive(false);
        }
        StartCoroutine(TriggerWallInteraction());
    }

    IEnumerator TriggerWallInteraction() {
        int randomWaitTime = Random.Range(120, 150);
        yield return new WaitForSeconds(randomWaitTime);

        VisitRoomLight.enabled = false;
        PlayerRoomLight.enabled = false;
        GetComponent<AudioSource>().Play();
        foreach (GameObject wall in walls) {
            wall.SetActive(true);
        }
        yield return new WaitForSeconds(1);
        VisitRoomLight.enabled = true;
        PlayerRoomLight.enabled = true;
        yield return new WaitForSeconds(0.25f);
        VisitRoomLight.enabled = false;
        PlayerRoomLight.enabled = false;
        yield return new WaitForSeconds(0.25f);
        VisitRoomLight.enabled = true;
        PlayerRoomLight.enabled = true;
    }
}
