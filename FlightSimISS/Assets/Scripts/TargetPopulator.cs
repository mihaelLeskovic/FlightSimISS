using System.Collections.Generic;
using UnityEngine;

public class TargetPopulator : MonoBehaviour
{
    public List<GameObject> targets;
    public string targetTag = "target";

    private void Start()
    {
        // Find all GameObjects with the specified tag and add them to the list
        GameObject[] targetArray = GameObject.FindGameObjectsWithTag(targetTag);
        targets.AddRange(targetArray);
    }
}
