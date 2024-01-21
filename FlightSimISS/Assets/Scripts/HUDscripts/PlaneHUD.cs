using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneHUD : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform hudCenter;
    [SerializeField] public Camera camera;
    [SerializeField] public Transform planeTransform;

    private GameObject hudCenterGO;
    
    void Start() {
        hudCenterGO = hudCenter.gameObject;
        
    }

    // Update is called once per frame
    void Update() {
        centerHUDCenter();
    }

    Vector3 WorldToHUDCoords(Vector3 worldCoords) {
        return camera.WorldToScreenPoint(worldCoords) - new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2);
    }

    void centerHUDCenter() {
        Vector3 rotation = camera.transform.localEulerAngles;
        Vector3 hudPos = WorldToHUDCoords(camera.transform.position + planeTransform.forward);
        hudPos.z = 0;
        hudPos.y = 0;
        hudCenter.localPosition = hudPos;
        hudCenter.localEulerAngles = new Vector3(0, 0, -rotation.z);
    }
}
