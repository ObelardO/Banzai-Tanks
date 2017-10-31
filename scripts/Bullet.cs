using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float speed = 7.0f;
	public float lifeTime = 10.0f;
    public string fraction;
    public float damage;

    public GameObject sparks;

	private float startTime;

	private Transform myTransform;

    public delegate void BulletEvent(GameObject enemy, string fraction, float damage);
    public static event BulletEvent OnBullet;

    public void Set(string fraction, float damage)
    {
        this.fraction = fraction;
        this.damage = damage;
    }

    // Use this for initialization
    void Start ()
    {
		Destroy (gameObject, lifeTime);

        myTransform = transform;

		startTime = Time.time;
	}

    void OnTriggerEnter(Collider other)
    {
        float Radius = 1.0f;// explosion radius
        float Force = 70.0f;// explosion forse

        Collider[] hitColliders = Physics.OverlapSphere(myTransform.position, Radius);// create explosion
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].CompareTag("CanDestroy"))// if tag CanBeRigidbody
            {
                if (!hitColliders[i].GetComponent<Rigidbody>())
                {
                    hitColliders[i].gameObject.AddComponent<Rigidbody>();
                }
                hitColliders[i].GetComponent<Rigidbody>().AddExplosionForce(Force, transform.position, Radius, 0.0F); // push game object
            }

        }

        Destroy(Instantiate(sparks, myTransform.position, Quaternion.Euler(new Vector3(myTransform.rotation.eulerAngles.x - 90,
                                                                                       myTransform.rotation.eulerAngles.y,
                                                                                       myTransform.rotation.eulerAngles.z))), 1);

        if (other.name.Contains("Enemy") || other.name.Contains("Player")) OnBullet(other.gameObject, fraction, damage);

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update ()
    {
		myTransform.Translate (Vector3.down * speed * Time.deltaTime);
	}
}
