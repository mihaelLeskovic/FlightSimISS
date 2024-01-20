using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject enemyContainer;
    [SerializeField] GameObject endCanvas;
    [SerializeField] GameObject warningPanel;

    public GameObject rafale;
    float timer = 10f;

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
        if (warningActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                //fail scenario
                endCanvas.SetActive(true);
                Time.timeScale = 0f;
                warningPanel.SetActive(false);
            }
            else
            {
                warningPanel.SetActive(true);
            }
        } 
        else
        {
            timer = 10f;
            warningPanel.SetActive(false);
        }
    }

    IEnumerator startEndSequence()
    {
        yield return new WaitForSeconds(6);
        endCanvas.SetActive(true);
    }
}
