using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepAudioIncreaser : MonoBehaviour {
    AudioSource footstepSound;
    // Start is called before the first frame update
    void Start() {
        footstepSound = GetComponent<AudioSource>();
        StartCoroutine(IncreaseFootstepVolume());
    }

    IEnumerator IncreaseFootstepVolume() {
        while (true) {
            yield return new WaitForSeconds(1);
            footstepSound.volume += 0.0056f;
        }
    }
}
