using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScenarioSetup : MonoBehaviour
{
    [SerializeField] Material terrainMat;
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject setupUI;

    [SerializeField] GameObject levelContainer;
    [SerializeField] GameObject enemyContainer;
    [SerializeField] GameObject rafalePrefab;
    [SerializeField] GameObject terrainContainer;
    [SerializeField] GameObject gameManager;

    float hillSize = 0f;

    [SerializeField] GameObject bunkerIcon;
    [SerializeField] Button bunkerButton;
    [SerializeField] GameObject jetIcon;
    [SerializeField] Button jetButton;
    [SerializeField] Button startButton;
    [SerializeField] Text objectsLeftText;
    GameObject selectedIcon;

    [SerializeField] GameObject playableMapObject;
    [SerializeField] GameObject jetPrefab;
    [SerializeField] GameObject bunkerPrefab;
    [SerializeField] GameObject pista;
    [SerializeField] GameObject endCanvas;
    [SerializeField] GameObject hudContainer;
    [SerializeField] GameObject pitchHUD;
    [SerializeField] GameObject velocityHUD;
    [SerializeField] GameObject altitudeHUD;
    [SerializeField] GameObject compassHUD;
    [SerializeField] GameObject boresightHUD;
    [SerializeField] GameObject planeHUD;

    int objectsPlaced = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        startButton.interactable = objectsPlaced != 0;
        jetButton.interactable = selectedIcon != jetIcon;
        bunkerButton.interactable = selectedIcon != bunkerIcon;

        objectsLeftText.text = $"Objects Left: {8 - objectsPlaced}";

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, LayerMask.NameToLayer("UI")))
            {
                //Debug.Log("here");
                if (selectedIcon != null && hit.transform.gameObject == playableMapObject)
                {
                    //Debug.Log("here");
                }
            }
        }
    }

    public void MapClick()
    {
        if (selectedIcon == null) return;
        if (objectsPlaced >= 8) return;

        var newIcon = Instantiate(selectedIcon, playableMapObject.transform);
        var rectTransform = newIcon.GetComponent<RectTransform>();
        rectTransform.position = Input.mousePosition;
        newIcon.SetActive(true);
        var button = newIcon.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            ObjectClick(newIcon);
        });
        objectsPlaced++;
    }

    public void ObjectClick(GameObject target)
    {
        objectsPlaced--;
        Destroy(target);
    }

    public void selectBunker()
    {
        selectedIcon = bunkerIcon;
    }

    public void selectJet()
    {
        selectedIcon = jetIcon;
    }

    public void setHillSize(Single value)
    {
        hillSize = value;
    }

    public void FinishScenarioSetup()
    {
        setupUI.SetActive(false);
        var terrainGen = terrainContainer.AddComponent<TerrainGen>();
        terrainGen.material = terrainMat;
        var intensity = hillSize * 2.1f;
        var scale = hillSize * 1.4f;
        intensity = intensity == 0 ? 1f : intensity;
        scale = scale == 0 ? 1f : scale;
        terrainGen.Setup(16, 500, 250, 7 * (int)(intensity), 22 * (int)(scale));
        var rafale = Instantiate(rafalePrefab, levelContainer.transform);
        rafale.transform.position = new Vector3(30, 53, 0);
        rafale.GetComponent<PlaneController>().endCanvas = endCanvas;

        foreach (Transform child in playableMapObject.transform)
        {
            if (child.gameObject.activeSelf == true)
            {
                //todo adjust height add randomness
                var rectTransform = child.GetComponent<RectTransform>();
                var position = rectTransform.localPosition * 10;
                position.z = position.y;

                var random = UnityEngine.Random.Range(0, 200);
                position.y = 300 + random;
                if (child.gameObject.name.Contains("EnemyBunker"))
                {
                    var tempPositionY = 0f;
                    if (Physics.Raycast(position, Vector3.down, out var hit, Mathf.Infinity))
                    {
                        tempPositionY = hit.point.y + 50f;
                    }
                    position.y = tempPositionY;
                    Instantiate(bunkerPrefab, position, Quaternion.identity, enemyContainer.transform);
                }
                else
                {
                    Instantiate(jetPrefab, position, Quaternion.identity, enemyContainer.transform);
                }
            }
        }

        gameManager.GetComponent<GameManager>().rafale = rafale;
        gameManager.GetComponent<GameManager>().enabled = true;

        var planeUI = planeHUD.GetComponent<PlaneHUD>();
        planeUI.camera = rafale.GetComponent<CameraSwitcher>().thirdPersonCamera; // mozda nije dobro
        planeUI.planeTransform = rafale.transform;

        var pitchUI = pitchHUD.GetComponent<PitchUI>();
        pitchUI.camera = rafale.GetComponent<CameraSwitcher>().thirdPersonCamera;
        pitchUI.planeTransform = rafale.transform;

        var compass = compassHUD.GetComponent<CompassScript>();
        compass.camera = rafale.GetComponent<CameraSwitcher>().thirdPersonCamera;
        compass.planeTransform = rafale.transform;

        var velocity = velocityHUD.GetComponent<VelocityScript>();
        velocity.planeTransform = rafale.transform;

        var altitude = altitudeHUD.GetComponent<LatitudeScript>();
        altitude.planeTransform = rafale.transform;

        var boresight = boresightHUD.GetComponent<BoresightScript>();
        boresight.planeReference = rafale.transform;
        boresight.cameraReference = rafale.GetComponent<CameraSwitcher>().thirdPersonCamera.transform;

        hudContainer.SetActive(true);

    }

    public void Quit()
    {
        Application.Quit();
    }
}
