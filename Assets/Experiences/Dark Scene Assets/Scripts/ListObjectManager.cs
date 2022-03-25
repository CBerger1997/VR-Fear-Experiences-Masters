using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListObjectManager : MonoBehaviour {

    public List<GameObject> ObjectsToFind;
    public GameObject clipboard;

    public Text clipboardTitle;
    public Text clipboardDesc;

    private void Start() {
        foreach (Transform child in this.transform) {
            ObjectsToFind.Add(child.gameObject);
        }
    }
    public void EnableListObjects() {
        clipboard.SetActive(true);

        foreach(GameObject item in ObjectsToFind) {
            item.SetActive(true);
        }
    }

    void Update() {
        if(ObjectsToFind.Count == 0) {
            clipboardTitle.text = "ToDo: Finish Shift";
            clipboardDesc.text = "Well done! You found the essentials, now enjoy the rest of your shift";
        }
    }
}
