using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RocketLauncher : MonoBehaviour
{

    public List<GameObject> targets;

    public string targetTag = "target";

    private void Start()
    {
        // Find all GameObjects with the specified tag and add them to the list
        GameObject[] targetArray = GameObject.FindGameObjectsWithTag(targetTag);
        targets.AddRange(targetArray);
    }

    public GameObject rocketPrefab;
    public GameObject rafale;
    public GameObject smokeTrailPrefab;
    public float speed = 1f;

    public GameObject explosionPrefab;
    public AudioClip explosionSound;
    public AudioClip blastSound;



    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Collider[] hitColliders = Physics.OverlapBox(rafale.transform.position + rafale.transform.forward * 100, Vector3.one * 200, rafale.transform.rotation);
            //transform.localScale / 2, Quaternion.identity);
            //RaycastHit hit;
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //if (Physics.Raycast(ray, out hit))
            //{
            //    GameObject targetHit = hit.collider.gameObject;
            //    // Check if targets list is not null and contains the targetHit
            //    if (targets != null && targets.Count > 0 && targets.Contains(targetHit))
            //    {
            if (hitColliders.Length > 0)
            {
                var minDist = float.MaxValue;
                GameObject target = null;
                foreach (Collider collider in hitColliders)
                {
                    if (targets.Contains(collider.gameObject))
                    {
                        if (Vector3.Distance(rafale.transform.position, collider.transform.position) < minDist)
                        {
                            minDist = Vector3.Distance(rafale.transform.position, collider.transform.position);
                            target = collider.gameObject;
                        }
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

    public IEnumerator SendHoming(GameObject rocket, GameObject smokeTrail, GameObject target)
    {
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
        }

        // Destroy the rocket and smoke trail whether it hit the target or not
        Destroy(rocket);
        Destroy(smokeTrail);

        // Delay for 1 second
        yield return new WaitForSeconds(1f);

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
