using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Radar : MonoBehaviour {

    public GameObject healthBar;
    public Image healthLine;

    public float health;
    private float maxHealth;

	// Use this for initialization
	void Start ()
    {
        maxHealth = health;
    }

    public void OnBanzai(float damage)
    {
        health -= damage;

        health = Mathf.Clamp(health, 0, maxHealth);

        //if (health <= 0) gameover;     

        if (health > 0) healthLine.fillAmount = health / maxHealth; else healthLine.fillAmount = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //healthBar.transform.rotation = Camera.main.transform.rotation;
        //healthBar.transform.Rotate(0, 180, 0);
        healthBar.transform.LookAt(Camera.main.transform);
    }
}
