using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualControlButton : MonoBehaviour {

    public void OnManualControlActivated()
    {
        CameraController.Instance.ActivateManualControl();
    }
}
