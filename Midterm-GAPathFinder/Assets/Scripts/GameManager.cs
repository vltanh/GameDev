using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int level = 1;
    int numberOfEnemies = 2;

    public GameObject bigEnemy;

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
        level++;
        numberOfEnemies = 0;
        for (int i = 0; i < level; i++)
        {
            float spawnY = UnityEngine.Random.Range
                    (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
            float spawnX = UnityEngine.Random.Range
                    (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);

            Vector2 spawnPosition = new Vector2(spawnX, spawnY);
            Instantiate(bigEnemy, spawnPosition, Quaternion.identity);
            numberOfEnemies++;
        }
    }
}
