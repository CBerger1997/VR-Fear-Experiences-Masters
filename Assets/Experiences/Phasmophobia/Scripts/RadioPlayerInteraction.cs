using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioPlayerInteraction : MonoBehaviour {

    public AudioClip RadioSound;
    public AudioClip BackgroundSound;
    public List<AudioClip> HorrorSounds;
    public Light PlayerRoomLight;

    AudioSource audioSource;

    bool RadioActive = false;

    void Start() {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = BackgroundSound;
        audioSource.Play();

        //StartCoroutine(PlayRadioSound());
    }

    public void TriggerRadio() {
        if (!RadioActive) {
            StartCoroutine(PlayRadioSound());
        }
    }

    IEnumerator PlayRadioSound() {
        //  while (true) {

        //audioSource.clip = BackgroundSound;
        //audioSource.Play();

        //int randomWaitTime = Random.Range(30, 60);

        //yield return new WaitForSeconds(randomWaitTime);

        RadioActive = true;

        audioSource.clip = RadioSound;
        audioSource.Play();

        yield return new WaitForSeconds(4);

        int horrorIndex = Random.Range(0, HorrorSounds.Count);

        audioSource.clip = HorrorSounds[horrorIndex];
        audioSource.Play();

        yield return new WaitForSeconds(audioSource.clip.length);

        audioSource.clip = RadioSound;
        audioSource.Play();

        yield return new WaitForSeconds(4);

        audioSource.clip = BackgroundSound;
        audioSource.Play();

        RadioActive = false;
        // }
    }
}
