using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public GameObject door;

	public Transform closed;
	public Transform opened;

    public bool isOpened = false;


    private enum doorStates {
		doorClosed,
		doorOpening,
		doorOpened,
		doorClosing
	}

	private doorStates doorState;

	// Use this for initialization
	void Start () {
		doorState = doorStates.doorClosed;
	}
	
	// Update is called once per frame
	void Update ()
    {
		UpdateDoorState ();
	}

	public bool SpawnEnemy (GameObject enemy)
    {
        if (doorState != doorStates.doorClosed) return false;

        Instantiate(enemy, transform.position, transform.rotation).GetComponent<Enemy>().mySpawner = this;
        doorState = doorStates.doorOpening;

        return true;
    }

	void UpdateDoorState()
    {
		switch (doorState)
        {
		    case doorStates.doorClosed:

                break;

		    case doorStates.doorOpening:

			    door.transform.rotation = Quaternion.Lerp (door.transform.rotation, opened.rotation, Time.deltaTime * 1.6f);

			    if (door.transform.rotation == opened.transform.rotation) {
				    doorState = doorStates.doorOpened;
				    isOpened = true;
			    }

			    break;

		    case doorStates.doorOpened:
			
			    if (!isOpened) doorState = doorStates.doorClosing;
			    break;

		    case doorStates.doorClosing:

			    door.transform.rotation = Quaternion.Lerp (door.transform.rotation, closed.rotation, Time.deltaTime * 0.8f);

			    if (door.transform.rotation == closed.rotation) {
				    doorState = doorStates.doorClosed;
			    }

			    break;
		}
	}
}
