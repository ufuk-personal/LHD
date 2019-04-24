using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BezierSolution;
using UnityEngine.Animations;

public class CameraController : MonoBehaviour
{
    public BezierWalkerWithSpeed cameraWalker;
    public List<CameraPath> allPaths;
    public Dictionary<int, CameraPath> pathCache;
    public LookAtConstraint cameraLookAt;
    public float cameraManuelMovementSpeed = 15f;
    public float switchSpeed = 5f;
    private int currentPathID = 0;
    private bool manualControlMode = false;
    public static CameraController Instance = null;
    // Use this for initialization
    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize() {
        pathCache = new Dictionary<int, CameraPath>();
        foreach(CameraPath path in allPaths)
        {
            pathCache.Add(path.PathID, path);
        }
        SwitchToPath(0);
    }

    public Vector3 CameraManualStartPosition;
    public Vector3 CameraManualStartRotation;

    public void ActivateManualControl() {
        manualControlMode = true;
        this.cameraWalker.enabled = false;
        this.cameraLookAt.enabled = false;
        readyToMove = false;
        StopAllCoroutines();
        StartCoroutine(MoveCameraManuallyToTarget());
        cameraWalker.transform.rotation = Quaternion.Euler(CameraManualStartRotation);
    }

    public void DeactivateManualControl() {
        manualControlMode = false;
        this.cameraWalker.enabled = true;
        this.cameraLookAt.enabled = true;
        StopAllCoroutines();
    }
    public GameObject cameraHandle;
    public float pcCameraRotationSpeed = 30f;
    public float pcCameraZoomSpeed = 10f;

    public void Update()
    {
        if (readyToMove)
        {
            if (manualControlMode && Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
            {
                if (Input.touchCount < 2)
                {
                    Vector3 displacement = new Vector3(-Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y"));
                    Quaternion displacementRotation = Quaternion.Euler(0, cameraWalker.transform.rotation.eulerAngles.y, 0);
                    displacement = displacementRotation * displacement;
                    displacement.Normalize();
                    displacement = displacement * Time.deltaTime * cameraManuelMovementSpeed * (Camera.main.fieldOfView / 20f);
                    this.cameraWalker.transform.Translate(displacement, Space.World);
                }
            }
            if (Input.GetKey(KeyCode.A))
            {
                this.cameraHandle.transform.Rotate(new Vector3(0, pcCameraRotationSpeed * Time.deltaTime, 0), Space.World);
            }
            if (Input.GetKey(KeyCode.D))
            {
                this.cameraHandle.transform.Rotate(new Vector3(0, -pcCameraRotationSpeed * Time.deltaTime, 0), Space.World);
            }
        }

        
        if (Input.GetKey(KeyCode.W))
        {
            Camera.main.fieldOfView -= Time.deltaTime * pcCameraZoomSpeed;
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 1, 90);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Camera.main.fieldOfView += Time.deltaTime * pcCameraZoomSpeed;
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 1, 90);
        }
    }

    public bool readyToMove = false;

    public IEnumerator MoveCameraManuallyToTarget()
    {
        yield return new WaitForEndOfFrame();
        cameraWalker.transform.position = Vector3.Lerp(cameraWalker.transform.position, CameraManualStartPosition, 0.1f);
        if(Vector3.Distance(cameraWalker.transform.position, CameraManualStartPosition) < 0.1f){
            cameraWalker.transform.position = CameraManualStartPosition;
            readyToMove = true;
            yield return null;
        }
        else
        {
            yield return MoveCameraManuallyToTarget();
        }
    }

    public void SwitchToPath(int pathID)
    {
        DeactivateManualControl();
        if (this.currentPathID != pathID)
        {
            this.currentPathID = pathID;

            cameraWalker.enabled = false;
            StopAllCoroutines();

            ConstraintSource source = new ConstraintSource();
            source.weight = 0;
            source.sourceTransform = pathCache[this.currentPathID].sourceTransform;

            if(cameraLookAt.sourceCount == 1)
            {
                cameraLookAt.AddSource(source);
            }
            else if (cameraLookAt.sourceCount == 2)
            {
                cameraLookAt.SetSource(1, source);
            }
            float distance = Vector3.Distance(cameraWalker.transform.position, pathCache[pathID].GetStartPosition());
            StartCoroutine(MoveCameraToTarget(pathCache[pathID].GetStartPosition(), distance));
        }
    }

    private void RestartWalkingOnPath() {
        cameraWalker.NormalizedT = 0f;
        cameraWalker.travelMode = pathCache[this.currentPathID].travalMode;
        cameraWalker.speed = pathCache[this.currentPathID].speed;
        cameraWalker.spline = pathCache[this.currentPathID].Spline();
        ConstraintSource source = new ConstraintSource();
        source.weight = 1;
        source.sourceTransform = pathCache[this.currentPathID].sourceTransform;
        cameraLookAt.SetSource(0, source);
        cameraLookAt.RemoveSource(1);
        cameraWalker.enabled = true;
    }

    public IEnumerator MoveCameraToTarget(Vector3 targetPos, float distance) {

        float currentDistance = Vector3.Distance(cameraWalker.transform.position, targetPos);

        cameraWalker.transform.position = Vector3.Lerp(cameraWalker.transform.position, targetPos, ( (Time.deltaTime * switchSpeed) / currentDistance));

       

        ConstraintSource s1 = cameraLookAt.GetSource(0);
        s1.weight = currentDistance / distance;
        cameraLookAt.SetSource(0, s1);

        ConstraintSource s2 = cameraLookAt.GetSource(1);
        s2.weight = 1 - ( currentDistance / distance);
        cameraLookAt.SetSource(1, s2);

        if (currentDistance < 0.1f) {
            cameraWalker.transform.position = targetPos;
            RestartWalkingOnPath();
            yield return null;
        }
        else
        {
            yield return new WaitForEndOfFrame();
            yield return MoveCameraToTarget(targetPos, distance);
        }
        yield return null;
    }
}
