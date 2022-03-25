using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorrorNoiseController : MonoBehaviour {

    List<AudioSource> horrorSounds;

    void Start() {
        horrorSounds = new List<AudioSource>();

        foreach (Transform child in transform) {
            horrorSounds.Add(child.GetComponent<AudioSource>());
        }

        StartCoroutine(PlayHorrorSound());
    }

    IEnumerator PlayHorrorSound() {
        while (true) {
            int randomTimer = Random.Range(10, 20);

            yield return new WaitForSeconds(randomTimer);

            int horrorSoundIndex = Random.Range(0, horrorSounds.Count);

            horrorSounds[horrorSoundIndex].Play();
        }
    }
}
