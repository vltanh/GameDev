using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    int level = 1;
    int numberOfEnemies = 2;

    public bool isShootable = false;

    public GameObject bigEnemy;
    public GameObject enemySpaceship;
    public GameObject warningArea;

    public GameObject powerUp;
    private float lastPowerUpSpawn = 0.0f;
    public float powerUpSpawnTime = 2.0f;
    public float powerUpTime = 10.0f;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Time.time > lastPowerUpSpawn + powerUpSpawnTime)
        {
            float spawnY = UnityEngine.Random.Range
                    (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
            float spawnX = UnityEngine.Random.Range
                    (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);

            Vector2 spawnPosition = new Vector2(spawnX, spawnY);
            GameObject tmpPU = Instantiate(powerUp, spawnPosition, Quaternion.identity);
            Destroy(tmpPU, powerUpTime);
            lastPowerUpSpawn = Time.time;
        }
    }

    public void UpdateNumberOfEnemies(int val)
    {
        numberOfEnemies += val;
        if (numberOfEnemies <= 0)
        {
            levelUp();
        }
    }

    private void levelUp()
    {
        CancelInvoke();
        level++;
        isShootable = false;
        StartLevel();
    }

    public void StartLevel()
    {
        numberOfEnemies = 0;
        List<Vector2> newEnemies = new List<Vector2>();
        for (int i = 0; i < level; i++)
        {
            float spawnY = UnityEngine.Random.Range
                    (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
            float spawnX = UnityEngine.Random.Range
                    (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);

            Vector2 spawnPosition = new Vector2(spawnX, spawnY);
            newEnemies.Add(spawnPosition);
            GameObject tmp = Instantiate(warningArea, spawnPosition, Quaternion.identity);
            Destroy(tmp, 5.0f);
        }

        StartCoroutine(SpawnEnemiesAfter(5.0f, newEnemies));
        Invoke("StartSpawnEnemySpaceship", 10.0f);
    }

    IEnumerator SpawnEnemiesAfter(float time, List<Vector2> newEnemies)
    {
        yield return new WaitForSeconds(time);

        isShootable = true;
        foreach (Vector2 spawnPosition in newEnemies) {
            Instantiate(bigEnemy, spawnPosition, Quaternion.identity);
            numberOfEnemies++;
        }
    }

    void StartSpawnEnemySpaceship()
    {
        float spawnY = UnityEngine.Random.Range
                    (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
        float spawnX = UnityEngine.Random.Range
                (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);

        Vector2 spawnPosition = new Vector2(spawnX, spawnY);
        GameObject tmp = Instantiate(warningArea, spawnPosition, Quaternion.identity);
        Destroy(tmp, 2.0f);
        StartCoroutine(SpawnEnemySpaceshipAfter(2.0f, spawnPosition));
    }

    IEnumerator SpawnEnemySpaceshipAfter(float t, Vector2 spawnPosition)
    {
        yield return new WaitForSeconds(t);
        Instantiate(enemySpaceship, spawnPosition, Quaternion.identity);
        numberOfEnemies++;
    }

    public void SwitchScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
