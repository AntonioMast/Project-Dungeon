using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AiHero : MonoBehaviour
{
    public Slider slider;

    Rigidbody2D body;
    float horizontal;
    float vertical;

    float timer = 0.0f;
    float finalTimer = 2.0f;
    float seconds = 0;

    public float runSpeed;
    public float health;
    float healthIncreaseValue;

    Scene currentScene;
    string sceneName;
    GameObject Stage;

    //for shooting
    public float attackDelay = 0.8f;
    float lastAttackTime;
    public float bulletForce;
    public GameObject bullet;
    private float distanceToTarget;
    public float attackRange;

    //for navigation
    int layerMask;
    int layerMaskNoPlayer;
    Vector3 whereToGo;
    GameObject lastNode;
    bool firstmove;
    bool withinCurrentNodeRange;
    float movementResetTimer = 0;
    bool keepCorrectingMovement = false;

    //for spawning
    public int spawntimer;

    //for others
    public GameObject eventSystem;
    public GameObject blackImage;
    public GameObject music;
    bool endgame;
    public AudioSource audioSource;

    void Start()
    {
        spawntimer = 4;
        healthIncreaseValue = 0.15f;
        health = 100f;
        body = GetComponent<Rigidbody2D>();

        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;

        Stage = GameObject.Find("GameController");

        if (sceneName == "AIScene")
        {
            this.gameObject.transform.position = GameObject.Find("StageStartingArea").transform.position + new Vector3(0, 0, -1f);
        }

        lastAttackTime = Time.time;

        //ignores the player and ainode layers
        layerMask = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("AINode"));
        layerMaskNoPlayer = ~(LayerMask.GetMask("Player"));

        //navigation variables
        whereToGo = new Vector3 (0, 0, -1);
        firstmove = true;
        withinCurrentNodeRange = false;

        //others
        endgame = false;
    }

    void Update()
    {
        //health
        timer += Time.deltaTime;
        seconds = timer % 60;



        for (seconds = seconds; seconds > 1f; seconds = seconds - 1f)
        {
            runSpeed += 0.01f;
            if (runSpeed > 7f) { healthIncreaseValue = 7f; }
            healthIncreaseValue += 0.002f;
            if (healthIncreaseValue > 2.0f) { healthIncreaseValue = 2.0f; }
            attackDelay -= 0.002f;
            if (attackDelay < 0.2f) { attackDelay = 0.2f; }
            timer = 0;
            if (health < 100f) { health += healthIncreaseValue; }
            if (health > 100f) { health = 100f; }
            if (spawntimer > 0) { spawntimer -= 1; }
        }

        if (sceneName == "AIScene") { setHealthBar(); }

        checkIfTimeToFire();
        gameEndHandler();

        if (spawntimer > 0)
        {
            vertical += Random.Range(-0.005f, 0.005f); horizontal += Random.Range(-0.005f, 0.005f);
            Vector2 PushVec = new Vector2(horizontal * runSpeed * Time.deltaTime, vertical * runSpeed * Time.deltaTime);
            Vector2 tmpVec = (body.velocity + PushVec);
            if (tmpVec.magnitude < 5f)
            { body.velocity = tmpVec; }
        }

        else
        {
            //movement
            vertical = 0; horizontal = 0;
            NavigateDungeon();
        }

    }

    private void NavigateDungeon()
    {
        //check if endpoint is in sight
        GameObject[] endFinder = null;
        endFinder = GameObject.FindGameObjectsWithTag("EndTrigger");

        if (endFinder.Length > 0)
        {
            Transform endpointInSight = GetEndpointInSight(endFinder);

            if (endpointInSight != null)
            {
                RaycastHit2D Hit = Physics2D.Raycast(transform.position, (endpointInSight.position - transform.position).normalized, Mathf.Infinity, layerMask);

                if (Hit.collider.tag == "EndTrigger")
                {
                    Vector3 tmp = (endpointInSight.position - transform.position).normalized;
                    horizontal = tmp.x;
                    vertical = tmp.y;

                    //stop moving it enemy is in way nearby

                    RaycastHit2D Hit2 = Physics2D.Raycast(transform.position, (endpointInSight.position - transform.position).normalized, 15f, LayerMask.NameToLayer("Enemy"));

                    if (Hit2 && Hit2.collider.tag == "Enemy")
                    {
                       
                        horizontal = 0;
                        vertical = 0;
                    }

                }
            }

            else
            {
                //*********************
                //Trémaux's algorithm +
                //*********************

                Transform UnmarkedNode = null;
                Transform MarkedNode = null;

                
                //Opponent determines inital movement when game starts
                if (firstmove == true)
                {
                    GameObject[] destinationFinder = null;
                    destinationFinder = GameObject.FindGameObjectsWithTag("AINode");

                    if (destinationFinder.Length > 0)
                    {
                        UnmarkedNode = CastRayToUnmarkedNodes(destinationFinder);
                        MarkedNode = CastRayToMarkedNodes(destinationFinder);

                        if (UnmarkedNode != null)
                        {
                            //logic to determine how to mark previous node
                            if (lastNode != null)
                            {
                                if (lastNode.GetComponent<NodeScript>().firstnode == false)
                                {
                                    if (lastNode.GetComponent<NodeScript>().unmarked == true)
                                    { lastNode.GetComponent<NodeScript>().unmarked = false; }
                                }
                            }
                            whereToGo.x = UnmarkedNode.position.x;
                            whereToGo.y = UnmarkedNode.position.y;
                            lastNode = UnmarkedNode.gameObject;
                        }

                        else
                        {
                            //logic to determine how to mark previous node
                            whereToGo.x = MarkedNode.position.x;
                            whereToGo.y = MarkedNode.position.y;
                            lastNode = MarkedNode.gameObject;
                        }
                        
                    }
                    lastNode.GetComponent<BoxCollider2D>().enabled = false;
                    firstmove = false;
                }

                float dist = Vector3.Distance(lastNode.transform.position, transform.position);

                //Opponent determines subsequent movement once it reaches destination
                if (dist < 1.05f)
                {
                    GameObject tmpNode = lastNode;
                    GameObject[] destinationFinder = null;
                    destinationFinder = GameObject.FindGameObjectsWithTag("AINode");

                    if (destinationFinder.Length > 0)
                    {
                        UnmarkedNode = CastRayToUnmarkedNodes(destinationFinder);
                        MarkedNode = CastRayToMarkedNodes(destinationFinder);

                        if (UnmarkedNode != null)
                        {
                            //logic to determine how to mark previous node
                            if (lastNode != null)
                            {
                                if (lastNode.GetComponent<NodeScript>().firstnode == false)
                                {
                                    if (lastNode.GetComponent<NodeScript>().unmarked == true)
                                    { lastNode.GetComponent<NodeScript>().unmarked = false; }
                                }
                            }
                            whereToGo.x = UnmarkedNode.position.x;
                            whereToGo.y = UnmarkedNode.position.y;
                            lastNode = UnmarkedNode.gameObject;
                        }

                        else
                        {
                            //logic to determine how to mark previous node
                            if (lastNode != null)
                            {
                                if (lastNode.GetComponent<NodeScript>().firstnode == false)
                                {
                                    lastNode.GetComponent<NodeScript>().completelyUsed = true;
                                }
                            }
                            whereToGo.x = MarkedNode.position.x;
                            whereToGo.y = MarkedNode.position.y;
                            lastNode = MarkedNode.gameObject;
                        }
                    }
                    tmpNode.GetComponent<BoxCollider2D>().enabled = true;
                    lastNode.GetComponent<BoxCollider2D>().enabled = false;
                    movementResetTimer = 0.5f;
                }

                //subfinal movement
                Vector3 tmp = (whereToGo - transform.position).normalized;
                horizontal = tmp.x;
                vertical = tmp.y;

                //corrects if wall in path
                RaycastHit2D HitInCurrentMovement = Physics2D.Raycast(transform.position, (whereToGo - transform.position).normalized, 6f, layerMask);
                if (HitInCurrentMovement && HitInCurrentMovement.collider.tag == "Floor")
                {
                    keepCorrectingMovement = true; ;
                }

                if (movementResetTimer > 0f && keepCorrectingMovement)
                {
                    movementResetTimer -= Time.deltaTime;

                    if (Mathf.Abs(horizontal) < Mathf.Abs(vertical))
                    {
                        horizontal = horizontal * 6;
                        vertical = 0;
                    }

                    else if (Mathf.Abs(vertical) < Mathf.Abs(horizontal))
                    {
                        vertical = vertical * 6;
                        horizontal = 0;
                    }

                    if (movementResetTimer <= 0)
                    { keepCorrectingMovement = false; }
                }

            }

            //randomize movement
            vertical += Random.Range(-0.5f, 0.5f); horizontal += Random.Range(-0.5f, 0.5f);
        }

        //final movement
        Vector2 PushVec = new Vector2(horizontal * runSpeed * Time.deltaTime, vertical * runSpeed * Time.deltaTime);
        Vector2 tmpVec = (body.velocity + PushVec);
        if (tmpVec.magnitude < 5f)
        { body.velocity = tmpVec; }
    }

    Transform CastRayToUnmarkedNodes(GameObject[] endpoint)
    {

        Transform tMin = null;
        float minDist = 26f;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in endpoint)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, (t.transform.position - transform.position).normalized, 26f, layerMaskNoPlayer);
            if (dist < minDist && Hit && Hit.collider.tag == "AINode" && Hit.collider.gameObject.GetComponent<NodeScript>().unmarked == true && Hit.collider.gameObject.GetComponent<NodeScript>().completelyUsed == false)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }
        return tMin;
    }

    Transform CastRayToMarkedNodes(GameObject[] endpoint)
    {

        Transform tMin = null;
        float minDist = 26f;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in endpoint)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, (t.transform.position - transform.position).normalized, 26f, layerMaskNoPlayer);
            if (dist < minDist && Hit && Hit.collider.tag == "AINode" && Hit.collider.gameObject.GetComponent<NodeScript>().unmarked == false && Hit.collider.gameObject.GetComponent<NodeScript>().completelyUsed == false)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }
        return tMin;
    }

    Transform GetEndpointInSight(GameObject[] endpoint)
    {
        
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in endpoint)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, (t.transform.position - transform.position).normalized, Mathf.Infinity, layerMask);
            if (dist < minDist && Hit && Hit.collider.tag == "EndTrigger")
            {
                tMin = t.transform;
                minDist = dist;
            }
        }
        return tMin;
    }

    private void checkIfTimeToFire()
    {
        GameObject[] enemyFinder = null;
        enemyFinder = GameObject.FindGameObjectsWithTag("Enemy");


        if (enemyFinder.Length > 0)
        {
            Transform currentTarget = GetClosestEnemy(enemyFinder);

            if (currentTarget != null)
            {
                distanceToTarget = Vector3.Distance(currentTarget.position, transform.position);

                //heres the rest
                if (distanceToTarget != null && distanceToTarget < attackRange)
                {
                    float timeUntilNextAttack = (lastAttackTime + attackDelay);
                    if (Time.time > timeUntilNextAttack)
                    {
                        RaycastHit2D Hit = Physics2D.Raycast(transform.position, (currentTarget.position - transform.position).normalized, attackRange, LayerMask.NameToLayer("Enemy"));

                        if (Hit.transform.tag == "Enemy")
                        {
                            Vector3 tmp = (currentTarget.position - transform.position).normalized;
                            GameObject newBullet = Instantiate(bullet, transform.position, transform.rotation);
                            newBullet.GetComponent<Rigidbody2D>().AddForce((tmp + new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f), 0)) * bulletForce, ForceMode2D.Impulse);

                            lastAttackTime = Time.time;
                        }
                    }
                }
            }

        }
    }

    Transform GetClosestEnemy(GameObject[] enemies)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in enemies)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, (t.transform.position - transform.position).normalized, attackRange, LayerMask.NameToLayer("Enemy"));
            if (dist < minDist && Hit && Hit.transform.tag == "Enemy")
            {

                tMin = t.transform;
                minDist = dist;
            }
        }
        return tMin;
    }


    public void setHealthBar()
    {
        slider.value = health;
    }

    private void gameEndHandler()
    {
        if (endgame == true)
        {
            if (finalTimer == 2.0f)
            {
                //score keeping stuff
                int score = eventSystem.GetComponent<GameControllerScript>().score;
                PlayerPrefs.SetInt("currentScore", score);

                if (score > PlayerPrefs.GetInt("HighScore1st", 0))
                {
                    PlayerPrefs.SetInt("HighScore3rd", PlayerPrefs.GetInt("HighScore2nd", 0));
                    PlayerPrefs.SetInt("HighScore2nd", PlayerPrefs.GetInt("HighScore1st", 0));
                    PlayerPrefs.SetInt("HighScore1st", score);
                }

                else if (score > PlayerPrefs.GetInt("HighScore2nd", 0))
                {
                    PlayerPrefs.SetInt("HighScore3rd", PlayerPrefs.GetInt("HighScore2nd", 0));
                    PlayerPrefs.SetInt("HighScore2nd", score);
                }

                else if (score > PlayerPrefs.GetInt("HighScore3rd", 0))
                {
                    PlayerPrefs.SetInt("HighScore3rd", score);
                }
                Destroy(music);
                audioSource.Play();
            }

            Time.timeScale = 0f;
            finalTimer -= Time.unscaledDeltaTime;

            if (finalTimer < 1.1f)
            {
                blackImage.GetComponent<fadeFromBlackScript>().fadeFromBlack = false;
            }

            if (finalTimer <= 0.0f)
            {
                //loading stuff
                Time.timeScale = 1f;
                Destroy(Stage);
                SceneManager.LoadScene(3);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Enemy"))
        {
            if (collision.gameObject.name.Contains("Trap"))
            {
                health -= 2;
                GameObject.Find("EventSystem").GetComponent<GameControllerScript>().currency += 3;
                eventSystem.GetComponent<GameControllerScript>().score += 10;
            }
            else if (collision.gameObject.name.Contains("Zombie"))
            {
                health -= 3;
                GameObject.Find("EventSystem").GetComponent<GameControllerScript>().currency += 5;
                eventSystem.GetComponent<GameControllerScript>().score += 15;
            }

            else if (collision.gameObject.name.Contains("EnemyBullet"))
            {
                health -= 2;
                GameObject.Find("EventSystem").GetComponent<GameControllerScript>().currency += 4;
                eventSystem.GetComponent<GameControllerScript>().score += 10;
            }

            else if (collision.gameObject.name.Contains("Turret"))
            {
                health -= 1;
                GameObject.Find("EventSystem").GetComponent<GameControllerScript>().currency += 5;
                eventSystem.GetComponent<GameControllerScript>().score += 15;
            }



            Destroy(collision.gameObject);
            if (health <= 0)
            {
                //score keeping stuff
                eventSystem.GetComponent<GameControllerScript>().score += 1000;
                endgame = true;
            }
        }

        if (collision.gameObject.tag == "EndTrigger" && sceneName == "AIScene")
        {
            endgame = true;
        }
    }
}
