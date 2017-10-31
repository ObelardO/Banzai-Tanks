using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ItemSpawner : MonoBehaviour {

    private Transform myTransform;

    public GameObject[] items;
    public bool incrementMode = false;

    private int itemID;

    public delegate void ItemEvent(GameObject entity, GameObject item);
    public static event ItemEvent OnItem;

    private ParticleSystem particle;
    private CapsuleCollider myCollider;

    public AudioSource itemPickSound;
    public AudioSource itemSpawnerSound;

    private void OnEnable()
    {
        LevelSettings.OnWaveComplite += OnWaveComplite;
    }

    private void OnDisable()
    {
        LevelSettings.OnWaveComplite -= OnWaveComplite;
    }


    public void OnWaveComplite()
    {
        if (items[itemID].activeSelf || items.Length == 0 || (itemID == items.Length - 1 && incrementMode)) return;

        items[itemID].SetActive(false);

        if (incrementMode && itemID < items.Length - 1) itemID++;

        items[itemID].SetActive(true);
        myCollider.enabled = true;
        particle.Play();
        itemSpawnerSound.Play();
    }

    void OnTriggerEnter(Collider other)
    {
        if (items[itemID] == null || !other.name.Contains("Player")) return;

        if (!items[itemID].activeSelf) return;

        OnItem(other.gameObject, items[itemID]);
        items[itemID].SetActive(false);
        myCollider.enabled = false;
        particle.Stop();

        itemPickSound.Play();
        itemSpawnerSound.Stop();
    }

    // Use this for initialization
    void Start ()
    {
        myTransform = transform;

        particle = GetComponent<ParticleSystem>();

        myCollider = GetComponent<CapsuleCollider>();

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null) continue;
       
            items[i] = Instantiate(items[i], myTransform.position, Quaternion.Euler(new Vector3(-35, 0, 0)));
            if (i > 0) items[i].SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {

        if (items[itemID] != null)
        {
            items[itemID].transform.Rotate(0, 160.0f * Time.deltaTime, 0,Space.World);

            items[itemID].transform.position = new Vector3(myTransform.position.x, 
                1.0f + Mathf.Cos(Time.time * 2.0f) * 0.2f, myTransform.position.z);
        }

    }
}
