using System.Collections.Generic;
using UnityEngine;

public class TargetPopulator : MonoBehaviour
{
    private void Start()
    {
        RocketLauncher.targets.Add(gameObject);
    }
}
