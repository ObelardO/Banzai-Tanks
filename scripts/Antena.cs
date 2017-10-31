using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antena : MonoBehaviour {

    public GameObject anchor;

    Transform myTransform;
    Transform anchorTransform;

    // Use this for initialization
    void Start () {
        myTransform = transform;
        anchorTransform = anchor.transform;
    }
	
	// Update is called once per frame
	void Update () {
        anchorTransform.Rotate(0, Mathf.Sin(Time.time), 0);
        myTransform.Rotate(0, 0, Mathf.Cos(Time.time) * 0.5f);
	}
}
