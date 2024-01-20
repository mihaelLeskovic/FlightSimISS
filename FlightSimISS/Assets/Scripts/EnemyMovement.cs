using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    float yRotation = 0;
    // Start is called before the first frame update
    void Start()
    {
        yRotation = transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        transform.Rotate(0, 0, 30);
        transform.position += (-transform.forward) * Time.deltaTime * 80f;
        yRotation += 30 * Time.deltaTime;
    }
}
