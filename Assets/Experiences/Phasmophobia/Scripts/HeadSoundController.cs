using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSoundController : MonoBehaviour {

    List<AudioSource> headSounds;

    void Start() {
        headSounds = new List<AudioSource>();

        foreach( Transform child in transform) {
            headSounds.Add(child.GetComponent<AudioSource>());
        }

        StartCoroutine(PlayHeadSound());
    }
    
    IEnumerator PlayHeadSound() {

        while (true) {
            int randomTimer = Random.Range(20, 40);

            yield return new WaitForSeconds(randomTimer);

            int headSoundIndex = Random.Range(0, headSounds.Count);

            headSounds[headSoundIndex].Play();
        }
    }
}
