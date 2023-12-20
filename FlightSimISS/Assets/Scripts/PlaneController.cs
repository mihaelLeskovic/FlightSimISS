using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaneController : MonoBehaviour {
    [Header("Plane stats")]
    [Tooltip("How much the throttle increases/decreases per update when pressed")]
    public float throttleIncrement = 0.1f;
    
    [Tooltip("Maximum engine thrust when at 100% throttle")]
    public float maxThrust = 200f;
    // rafale ima 50kN po pogonu, 75kN po pogonu s afterburnerom

    [Tooltip("How responsive the plane is when turning")]
    public float responsiveness = 10f;
    
    public float pitchResponsiveness = 10f;

    public float rollResponsiveness = 10f;

    public float yawResponsiveness = 10f;
    
    public float angularDamping = 1f; // Adjust the value as needed
    
    public float throttle = 0f;

    public float lift = 135f;

    public float velocity = 0;
    
    private float roll;
    private float pitch;
    private float yaw;

    private Rigidbody rb;

    private float responseModifier {
        get {
            return (rb.mass / 10f) * responsiveness;
        }
    }

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void HandleInputs() {
        roll = Input.GetAxis("Roll");
        pitch = Input.GetAxis("Pitch");
        yaw = Input.GetAxis("Yaw");

        if (Input.GetKey(KeyCode.Space)) throttle += throttleIncrement;
        if (Input.GetKey(KeyCode.LeftControl)) throttle -= throttleIncrement;
        throttle = Mathf.Clamp(throttle, 0f, 100f);
        
    }

    private void Update() {
        HandleInputs();
    }

    private void FixedUpdate() {
        // throttle
        rb.AddForce(transform.forward * (maxThrust * throttle));
        
        rb.AddForce(transform.up * rb.velocity.magnitude * lift);

        velocity = rb.velocity.magnitude;
        
        // rotation
        rb.AddTorque(transform.up * (yaw * yawResponsiveness));
        rb.AddTorque(transform.right * (pitch * pitchResponsiveness));
        rb.AddTorque(-transform.forward * (roll * rollResponsiveness));
        
        // rb.angularVelocity *= (1f - Time.deltaTime * angularDamping);
        
    }
}
