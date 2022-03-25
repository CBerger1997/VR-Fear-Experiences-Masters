using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxController : MonoBehaviour {

    public List<AudioClip> SfxAudio;

    public List<GameObject> audioPoints;

    void Start() {

        foreach (Transform child in this.transform) {
            audioPoints.Add(child.gameObject);
        }

        StartCoroutine(PlaySFXAudio());
    }


    IEnumerator PlaySFXAudio() {

        yield return new WaitForSeconds(30);

        while (true) {

            int randomTimeRange = Random.Range(5, 20);

            yield return new WaitForSeconds(randomTimeRange);

            int randomChildIndex = Random.Range(0, audioPoints.Count);

            int randomAudioIndex = Random.Range(0, SfxAudio.Count);

            audioPoints[randomChildIndex].GetComponent<AudioSource>().clip = SfxAudio[randomAudioIndex];
            audioPoints[randomChildIndex].GetComponent<AudioSource>().Play();
        }
    }
}
