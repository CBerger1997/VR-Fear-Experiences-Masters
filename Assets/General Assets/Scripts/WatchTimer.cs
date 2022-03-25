using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WatchTimer : MonoBehaviour {
    int currentMins = 3;
    int currentSecs = 0;

    public Text Timer;

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown() {
        while (currentMins > 0 || currentSecs > 0) {
            yield return new WaitForSeconds(1);

            if (currentSecs == 0) {
                currentSecs = 59;
                currentMins--;
            } else {
                currentSecs--;
            }

            if (currentSecs < 10) {
                Timer.text = currentMins.ToString() + ":0" + currentSecs.ToString();
            } else {
                Timer.text = currentMins.ToString() + ":" + currentSecs.ToString();
            }
        }

        GameObject.FindObjectOfType<LevelController>().LoadBackToIntermediaryLevel();
    }
}
