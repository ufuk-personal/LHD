using BezierSolution;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F35_MovementController : MonoBehaviour {

    public ParticleSystem takeOffParticle;
    public ParticleSystem landingParticle;
    
    public Vector3 landingPosition;
    public float landingAngle;
    public float landingSpeed;

    public float takeOffDelay = 0f;
    public bool waitAtStart = false;

    public BezierWalkerWithSpeed bezierWalker;

    // Use this for initialization
    void Start () {
        if (waitAtStart)
        {
            bezierWalker.enabled = false;
            StartCoroutine(TakeOff());
        }
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(Time.time);
	}


    public void StartLanding() {
        bezierWalker.enabled = false;
        takeOffParticle.Stop();
        landingParticle.Play();
        StartCoroutine(Land());
    }

    public IEnumerator TakeOff() {
        yield return new WaitForSeconds(takeOffDelay);
        bezierWalker.enabled = true;
        takeOffParticle.Play();
        yield return null;
    }

    public IEnumerator Land()
    {
        float distance = Vector3.Distance(this.transform.position, this.landingPosition);
        Vector3 landingDirection = this.landingPosition - this.transform.position;
        if(distance <= (landingDirection.normalized.magnitude * landingSpeed * Time.deltaTime))
        {
            this.transform.position = this.landingPosition;
            this.transform.rotation = Quaternion.Euler(0, 270, 0);
            landingParticle.Stop();
            yield return StartCoroutine(TakeOff());
        }
        else
        {
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(landingAngle*Mathf.Min(1, distance / 2.0f), 270, 0), 0.2f);
            this.transform.position = this.transform.position + (landingDirection.normalized * landingSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            yield return StartCoroutine(Land());
        }
    }
}
