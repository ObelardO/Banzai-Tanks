using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSettings : MonoBehaviour
{

    private GameController gameController;

    public int enemyCount;
    public int enemyLimit = 10;

    private int totalEnemyDied;

    [System.Serializable]
    public struct Wave
    {
        public int waveID;
        public int needEnemyDied;

        public GameObject[] enemies;
    }

    public Wave[] waves = new Wave[5];

    public Wave currentWave;

    public int autoWaveInc = 10;

    public delegate void WaveEvent();
    public static event WaveEvent OnWaveComplite;

    private void Start()
    {
        currentWave = waves[0];

        gameController = FindObjectOfType<GameController>();
    }

    private void OnEnable()
    {
        Enemy.OnEnemyDie += OnEnemyDie;
        Enemy.OnEnemySpawn += OnEnemySpawn;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyDie -= OnEnemyDie;
        Enemy.OnEnemySpawn -= OnEnemySpawn;
    }

    public GameObject GetEnemyPrefab()
    {
        return currentWave.enemies[Random.Range(0, currentWave.enemies.Length)];
        //return null;
    }

    private void OnEnemyDie()
    {
        if (enemyCount > 0) enemyCount--;

        totalEnemyDied++;

        Debug.Log("total die: " + totalEnemyDied);

        for (int i = waves.Length - 1; i > 0; i--)
        {
            if (totalEnemyDied >= waves[i].needEnemyDied)
            {
                if (currentWave.waveID == waves[waves.Length - 1].waveID)
                {
                    if (totalEnemyDied - waves[i].needEnemyDied < autoWaveInc) break;
                    totalEnemyDied = currentWave.needEnemyDied;
                    waves[i].waveID++;
                }
                else if (currentWave.waveID >= waves[i].waveID) continue;





                currentWave = waves[i];
                Debug.Log("WAVE: " + waves[i].waveID);

                gameController.SetWave(waves[i].waveID);

                OnWaveComplite();
                break;
            }
        }


        //foreach (Wave wave in waves)
        //{
        // if (totalEnemyDied >= wave.)
        //}
    }

    private void OnEnemySpawn()
    {
        if (enemyCount < enemyLimit) enemyCount++;
    }

}
