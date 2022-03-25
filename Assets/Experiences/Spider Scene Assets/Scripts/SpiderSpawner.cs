using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderSpawner : MonoBehaviour {

    public GameObject Spider;

    float maxSpawnSize = 0.05f;
    float minSpawnSize = 0.02f;

    float spiderSpawnTimer = 9;

    public List<GameObject> SpawnPoints;


    void Start() {
        foreach (Transform child in this.transform) {
            SpawnPoints.Add(child.gameObject);
        }

        StartCoroutine(WaitTimerToSpawn());
    }

    IEnumerator WaitTimerToSpawn() {
        yield return new WaitForSeconds(30);
        StartCoroutine(SpawnSpidersController());
    }

    IEnumerator SpawnSpidersController() {
        int count = 0;
        while (true) {

            SpawnSpider();

            count++;

            if (count >= 3) {
                spiderSpawnTimer--;
                if (spiderSpawnTimer <= 0) {
                    spiderSpawnTimer = 0.5f;
                }
                count = 0;
            }

            yield return new WaitForSeconds(spiderSpawnTimer);
        }
    }

    public void SpawnSpider() {
        int spawnerVal = (int)Random.Range(0, SpawnPoints.Count);
        GameObject newSpider = Instantiate(Spider);
        newSpider.transform.position = SpawnPoints[spawnerVal].transform.position;
        float size = Random.Range(minSpawnSize, maxSpawnSize);
        newSpider.transform.localScale = new Vector3(size, size, size);
        newSpider.GetComponent<SpiderController>().waypoint = SpawnPoints[spawnerVal];
        newSpider.GetComponent<SpiderController>().RotationMod = SpawnPoints[spawnerVal].GetComponent<SpawnerWaypoints>().RotationMod;
        newSpider.transform.rotation = Quaternion.Euler(SpawnPoints[spawnerVal].GetComponent<SpawnerWaypoints>().RotationMod);
    }
}
