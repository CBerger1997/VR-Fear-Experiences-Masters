using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

    static LevelController instance = null;

    public static LevelController Instance {
        get { return instance; }
    }

    int levelCount = 1;

    bool isInLevel = false;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    void Start() {
        DontDestroyOnLoad(this);
    }

    private void Update() {
        if (!isInLevel) {
            if (OVRInput.GetDown(OVRInput.Button.One)) {
                SceneManager.LoadScene(levelCount);
                levelCount++;
                isInLevel = true;
            }
        }
    }

    public void LoadBackToIntermediaryLevel() {
        if(levelCount > 5) {
            Application.Quit();
        }
        
        isInLevel = false;
        SceneManager.LoadScene(0);
    }
}
