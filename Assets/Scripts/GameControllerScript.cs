using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControllerScript : MonoBehaviour
{
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

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        seconds = timer % 60;

        if (spawnTimer > 0)
        {
            spawnTimer -= (Time.deltaTime % 60);
            if (spawnTimer < 0) { spawnTimer = 0f; }
        }

        if (clickedCurrencyTimer > 0)
        {
            clickedCurrencyTimer -= (Time.unscaledDeltaTime % 60);
            if (clickedCurrencyTimer < 0) { clickedCurrencyTimer = 0f; }
        }

        for (seconds = seconds; seconds > 1f; seconds = seconds - 1f)
        {
            timer = 0;
            score += 10;
            currency += currencyAddition;
        }

        pauseHandler();
        setCooldownBar();
        UpdateCurrencyToScreen();
        spawnEnemies();
        UpdateScoreToScreen();
    }

    public void IncreaseCurrencyEarned()
    {
        if (currency >= currencyAdditionPrice)
        {
            currency -= currencyAdditionPrice;
            currencyAddition += 1;
            currencyAdditionPrice = (int)(currencyAdditionPrice * 1.6f);
            CurrencyUpgradeText.text = "$" + currencyAdditionPrice.ToString();
        }
    }

    public void pauseHandler()
    {
        if (Input.GetKeyDown("space"))
        {
            if (Time.timeScale == 0)
            { Time.timeScale = 1f; }

            else
            { Time.timeScale = 0; }
        }

    }

    public void setCooldownBar()
    {
        cooldownSlider.value = spawnTimer;
    }

    public void SetObjectIndex(int i)
    {
        switch (i)
        {
            case 0:
                objectSpawned = null;
                price = 0;
                break;
            case 1:
                objectSpawned = objectSpawned1;
                price = 3;
                break;
            case 2:
                objectSpawned = objectSpawned2;
                price = 6;
                break;
            case 3:
                objectSpawned = objectSpawned3;
                price = 20;
                break;
            case 4:
                objectSpawned = objectSpawned4;
                price = 30;
                break;
            default:
                objectSpawned = null;
                price = 0;
                break;
        }
    }

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

    void UpdateScoreToScreen()
    {
        ScoreText.text = score.ToString();
    }

    void UpdateCurrencyToScreen()
    {
        CurrencyText.text = "$" + currency.ToString();
    }

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
