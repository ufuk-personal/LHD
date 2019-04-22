using UnityEngine;
using System.Collections;
using BezierSolution;

public class CameraPath : MonoBehaviour
{
    public int PathID;
    public BezierWalkerWithSpeed.TravelMode travalMode;
    public float speed;
    public Transform sourceTransform;

    private BezierSpline spline;

    private void Start()
    {
        spline = this.GetComponent<BezierSpline>();
    }

    public Vector3 GetStartPosition()
    {
        return spline.GetPoint(0);
    }

    public BezierSpline Spline() {
        return spline;
    }
}
