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
    public float switchSpeed = 5f;
    private int currentPathID = 0;

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

    public void SwitchToPath(int pathID)
    {
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
