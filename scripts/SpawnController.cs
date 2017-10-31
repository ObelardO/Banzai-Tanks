using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour {

    //public GameObject[] enemies = new GameObject[3];

    private LevelSettings levelSettings;
    private Spawner[] spawners;
    private GameController gameController;

    // Use this for initialization
    void Start ()
    {
        levelSettings = FindObjectOfType<LevelSettings>().GetComponent<LevelSettings>();
        spawners = GetComponentsInChildren<Spawner>();

        gameController = FindObjectOfType<GameController>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (gameController.state != GameController.States.battle) return;

        if (levelSettings.enemyCount < levelSettings.enemyLimit) spawners[Random.Range(0, spawners.Length)].SpawnEnemy(levelSettings.GetEnemyPrefab());
	}
}
