using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BunkerRocket : MonoBehaviour
{
    public GameObject explosionPrefab;

    private void Start()
    {
        Destroy(gameObject, 30f);
    }

    void Update()
    {
        transform.position += (transform.up).normalized * 100f * Time.deltaTime;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("terrain"))
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            var collider = GetComponent<BoxCollider>();
            collider.enabled = false;
            collider.size = Vector3.zero;
            GetComponent<MeshRenderer>().enabled = false;
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("rafale"))
        {
            Instantiate(explosionPrefab, transform.position + Vector3.up * 25f, Quaternion.identity);
            var collider = GetComponent<BoxCollider>();
            collider.enabled = false;
            collider.size = Vector3.zero;
            GetComponent<MeshRenderer>().enabled = false;
            Destroy(gameObject);

            other.gameObject.GetComponent<PlaneController>().endGame();
        }
    }
}
