using System.Collections;
using UnityEngine;

public class TargetMotion : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 5f;
    public float changeDirectionInterval = 3f;

    private Vector3 targetPosition;

    void Start()
    {
        StartCoroutine(ChangeDirection());
    }

    void Update()
    {
        MoveTowardsTarget();
        RotateTowardsTarget();
    }

    void MoveTowardsTarget()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void RotateTowardsTarget()
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f; // Ignore changes in the y-axis

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    IEnumerator ChangeDirection()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeDirectionInterval);
            SetRandomTargetPosition();
        }
    }

    void SetRandomTargetPosition()
    {
        float randomX = Random.Range(-50f, 50f);
        float randomZ = Random.Range(-50f, 50f);

        targetPosition = new Vector3(randomX, 0f, randomZ);
    }
}
