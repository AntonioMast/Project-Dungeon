/******************************************************
 * This code is applied to the EventSystem object.
 * The purpose of this script is to handle all
 * events and caluclations needed for the general
 * game state.
*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControllerScript : MonoBehaviour
{
    //*********************
    //Variable Declarations
    //*********************
    public Slider cooldownSlider;

    GameObject ScoreObj;
    Text ScoreText;
    GameObject CurrencyObj;
    Text CurrencyText;
    GameObject CurrencyUgradeObj;
    Text CurrencyUpgradeText;

    float clickedCurrencyTimer = 0f;
    float timer = 0f;
    float spawnTimer = 0f;
    float seconds = 0;
    public int score = 0;
    public int currency = 0;
    public int currencyAddition;
    public int currencyAdditionPrice;
    public int price;

    GameObject objectSpawned;
    public GameObject objectSpawned1;
    public GameObject objectSpawned2;
    public GameObject objectSpawned3;
    public GameObject objectSpawned4;

    //This function only runs when the object is created
    //it is used to set variables to initial values
    void Start()
    {
        ScoreObj = GameObject.Find("ScoreValue");
        ScoreText = ScoreObj.GetComponent<Text>();

        CurrencyObj = GameObject.Find("CurrencyValue");
        CurrencyText = CurrencyObj.GetComponent<Text>();

        CurrencyUgradeObj = GameObject.Find("CurrencyUpgradeValue");
        CurrencyUpgradeText = CurrencyUgradeObj.GetComponent<Text>();

        currencyAddition = 2;
        currencyAdditionPrice = 25;
    }

    //this functions runs once per frame
    void Update()
    {
        //runs the timer that is used to slowly increase the score
        timer += Time.deltaTime;
        seconds = timer % 60;

        if (spawnTimer > 0) //lets the timer run
        {
            spawnTimer -= (Time.deltaTime % 60);
            if (spawnTimer < 0) { spawnTimer = 0f; }
        }

        if (clickedCurrencyTimer > 0) //lets the currency timer run
        {
            clickedCurrencyTimer -= (Time.unscaledDeltaTime % 60);
            if (clickedCurrencyTimer < 0) { clickedCurrencyTimer = 0f; }
        }

        for (seconds = seconds; seconds > 1f; seconds = seconds - 1f) //when the timer stops, the score and currency is increased, and the timer is reset
        {
            timer = 0;
            score += 10;
            currency += currencyAddition;
        }

        setCooldownBar();
        UpdateCurrencyToScreen();
        spawnEnemies();
        UpdateScoreToScreen();
    }

    //this function runs once called and multiples the amount of currency gained per second by 1.6
    public void IncreaseCurrencyEarned()
    {
        if (currency >= currencyAdditionPrice) //checks if the currency had is enough to pay for the upgrade
        {
            currency -= currencyAdditionPrice;
            currencyAddition += 1;
            currencyAdditionPrice = (int)(currencyAdditionPrice * 1.6f);
            CurrencyUpgradeText.text = "$" + currencyAdditionPrice.ToString();
        }
    }

    //this function runs once per frame and updates the value of the cooldown bar that is drawn to the screen
    public void setCooldownBar()
    {
        cooldownSlider.value = spawnTimer;
    }

    //this function runs once called and sets the index of the object to spawn to whichever item is clicked by the player
    public void SetObjectIndex(int i)
    {
        switch (i) //sets the index of the object to spawn to whichever item is clicked by the player
        {
            case 0://null
                objectSpawned = null;
                price = 0;
                break;
            case 1://trap
                objectSpawned = objectSpawned1;
                price = 3;
                break;
            case 2://zombie
                objectSpawned = objectSpawned2;
                price = 6;
                break;
            case 3://zombie group
                objectSpawned = objectSpawned3;
                price = 20;
                break;
            case 4://turret
                objectSpawned = objectSpawned4;
                price = 30;
                break;
            default://null
                objectSpawned = null;
                price = 0;
                break;
        }
    }

    //this function runs once per frame and handles the player spawning enemies when clicking on the screen
    void spawnEnemies()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        Vector3 finalVec = new Vector3(worldPoint.x, worldPoint.y, 0.5f);
        float distanceToPlayer = Vector3.Distance(finalVec, GameObject.Find("Hero").transform.position);

        //enemy spawning
        if (Input.GetKeyDown(KeyCode.Mouse0) && Input.mousePosition.y > Screen.height / 6 && currency >= price && spawnTimer == 0 && distanceToPlayer > 5f)
        {
            spawnTimer = 0.2f;
            currency -= price;

            if (objectSpawned != null)
            {
                GameObject tmp = Instantiate(objectSpawned) as GameObject;
                tmp.transform.position = finalVec;
            }
        }
    }

    //this function runs once per frame and updates the score drawn to the screen
    void UpdateScoreToScreen()
    {
        ScoreText.text = score.ToString();
    }

    //this function runs once per frame and updates the currency drawn to the screen
    void UpdateCurrencyToScreen()
    {
        CurrencyText.text = "$" + currency.ToString();
    }

    //this function runs once called and increases the currency had by one per click on the corresponding button
    public void ClickedCurrency()
    {
        if (Time.timeScale == 1 && clickedCurrencyTimer <= 0f)
        {
            clickedCurrencyTimer = 0.1f;
            currency += 1;
            UpdateCurrencyToScreen();
        }
    }
}
