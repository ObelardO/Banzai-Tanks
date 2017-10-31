using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public GameObject menuUI;
    public GameObject battleUI;
    public GameObject gameOverUI;
    public Text textScore;
    public Text textScoreInBattle;
    public Text textWavesInBattle;

    public SpawnController spawners;
    public Player player;
    public Radar radar;

    public LevelSettings levelSettings;

    public enum States{ menu, battle, gameOver}

    public States state;

    public float score;

    public void SetScore(float scoreAdd)
    {
        score += scoreAdd;

        textScoreInBattle.text = "SCORE: " + ((int)score).ToString();
    }

    public void SetWave(int wave)
    {
        
        textWavesInBattle.text = "WAVE: " + ((int)wave).ToString();
    }

	// Use this for initialization
	void Start ()
    {
        state = States.menu;

        Cursor.visible = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        switch (state)
        {
            case States.menu:
                
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    state = States.battle;

                    menuUI.SetActive(false);
                    battleUI.SetActive(true);
                }
                break;

            case States.battle:

                if (player.health <= 0 || radar.health <= 0) //|| Input.GetKeyDown(KeyCode.F))
                {
                    state = States.gameOver;

                    battleUI.SetActive(false);
                    gameOverUI.SetActive(true);

                    textScore.text = "your score: " + ((int)score).ToString();
                }

                break;


            case States.gameOver:

                if (Input.GetKeyDown(KeyCode.Space)) SceneManager.LoadScene(0);

                break;

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
