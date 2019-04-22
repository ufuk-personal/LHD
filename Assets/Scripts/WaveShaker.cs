using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveShaker : MonoBehaviour {

    public float xFactor = 1f;
    public float zFactor = 1f;
    public float xSpeed = 1f;
    public float zSpeed = 1f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.rotation = Quaternion.Euler(Mathf.Sin(Time.time * xSpeed) * xFactor, 0, Mathf.Sin(Time.time * zSpeed) * zFactor);
	}
}
