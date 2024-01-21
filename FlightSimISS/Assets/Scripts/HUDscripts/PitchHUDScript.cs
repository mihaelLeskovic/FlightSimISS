using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PitchUI : MonoBehaviour {
    [SerializeField] private GameObject horizonIndicatorPrefab;
    [SerializeField] private GameObject regularBarPrefab;
    [SerializeField] public Transform planeTransform;
    [SerializeField] public Camera camera;
    [SerializeField] private int barDistance;
    [SerializeField] private int max;
    [SerializeField] private int maxBarHeight;

    private GameObject horizonGO;
    private RectTransform horizonTransform;

    private Dictionary<int, GameObject> barGOs;
    private Dictionary<int, RectTransform> barTransforms;
    
    // Start is called before the first frame update
    void Start() {

        barGOs = new Dictionary<int, GameObject>();
        barTransforms = new Dictionary<int, RectTransform>();
        
        for (int i = -max; i <= max; i += barDistance) {
            if (i == 0) {
                CreateBar(i, horizonIndicatorPrefab);
            }else CreateBar(i, regularBarPrefab);
        }
    }

    // Update is called once per frame
    void Update() {
        float pitch = -planeTransform.eulerAngles.x;
        float roll = planeTransform.eulerAngles.z;

        transform.localEulerAngles = new Vector3(0, 0, -roll);
        

        for (int barAngle = -max; barAngle <= max; barAngle += barDistance) {
            // if(barAngle==0) continue;

            float angle = Mathf.DeltaAngle(pitch, barAngle);
            float position = GetPosition(ConvertAngle(angle));

            if (Mathf.Abs(angle) < 90f && Mathf.Abs(position) < maxBarHeight) {
                Vector3 pos = barTransforms[barAngle].localPosition;
                barTransforms[barAngle].localPosition = new Vector3(pos.x, position, pos.z);
                barGOs[barAngle].SetActive(true);
            }
            else {
                barGOs[barAngle].SetActive(false);
            }
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

    void CreateBar(int angle, GameObject prefab)
    {
        GameObject barGO = Instantiate(prefab, transform);
        RectTransform barTransform = barGO.GetComponent<RectTransform>();

        if(angle!=0) SetNumber(barGO, angle);
        barGOs.Add(angle, barGO);
        barTransforms.Add(angle, barTransform);
    }

    void SetNumber(GameObject barGO, int angle) {
        Text[] texts = barGO.GetComponentsInChildren<Text>();
        foreach(Text text in texts) {
            text.text = angle.ToString();
        }
    }
}
