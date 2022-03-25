using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingInteractionManager : MonoBehaviour {

    List<GameObject> ThrowableObjects;

    void Start() {
        ThrowableObjects = new List<GameObject>();

        foreach(Transform child in transform) {
            ThrowableObjects.Add(child.gameObject);
        }

        Debug.Log(ThrowableObjects.Count);

        StartCoroutine(ThrowObjects());
    }

    IEnumerator ThrowObjects() {

        while (true) {
            int randomTimer = Random.Range(5, 20);

            yield return new WaitForSeconds(randomTimer);

            int objectIndex = Random.Range(0, ThrowableObjects.Count);

            ThrowableObjects[objectIndex].GetComponent<ThrowingInteraction>().ThrowObject();
        }
    }
}
