using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    public List<GameObject> targets;
    public string targetTag = "target";

    public GameObject rocketPrefab;
    public GameObject rafale;
    public GameObject smokeTrailPrefab;
    public float speed = 1f;

    public GameObject explosionPrefab;
    public AudioClip explosionSound;
    public AudioClip blastSound;

    public GameObject customSmokePrefab; // Add this variable for your custom smoke effect

    private void Start()
    {
        // Find all GameObjects with the specified tag and add them to the list
        GameObject[] targetArray = GameObject.FindGameObjectsWithTag(targetTag);
        targets.AddRange(targetArray);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Find the closest target in the camera view
            GameObject closestTarget = GetClosestTargetInCameraView();

            if (closestTarget != null)
            {
                LaunchRocket(closestTarget);
            }
        }
    }

    private GameObject GetClosestTargetInCameraView()
    {
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        // Get the camera position and forward direction
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;

        foreach (GameObject target in targets)
        {
            if (target != null)
            {
                // Check if the target is in the camera view
                Vector3 toTarget = target.transform.position - cameraPosition;
                float angle = Vector3.Angle(cameraForward, toTarget);

                if (angle < Camera.main.fieldOfView * 0.5f)
                {
                    // Check if the target is closer than the current closest target
                    float distance = Vector3.Distance(cameraPosition, target.transform.position);

                    if (distance < closestDistance)
                    {
                        closestTarget = target;
                        closestDistance = distance;
                    }
                }
                if (target != null)
                {
                    LaunchRocket(target);
                }            
            }
            //    }
            //}
        }

        return closestTarget;
    }

    private void LaunchRocket(GameObject target)
    {
        GameObject rocket = Instantiate(rocketPrefab, transform.position, Quaternion.identity);
        GameObject smokeTrail = Instantiate(smokeTrailPrefab, rocket.transform.position, Quaternion.identity);

        StartCoroutine(SendHoming(rocket, smokeTrail, target));

        // Play the blast sound at the spawn point of the rocket
        if (blastSound != null)
        {
            AudioSource.PlayClipAtPoint(blastSound, rafale.transform.position);
        }
    }

    private IEnumerator SendHoming(GameObject rocket, GameObject smokeTrail, GameObject target)
    {
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();

        while (Vector3.Distance(target.transform.position, rocket.transform.position) > 0.3f)
        {
            rocket.transform.position += (target.transform.position - rocket.transform.position).normalized * speed * Time.deltaTime;
            rocket.transform.LookAt(target.transform);

            if (smokeTrail != null)
            {
                smokeTrail.transform.position = rocket.transform.position;
            }

            yield return null;
        }

        Bounds rocketBounds = rocket.GetComponent<Collider>().bounds;
        Bounds targetBounds = target.GetComponent<Collider>().bounds;

        if (rocketBounds.Intersects(targetBounds))
        {
            // The rocket hit the target, trigger explosion
            TriggerExplosion(rocket.transform.position);

            // Make the target smoke and fall to the ground with custom smoke effect
            StartSmokingAndFalling(target, smokeTrail);
        }

        // Destroy the rocket and smoke trail whether it hit the target or not
        Destroy(rocket);
        Destroy(smokeTrail);
    }

    private void StartSmokingAndFalling(GameObject target, GameObject smokeTrail)
    {
        // Attach your custom smoke particle system to the target
        GameObject customSmoke = Instantiate(customSmokePrefab, target.transform.position, Quaternion.identity);
        customSmoke.transform.parent = target.transform; // Set the target as the parent to follow its position

        // Add a Rigidbody component to enable falling
        Rigidbody targetRigidbody = target.AddComponent<Rigidbody>();
        targetRigidbody.useGravity = true;

        // Optionally, you may need to adjust other Rigidbody settings for realistic falling
        // targetRigidbody.drag = 0.5f;
        // targetRigidbody.angularDrag = 0.5f;
        // ...

        // Destroy the smoke trail as it's no longer needed
        Destroy(smokeTrail);

        // Delay for some time before destroying the target
        StartCoroutine(DestroyTargetDelayed(target, customSmoke));
    }

    private IEnumerator DestroyTargetDelayed(GameObject target, GameObject customSmoke)
    {
        // Adjust the delay time as needed
        yield return new WaitForSeconds(2f);

        // Stop the custom smoke particle system before destroying the target
        customSmoke.GetComponent<ParticleSystem>().Stop();

        // Destroy the target
        Destroy(target);
    }

    private void TriggerExplosion(Vector3 position)
    {
        // Instantiate the explosion effect
        Instantiate(explosionPrefab, position, Quaternion.identity);

        // Play the explosion sound
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, rafale.transform.position);
        }

        Debug.Log("Rocket hit! Triggering explosion.");
    }
}
