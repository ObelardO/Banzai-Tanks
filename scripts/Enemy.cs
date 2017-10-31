using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

	public Spawner mySpawner;
    public GameObject explosion;
    public Transform player;

    private GameObject radar;
    public bool banzai;

    public float speed;
    public float turnSpeed;
    public float health;
    private float maxHealth;
    public float armor;
    private float maxArmor;
    public float damage;
    public float shotDistance;

    public GameObject healthBar;
    public Image healthLine;
    public Image armorLine;

    public GameObject[] guns = new GameObject[3];
    public GameObject bullet;
    public string fraction;
    public AudioSource gunSound;
    public float bulletReloadPeriod = 0.5f;
    private float bulletReloadTime = 0.0f;
    private GameObject currentGun;

    public delegate void EnemyDieEvent();
    public static event EnemyDieEvent OnEnemyDie;

    public delegate void EnemySpawnEvent();
    public static event EnemySpawnEvent OnEnemySpawn;

    private enum EnemyStates {idle, run, ai}
	private EnemyStates enemyState;
    private NavMeshAgent navMeshAgent;
    private float runDistance;
    private Transform myTransform;

    private GameController gameController;

    private void Start ()
    {
        gameController = FindObjectOfType<GameController>();

        enemyState = EnemyStates.idle;

        OnEnemySpawn();

        myTransform = transform;

        player = FindObjectOfType<Player>().transform;

        bulletReloadTime = Time.time;

        maxHealth = health;
        maxArmor = armor;

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        navMeshAgent.angularSpeed = turnSpeed;
        navMeshAgent.stoppingDistance = shotDistance;
        

        if (banzai)
        {
            radar = FindObjectOfType<Radar>().gameObject;
            navMeshAgent.stoppingDistance = 0;
        }

        navMeshAgent.isStopped = true;
    }

    private void Update ()
    {
        healthBar.transform.rotation = Camera.main.transform.rotation;
        healthBar.transform.Rotate(0, 180, 0);

        if (gameController.state != GameController.States.battle) return;

        if (banzai && radar != null && Vector3.Distance(myTransform.position, radar.transform.position) < 2.5f)
        {
            radar.GetComponent<Radar>().OnBanzai(damage);
            Die();
            return;
        }

        if (!banzai && enemyState == EnemyStates.ai && Vector3.Distance(myTransform.position, player.position) <= shotDistance + 1.0f)
        {
            Vector3 dirToTarget = (player.position - myTransform.position).normalized;

            float targetAngle = 90 - Mathf.Atan2(dirToTarget.z, dirToTarget.x) * Mathf.Rad2Deg;

            float turnAngle = Mathf.MoveTowardsAngle(myTransform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime * 0.5f);
            transform.eulerAngles = Vector3.up * turnAngle;


            OpenFire();
        }
    }

    private void FixedUpdate()
    {
        if (gameController.state != GameController.States.battle) { navMeshAgent.isStopped = true; return; }

        switch (enemyState)
        {
            case EnemyStates.idle:

                if (mySpawner == null) enemyState = EnemyStates.ai;
                else if (mySpawner.isOpened)
                {
                    enemyState = EnemyStates.run;
                    mySpawner.isOpened = false;
                }

                break;
            case EnemyStates.run:

                myTransform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
                runDistance += speed * Time.fixedDeltaTime;
                if (runDistance >= 2.0f) enemyState = EnemyStates.ai;

                break;

            case EnemyStates.ai:

                if (navMeshAgent.isStopped) navMeshAgent.isStopped = false;

                if (!banzai)navMeshAgent.SetDestination(player.position);
                else navMeshAgent.SetDestination(radar.transform.position);

                break;
        }
    }

    private void OnEnable()
    {
        Bullet.OnBullet += OnBullet;
    }

    private void OnDisable()
    {
        Bullet.OnBullet -= OnBullet;
    }

    private void OnBullet(GameObject enemy, string fraction, float damage)
    {
        if (enemy == gameObject && fraction != this.fraction)
        {
            health = health - damage * (1.0f - armor / maxArmor);
            armor -= damage;

            armor = Mathf.Clamp(armor, 0, maxArmor);

            if (health > 0) healthLine.fillAmount = health / maxHealth; else healthLine.fillAmount = 0;
            if (armor > 0) armorLine.fillAmount = armor / maxArmor; else armorLine.fillAmount = 0;
        }

        if (health > 0) return;

        Die();
    }

    private void Die()
    {
        gameController.SetScore(maxHealth + maxArmor);

        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
        OnEnemyDie();
    }

    private void OpenFire()
    {
        if (guns.Length == 0) return;


        if (bulletReloadTime + bulletReloadPeriod < Time.time)
        {
            bulletReloadTime = Time.time;

            RaycastHit hit;

            if (Physics.Linecast(myTransform.position, player.position, out hit))
            {
                //if (hit.transform != player && hit.collider.name.Contains("Rabber")) return;
                if (hit.transform != player && !hit.collider.CompareTag("CanDestroy")) return;
            }



            gunSound.Play();

            foreach (GameObject gun in guns)
            {
                if (gun == null || !gun.activeInHierarchy) continue;

                for (int barrelID = 0; barrelID < gun.transform.childCount; barrelID++)
                {
                    Transform barrel = gun.transform.GetChild(barrelID);

                    Vector3 bulletPosition = barrel.position;
                    Vector3 bulletRotation = new Vector3(-90, barrel.eulerAngles.y, 0);

                    Instantiate(bullet, bulletPosition, Quaternion.Euler(bulletRotation)).GetComponent<Bullet>().Set(fraction, damage);


                }
            }

            
        }
    }
}
