using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public GameObject explosionPrefab;
    public GameObject smokeTrail;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("terrain"))
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            var collider = GetComponent<BoxCollider>();
            collider.enabled = false;
            collider.size = Vector3.zero;
            GetComponent<MeshRenderer>().enabled = false;
            smokeTrail.SetActive(false);
            Destroy(gameObject, 12f);
        }
    }
}
