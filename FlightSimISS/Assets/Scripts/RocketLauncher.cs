using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RocketLauncher : MonoBehaviour
{
    public RocketLauncher next;

    public static List<GameObject> targets = new List<GameObject>();
    public string targetTag = "target";

    public GameObject rocketPrefab;
    public GameObject rafale;
    public GameObject smokeTrailPrefab;
    public float speed = 1f;

    public GameObject explosionPrefab;
    public AudioClip explosionSound;
    public AudioClip blastSound;

    public GameObject customSmokePrefab; // Add this variable for your custom smoke effect

    bool used = false;

    private void Start()
    {
        // Find all GameObjects with the specified tag and add them to the list
        //GameObject[] targetArray = GameObject.FindGameObjectsWithTag(targetTag);
        //targets.AddRange(targetArray);
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Tab)) && !used)
        {
            // Find the closest target in the camera view
            used = true;

            GameObject closestTarget = GetClosestTargetInCameraView();
            if (closestTarget != null)
            {
                LaunchRocket(closestTarget);
            }
            else
            {
                LaunchRocketLinear();
            }
            GameManager.rocketsLeft--;
            GetComponent<MeshRenderer>().enabled = false;
            if (next != null)
                next.enabled = true;
        }
    }

    private GameObject GetClosestTargetInCameraView()
    {
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        // Get the camera position and forward direction
        Vector3 cameraPosition = rafale.transform.position;
        Vector3 cameraForward = -rafale.transform.right;

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

                    if (distance < closestDistance && distance < 600f)
                    {
                        closestTarget = target;
                        closestDistance = distance;
                    }
                }
            }
        }

        return closestTarget;
    }

    private void LaunchRocket(GameObject target)
    {
        GameObject rocket = Instantiate(rocketPrefab, transform.position, Quaternion.identity);
        rocket.GetComponent<RocketLauncher>().enabled = false;
        var rocketComponent = rocket.AddComponent<Rocket>();
        rocketComponent.explosionPrefab = explosionPrefab;

        GameObject smokeTrail = Instantiate(smokeTrailPrefab, rocket.transform.position, Quaternion.identity);

        rocketComponent.smokeTrail = smokeTrail;

        StartCoroutine(SendHoming(rocket, smokeTrail, target));

        // Play the blast sound at the spawn point of the rocket
        if (blastSound != null)
        {
            AudioSource.PlayClipAtPoint(blastSound, rafale.transform.position);
        }
    }

    void LaunchRocketLinear()
    {
        GameObject rocket = Instantiate(rocketPrefab, transform.position, Quaternion.identity);
        rocket.GetComponent<RocketLauncher>().enabled = false;
        var rocketComponent = rocket.AddComponent<Rocket>();
        rocketComponent.explosionPrefab = explosionPrefab;

        GameObject smokeTrail = Instantiate(smokeTrailPrefab, rocket.transform.position, Quaternion.identity);

        rocketComponent.smokeTrail = smokeTrail;

        rocket.transform.rotation = Quaternion.LookRotation(-rafale.transform.right);
        rocket.transform.Rotate(new Vector3(90f, 0, 0));

        StartCoroutine(SendLinear(rocket, smokeTrail));

        // Play the blast sound at the spawn point of the rocket
        if (blastSound != null)
        {
            AudioSource.PlayClipAtPoint(blastSound, rafale.transform.position);
        }
    }

    IEnumerator SendLinear(GameObject rocket, GameObject smokeTrail)
    {
        // Play the blast sound at the spawn point of the rocket
        if (blastSound != null)
        {
            AudioSource.PlayClipAtPoint(blastSound, rafale.transform.position);
        }

        var timeElapsed = 0f;

        while (timeElapsed < 10f)
        {
            timeElapsed += Time.deltaTime;
            rocket.transform.position += (rocket.transform.up).normalized * speed * Time.deltaTime;

            if (smokeTrail != null)
            {
                smokeTrail.transform.position = rocket.transform.position;
            }

            yield return null;
        }

        Destroy(rocket);
        Destroy(smokeTrail);
    }

    private IEnumerator SendHoming(GameObject rocket, GameObject smokeTrail, GameObject target)
    {
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();

        while (Vector3.Distance(target.transform.position, rocket.transform.position) > 0.3f)
        {
            if (targets.Contains(target))
            {
                rocket.transform.position += (target.transform.position - rocket.transform.position).normalized * speed * Time.deltaTime;
                Vector3 rotation = (target.transform.position - transform.position).normalized;
                rocket.transform.rotation = Quaternion.LookRotation(rotation);
                rocket.transform.Rotate(new Vector3(90f, 0, 0));

                if (smokeTrail != null)
                {
                    smokeTrail.transform.position = rocket.transform.position;
                }
                yield return null;
                continue;
            }

            break;           
        }

        if (!targets.Contains(target))
        {
            var timeElapsed = 0f;

            while (timeElapsed < 10f)
            {
                timeElapsed += Time.deltaTime;
                rocket.transform.position += (rocket.transform.up).normalized * speed * Time.deltaTime;

                if (smokeTrail != null)
                {
                    smokeTrail.transform.position = rocket.transform.position;
                }

                yield return null;
            }

            Destroy(rocket);
            Destroy(smokeTrail);
        }
        else
        {
            Bounds rocketBounds = rocket.GetComponent<Collider>().bounds;
            Bounds targetBounds = target.GetComponent<Collider>().bounds;

            if (rocketBounds.Intersects(targetBounds))
            {
                // The rocket hit the target, trigger explosion
                TriggerExplosion(rocket.transform.position);

                // Make the target smoke and fall to the ground with custom smoke effect
                StartSmokingAndFalling(target, smokeTrail, rocket.transform.position, rocket.transform.up);
            }

            // Destroy the rocket and smoke trail whether it hit the target or not
            Destroy(rocket);
            Destroy(smokeTrail);
        }
    }

    private void StartSmokingAndFalling(GameObject target, GameObject smokeTrail, Vector3 rocketPosition, Vector3 rocketForward)
    {
        // Attach your custom smoke particle system to the target
        GameObject customSmoke = Instantiate(customSmokePrefab, target.transform.position, Quaternion.identity);
        customSmoke.transform.parent = target.transform; // Set the target as the parent to follow its position
        customSmoke.transform.localScale = Vector3.one;

        // Add a Rigidbody component to enable falling
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        targetRigidbody.useGravity = true;

        targets.Remove(target);

        // Optionally, you may need to adjust other Rigidbody settings for realistic falling

        //targetRigidbody.AddForce((forceDirection) * 100000, ForceMode.Impulse);
        //targetRigidbody.AddExplosionForce(1000000f, rocketPosition, 0f);
        targetRigidbody.velocity = -target.transform.forward * 80f;
        targetRigidbody.AddForceAtPosition(rocketForward * 20000f, rocketPosition, ForceMode.Impulse);
        
        // ...

        // Destroy the smoke trail as it's no longer needed
        Destroy(smokeTrail);

        // Delay for some time before destroying the target
        //StartCoroutine(DestroyTargetDelayed(target, customSmoke));
        if (target.transform.parent.parent != null)
        {
            target.transform.parent = target.transform.parent.parent;
            var isJet = target.TryGetComponent<EnemyMovement>(out var enemyMovement);
            if (isJet) enemyMovement.enabled = false;
            var isRapier = target.TryGetComponent<Bunker>(out var bunker);
            if (isRapier)
            {
                bunker.enabled = false;
                bunker.StopAllCoroutines();
            }
        }
        else
        {
            StartCoroutine(DestroyTargetDelayed(target, customSmoke));
        }
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
