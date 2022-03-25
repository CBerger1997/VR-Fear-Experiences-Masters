using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureInteraction : MonoBehaviour {

    public GameObject Creature;

    public Light RoomLight;

    private void Start() {
        StartCoroutine(SpawnCreatureWithLight());
    }

    IEnumerator SpawnCreatureWithLight() {
        yield return new WaitForSeconds(177.5f);
        RoomLight.enabled = false;
        Creature.SetActive(true);
        Creature.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(2.5f);
        Creature.SetActive(false);
    }
}
