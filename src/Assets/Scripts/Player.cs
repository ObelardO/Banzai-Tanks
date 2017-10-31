using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public float speed = 6.0f;
	public float turnSpeed = 90.0f;
    //public float health { get { return health; } set { health = value; OnParametrsChange(); } }
    public float health;
    private float maxHealth;
    //public float armor { get { return armor; } set { armor = value; OnParametrsChange(); } }
    public float armor;
    private float maxArmor;
    public float damage;

    public float itemHealthAdd;
    public float itemArmorAdd;
    public float ItemScoreAdd;

    public AudioSource gunSound;
    public AudioSource idleSound;
    public AudioSource runSound;

    public GameObject healthBar;
    public Image healthLine;
    public Image armorLine;

    public GameObject[] guns = new GameObject[3];
    public bool[] gunEnabled = new bool[3];
    public int currentGunID;
    public GameObject bullet;
    public string fraction;
    public float bulletReloadPeriod = 0.5f;
    private float bulletReloadTime = 0.0f;
    private GameObject currentGun;

    private GameController gameController;

    private Transform myTransform;
	
    private float velocity;
    private float turnAngle;

    private enum PlayerStates { idle, run }
    private PlayerStates playerState;

    private bool startmoving = false;

    private void SelectGun(int gunID = 0)
    {
        if (gunID < 0) gunID = guns.Length;

        gunID = gunID % (guns.Length);

        if (!gunEnabled[gunID]) return;

        currentGunID = gunID;

        for (int i = 0; i < guns.Length; i++) guns[i].SetActive(i == gunID ? true : false);

    }

    void Start ()
    {
        gameController = FindObjectOfType<GameController>();

        myTransform = transform;
        playerState = PlayerStates.idle;

        maxHealth = health;
        maxArmor = armor;

        health -= itemHealthAdd;
        armor -= itemArmorAdd;

        OnParametrsChange();
    }
	
	void Update ()
    {
        if (gameController.state != GameController.States.battle) return;

        if (Input.GetKeyDown(KeyCode.Q)) SelectGun(currentGunID - 1);
        if (Input.GetKeyDown(KeyCode.W)) SelectGun(currentGunID + 1);


        switch (playerState)
        {
            case PlayerStates.idle:

                if (startmoving)
                {
                    playerState = PlayerStates.run;
                    idleSound.Stop();
                    runSound.Play();
                }
                break;

            case PlayerStates.run:

                if (!startmoving)
                {
                    playerState = PlayerStates.idle;
                    runSound.Stop();
                    idleSound.Play();
                }
                break;
        }

        OpenFire();

	}

    void FixedUpdate ()
    {
        if (gameController.state != GameController.States.battle) return;

        float axisVertical = Input.GetAxisRaw("Vertical");
        float axisHorizontal = Input.GetAxisRaw("Horizontal");

        velocity = Mathf.Lerp(velocity, axisVertical * speed, Time.fixedDeltaTime * 2.0f);
		myTransform.Translate (Vector3.forward * velocity * Time.fixedDeltaTime);

        turnAngle = Mathf.Lerp(turnAngle, axisHorizontal * turnSpeed, Time.fixedDeltaTime * 3.0f);
        myTransform.Rotate (0, turnAngle * Time.fixedDeltaTime, 0);

        if (axisVertical != 0 || axisHorizontal != 0) startmoving = true; else startmoving = false;
    }

    private void OnEnable()
    {
        Bullet.OnBullet += OnBullet;

        ItemSpawner.OnItem += OnItem;
    }

    private void OnDisable()
    {
        Bullet.OnBullet -= OnBullet;

        ItemSpawner.OnItem -= OnItem;
    }

    private void OnBullet(GameObject player, string fraction, float damage)
    {
        if (gameObject == player && fraction != this.fraction)
        {
            health = health - damage * (1.0f - armor / maxArmor);
            armor -= damage;

            armor = Mathf.Clamp(armor, 0, maxArmor);

            OnParametrsChange();
        }

        //if (health > 0) return;
    }

    private void OnParametrsChange()
    {
        if (health > 0) healthLine.fillAmount = health / maxHealth; else healthLine.fillAmount = 0;
        if (armor > 0) armorLine.fillAmount = armor / maxArmor; else armorLine.fillAmount = 0;
    }

    private void OnItem(GameObject entity, GameObject item)
    {
        if (gameObject != entity) return;// Debug.Log(item.name);

        if (item.name.Contains("Health")) health = Mathf.Clamp(health + itemHealthAdd, health, maxHealth);
        if (item.name.Contains("Armor")) armor = Mathf.Clamp(armor + itemArmorAdd, armor, maxArmor);
        if (item.name.Contains("Gun1")) { gunEnabled[0] = true; SelectGun(0); } 
        if (item.name.Contains("Gun2")) { gunEnabled[1] = true; SelectGun(1); }
        if (item.name.Contains("Gun3")) { gunEnabled[2] = true; SelectGun(2); }

        if (item.name.Contains("Score")) gameController.SetScore(ItemScoreAdd);

        OnParametrsChange();
    }

    private void OpenFire()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.X)) && bulletReloadTime + bulletReloadPeriod < Time.time)
        {
            if (gunEnabled[currentGunID]) gunSound.Play();

            foreach (GameObject gun in guns)
            {
                if (!gun.activeInHierarchy) continue;

                for (int barrelID = 0; barrelID < gun.transform.childCount; barrelID++)
                {
                    Transform barrel = gun.transform.GetChild(barrelID);

                    Vector3 bulletPosition = barrel.position;
                    Vector3 bulletRotation = new Vector3(-90, barrel.eulerAngles.y, 0);

                    Instantiate(bullet, bulletPosition, Quaternion.Euler(bulletRotation)).GetComponent<Bullet>().Set(fraction, damage);
                }
            }

            bulletReloadTime = Time.time;

        }
    }
}
