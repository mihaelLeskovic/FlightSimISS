using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VelocityScript : MonoBehaviour {
    [SerializeField] public Transform planeTransform;

    private Rigidbody rb;
    private Text text;
    void Start() {
        text = GetComponent<Text>();
        rb = planeTransform.GetComponent<Rigidbody>();
    }

    void Update() {
        text.text = rb.velocity.magnitude.ToString();
    }
}
