using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneFall : MonoBehaviour
{
    Rigidbody _rigidbody;
    public GameObject explosionPrefab;
    bool crashed = false;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_rigidbody.useGravity)
        {
            _rigidbody.AddForce(_rigidbody.mass * Vector3.down * 5f * Time.deltaTime, ForceMode.Acceleration);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("terrain") && !crashed)
        {
            var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * 5f;
            crashed = true;

            _rigidbody.velocity = _rigidbody.velocity / 10f;
        }
    }
}
