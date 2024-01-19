using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchUI : MonoBehaviour {
    [SerializeField] private GameObject horizonIndicatorPrefab;
    [SerializeField] private Transform planeTransform;
    [SerializeField] private Camera camera;

    private GameObject horizonGO;
    private RectTransform horizonTransform;

    private float maxY = 128;
    
    // Start is called before the first frame update
    void Start() {
        horizonGO = Instantiate(horizonIndicatorPrefab, transform);
        horizonTransform = horizonGO.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update() {
        float pitch = -planeTransform.eulerAngles.x;
        float roll = planeTransform.eulerAngles.z;

        transform.localEulerAngles = new Vector3(0, 0, -roll);

        float angle = Mathf.DeltaAngle(pitch, 0);
        float position = GetPosition(ConvertAngle(angle));
        
        if (Mathf.Abs(angle) < 90f &&  Mathf.Abs(position) < maxY) {
            //if bar position is within bounds
            Vector3 pos = horizonTransform.localPosition;
            horizonTransform.localPosition = new Vector3(pos.x, position, pos.z);
            horizonGO.SetActive(true);
        
            // if (bar.bar != null) bar.bar.UpdateRoll(roll);
        } else {
            horizonGO.SetActive(false);
        }
    }

    float GetPosition(float angle) {
        float fov = camera.fieldOfView;
        return TransformAngle(angle, fov, camera.pixelHeight);
    }
    
    float TransformAngle(float angle, float fov, float pixelHeight) {
        return (Mathf.Tan(angle * Mathf.Deg2Rad) / Mathf.Tan(fov / 2 * Mathf.Deg2Rad)) * pixelHeight / 2;
    }
    
    float ConvertAngle(float angle) {
        //convert 0 - 360 range to -180 - 180
        if (angle > 180) {
            angle -= 360f;
        }

        return angle;
    }
}
