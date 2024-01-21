using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bunker : MonoBehaviour
{
    [SerializeField] GameObject bunkerRockets;
    [SerializeField] GameObject bunkerRocketPods;
    [SerializeField] GameObject rocketPrefab;

    GameObject rafale;
    int tempCnt = 0;

    // Start is called before the first frame update
    void Start()
    {
        rafale = GameObject.FindWithTag("rafale");

        StartCoroutine(BunkerFireWait());
    }

    // Update is called once per frame
    void Update()
    {

        bunkerRockets.transform.rotation = Quaternion.LookRotation(-(rafale.transform.position - this.transform.position).normalized);
        //bunkerRockets.transform.RotateAround(transform.position, Vector3.right, 180f);
        //var rot = bunkerRockets.transform.rotation.eulerAngles;
        //rot.z = 0f;
        //bunkerRockets.transform.rotation = Quaternion.Euler(rot + Vector3.up * 180f);

        bunkerRocketPods.transform.rotation = Quaternion.LookRotation(-(rafale.transform.position - this.transform.position).normalized);

        //bunkerRocketPods.transform.RotateAround(transform.position, Vector3.right, 180f);
        //var rotPods = bunkerRockets.transform.rotation.eulerAngles;
        //rotPods.z = 0f;
        //bunkerRocketPods.transform.rotation = Quaternion.Euler(rot + Vector3.up * 180f);     

    }

    private void FixedUpdate()
    {
        if (tempCnt <= 50)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 200f, Vector3.down, out var hit, Mathf.Infinity))
            {
                var position = transform.position;
                position.y = hit.point.y;
                transform.position = position;
                tempCnt++;
            }
        }
    }

    IEnumerator BunkerFireWait()
    {
        var random = UnityEngine.Random.Range(0f, 2f);
        yield return new WaitForSeconds(10f + random);
        StartCoroutine(BunkerFireLoop());
    }

    IEnumerator BunkerFireLoop()
    {
        var rocket = Instantiate(rocketPrefab, transform.position, transform.rotation);
        rocket.SetActive(true);

        Vector3 rotation = (rafale.transform.position - transform.position).normalized;
        rocket.transform.rotation = Quaternion.LookRotation(rotation);
        rocket.transform.Rotate(new Vector3(90f, 0, 0));
        rocket.transform.position += Vector3.up * 5f;

        var random = UnityEngine.Random.Range(0f, 3f);
        yield return new WaitForSeconds(8f + random);
        StartCoroutine(BunkerFireLoop());
    }
}
