using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject enemyContainer;
    [SerializeField] GameObject endCanvas;
    [SerializeField] GameObject warningPanel;

    public GameObject rafale;

    bool gameEnded = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var enemiesLeft = enemyContainer.transform.childCount;

        if (enemiesLeft == 0)
        {
            if (!gameEnded)
            {
                StartCoroutine(startEndSequence());
                gameEnded = true;
            }
            //end scenario
        }


        var warningActive = Mathf.Abs(rafale.transform.position.x) > 1000f || Mathf.Abs(rafale.transform.position.z) > 1000f;
        warningPanel.SetActive(warningActive);
    }

    IEnumerator startEndSequence()
    {
        yield return new WaitForSeconds(3);
        endCanvas.SetActive(true);
    }
}
