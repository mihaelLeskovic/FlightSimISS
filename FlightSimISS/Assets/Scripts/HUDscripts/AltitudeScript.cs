using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LatitudeScript : MonoBehaviour
{
    [SerializeField] private Transform planeTransform;

    private Text text;
    void Start() {
        text = GetComponent<Text>();
    }

    void Update() {
        text.text = planeTransform.localPosition.y.ToString();
    }
}
