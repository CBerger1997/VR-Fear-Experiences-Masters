using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {

    public GameObject playerLight;
    public LightController lightController;
    public ListObjectManager objectManager;
    public GameObject Generator;

    bool isButtonOnePressed = false;
    bool isButtonTwoPressed = false;

    public void ButtonOnePressed() {
        isButtonOnePressed = true;

        if (isButtonTwoPressed) {
            playerLight.SetActive(false);
            lightController.AllLightsOn();
            objectManager.EnableListObjects();
            Generator.GetComponent<AudioSource>().Play();
        }
    }

    public void ButtonTwoPressed() {
        isButtonTwoPressed = true;

        if (isButtonOnePressed) {
            playerLight.SetActive(false);
            lightController.AllLightsOn();
            objectManager.EnableListObjects();
            Generator.GetComponent<AudioSource>().Play();
        }
    }
}
