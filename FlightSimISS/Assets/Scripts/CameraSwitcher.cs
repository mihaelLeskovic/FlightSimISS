using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera noseCamera;
    public Camera cockpitCamera;
    public Camera thirdPersonCamera;
    public Camera rearviewCamera;
    private Camera currentCamera;
    // Start is called before the first frame update
    void Start()
    {
        cockpitCamera.enabled = false;
        noseCamera.enabled = false;
        thirdPersonCamera.enabled = true;
        currentCamera = thirdPersonCamera;
        rearviewCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentCamera = cockpitCamera;
            SwitchCamera(cockpitCamera);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentCamera = noseCamera;
            SwitchCamera(noseCamera);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentCamera = thirdPersonCamera;
            SwitchCamera(thirdPersonCamera);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SwitchCamera(rearviewCamera);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            SwitchCamera(currentCamera);
        }
    }

    void SwitchCamera(Camera newCamera)
    {
        // Disable all cameras
        cockpitCamera.enabled = false;
        noseCamera.enabled = false;
        thirdPersonCamera.enabled = false;

        // Enable the selected camera
        newCamera.enabled = true;
    }
}