using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject player;
    public GameObject cameraObject;
    public GameObject cameraTarget;

    public string collisionTag;
    public float collisionSmooth;

	private Transform playerTransform;
	private Transform myTransform;
    private Transform cameraTransform;
    private Transform targetTransform;

    private GameController gameController;

    private bool spectratorMode;

    // Use this for initialization
    void Start ()
    {
		playerTransform = player.transform;
		myTransform = transform;
        cameraTransform = cameraObject.transform;
        targetTransform = cameraTarget.transform;

        gameController = FindObjectOfType<GameController>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (gameController.state != GameController.States.battle) return;

        RaycastHit hit;

        if (Physics.Linecast(myTransform.position, targetTransform.position, out hit))
        {
            if (hit.collider.CompareTag(collisionTag)) cameraTransform.position = Vector3.Lerp(cameraTransform.position, hit.point, Time.deltaTime * collisionSmooth);
        }
        else cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetTransform.position, Time.deltaTime * collisionSmooth * 0.2f);

    }

    private void FixedUpdate()
    {
        switch (gameController.state)
        {
            case GameController.States.gameOver:
            case GameController.States.menu:

                myTransform.position = Vector3.zero;
                myTransform.Rotate(0, 10.0f * Time.fixedDeltaTime, 0);
                cameraTransform.localPosition = new Vector3(0, 10.0f, 15.0f);
                cameraTransform.LookAt(myTransform);

                spectratorMode = true;

                break;
            case GameController.States.battle:

                if (spectratorMode)
                {
                    cameraTransform.localPosition = Vector3.zero;

                    cameraTransform.position = targetTransform.position;
                    cameraTransform.rotation = targetTransform.rotation;

                    spectratorMode = false;
                }

                myTransform.position = Vector3.Lerp(transform.position, player.transform.position, Time.fixedDeltaTime * 6.0f);
                myTransform.rotation = Quaternion.Lerp(transform.rotation, player.transform.rotation, Time.fixedDeltaTime * 3.0f);

                break;
        }
    }
}
