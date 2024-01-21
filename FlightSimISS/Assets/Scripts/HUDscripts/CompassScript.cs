using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CompassScript : MonoBehaviour {
    [SerializeField] int smallColumnDistance;
    [SerializeField] private GameObject cardinalColumnPrefab;
    [SerializeField] private GameObject smallColumnPrefab;
    [SerializeField] private Transform planeTransform;
    [SerializeField] private Camera camera;
    
    string[] directions = {
        "N",
        "NE",
        "E",
        "SE",
        "S",
        "SW",
        "W",
        "NW"
    };

    private RectTransform transform;
    private Dictionary<int, GameObject> columnGOs;
    private Dictionary<int, RectTransform> columnTransforms;
    
    void Start() {
        columnGOs = new Dictionary<int, GameObject>();
        columnTransforms = new Dictionary<int, RectTransform>();
        transform = GetComponent<RectTransform>();
        for (int i = 0; i < 360; i+=smallColumnDistance) {
            if (i % 45 == 0) {
                MakeCardinalColumn(i);
            }
            else {
                MakeSmallColumn(i);
            }
        }
    }

    void LateUpdate() {
        float yaw = planeTransform.eulerAngles.y;

        foreach (var kvp in columnTransforms) {
            int columnAngle = kvp.Key;
            RectTransform columnTransform = kvp.Value;

            float angle = Mathf.DeltaAngle(yaw, columnAngle);
            float position = GetPosition(ConvertAngle(angle));

            if (Mathf.Abs(angle) < 90f && position >= transform.rect.xMin && position <= transform.rect.xMax) {
                Vector3 pos = columnTransform.localPosition;
                columnTransform.localPosition = new Vector3(position, pos.y, pos.z);
                columnGOs[columnAngle].SetActive(true);
            }
            else {
                columnGOs[columnAngle].SetActive(false);
            }
        }
        
        // for (int columnAngle = 0; columnAngle < 360; columnAngle += smallColumnDistance) {
        //     float angle = Mathf.DeltaAngle(yaw, columnAngle);
        //     float position = GetPosition(ConvertAngle(angle));
        //
        //     if (Mathf.Abs(angle) < 90f
        //         && position >= transform.rect.xMin && position <= transform.rect.xMax
        //         ) {
        //         Vector3 pos = columnTransforms[columnAngle].localPosition;
        //         columnTransforms[columnAngle].localPosition = new Vector3(position, pos.y, pos.z);
        //         columnGOs[columnAngle].SetActive(true);
        //     }
        //     else {
        //         columnGOs[columnAngle].SetActive(false);
        //     }
        //     columnGOs[columnAngle].SetActive(false);
        // }
    }
    
    float ConvertAngle(float angle) {
        //convert 0 - 360 range to -180 - 180
        if (angle > 180) {
            angle -= 360f;
        }

        return angle;
    }

    float GetPosition(float angle) {
        float fov = camera.fieldOfView;
        return TransformAngle(angle, fov, camera.pixelHeight);
    }
    
    float TransformAngle(float angle, float fov, float pixelHeight) {
        return (Mathf.Tan(angle * Mathf.Deg2Rad) / Mathf.Tan(fov / 2 * Mathf.Deg2Rad)) * pixelHeight / 2;
    }

    void MakeCardinalColumn(int angle) {
        GameObject columnGO = Instantiate(cardinalColumnPrefab, transform);
        RectTransform columnTransform = columnGO.GetComponent<RectTransform>();

        Text text = columnGO.GetComponentInChildren<Text>();
        text.text = directions[angle / 45];
        
        columnGOs.Add(angle,columnGO);
        columnTransforms.Add(angle, columnTransform);
    }


    void MakeSmallColumn(int angle) {
        GameObject columnGO = Instantiate(smallColumnPrefab, transform);
        RectTransform columnTransform = columnGO.GetComponent<RectTransform>();
        
        columnGOs.Add(angle,columnGO);
        columnTransforms.Add(angle, columnTransform);
    }
}
