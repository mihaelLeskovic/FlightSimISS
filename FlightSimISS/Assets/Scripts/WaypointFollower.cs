using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] GameObject[] waypoints;
    int currentWaypointIndex = 0;
    [SerializeField] float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].transform.position) < .1f)//svaki frame gledamo koliko smo udaljeni od current waypointa,ako ga dotaknemo onda promijenimo na drugi waypoint
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
                currentWaypointIndex = 0;
        }
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].transform.position, speed * Time.deltaTime);//micemo se prema aktivnom waypointu,transform.position je pozicija objekta koji se mice,pomicemo se za 1 game unit po sekundi neovisno koliki je frame rate(funkcija deltatime)

    }
}
