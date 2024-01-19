using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoresightScript : MonoBehaviour {
    [SerializeField] private Transform planeReference;
    [SerializeField] private Transform cameraReference;
    [SerializeField] private Transform hudCenter;
    [SerializeField] private Canvas canvas;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        hudCenter.gameObject.transform.position = planeReference.position + planeReference.forward * 5f;
    }

    void UpdateBoresight() {
        
    }
}
