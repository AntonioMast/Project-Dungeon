/***************************************************************
 * This code is applied to the A.I. controlled hero game object.
 * This code handles the traversal, shooting, health systems,
 * and more of that object. This code is essentially the 'main
 * component' for the premise of the game.
 **************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AiHero : MonoBehaviour
{
    //*********************
    //Variable Declarations
    //*********************

    //for setting healthbar
    public Slider slider;

    //for general movement
    Rigidbody2D body;
    float horizontal;
    float vertical;

    //for time keeping
    float timer = 0.0f;
    float finalTimer = 2.0f;
    float seconds = 0;

    //for game-specific values
    public float runSpeed;
    public float health;
    float healthIncreaseValue;

    //for score keeping
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

    //This function only runs when the object is created
    //it is used to set variables to initial values
    void Start()
    {
        spawntimer = 4;
        healthIncreaseValue = 0.15f;
        health = 100f;
        body = GetComponent<Rigidbody2D>();
        endgame = false;

        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;

        Stage = GameObject.Find("GameController");

        if (sceneName == "AIScene")
        {
            this.gameObject.transform.position = GameObject.Find("StageStartingArea").transform.position + new Vector3(0, 0, -1f);
        }

        lastAttackTime = Time.time;

        //layer mask that ignores the player and ainode layers
        layerMask = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("AINode"));
        //layer mask that ignores only the player layer
        layerMaskNoPlayer = ~(LayerMask.GetMask("Player"));

        //navigation and traversal variables
        whereToGo = new Vector3 (0, 0, -1);
        firstmove = true;
        withinCurrentNodeRange = false;
    }

    //this function runs once per frame
    void Update()
    {
        //runs the timer that is used to slowly increase hero stats
        timer += Time.deltaTime;
        seconds = timer % 60;

        for (seconds = seconds; seconds > 1f; seconds = seconds - 1f) //increases the heros stats when enough time has passes
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

        if (sceneName == "AIScene") { setHealthBar(); } //updates the drawn healthbar to the correct values

        checkIfTimeToFire();
        gameEndHandler();

        if (spawntimer > 0) //makes the hero object wait in place for a few seconds after loading to give the player time to prepare
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

    //This functions handles all of the dungeon navigation and movement done by the A.I. hero.
    //It is called once per frame after the initial waiting has ended.
    private void NavigateDungeon()
    {
        //checks if an endpoint is in sight
        GameObject[] endFinder = null;
        endFinder = GameObject.FindGameObjectsWithTag("EndTrigger");

        if (endFinder.Length > 0) //if an endpoint exists
        {
            Transform endpointInSight = GetEndpointInSight(endFinder);

            if (endpointInSight != null) //if an endpint is in sight
            {
                RaycastHit2D Hit = Physics2D.Raycast(transform.position, (endpointInSight.position - transform.position).normalized, Mathf.Infinity, layerMask);

                if (Hit.collider.tag == "EndTrigger") //if no object/wall is in the way of the endpoint (eg. an enemy)
                {
                    Vector3 tmp = (endpointInSight.position - transform.position).normalized;
                    horizontal = tmp.x;
                    vertical = tmp.y;
                    RaycastHit2D Hit2 = Physics2D.Raycast(transform.position, (endpointInSight.position - transform.position).normalized, 15f, LayerMask.NameToLayer("Enemy"));

                    if (Hit2 && Hit2.collider.tag == "Enemy") //stop moving if enemy is in direct path to the end
                    {
                        horizontal = 0;
                        vertical = 0;
                    }
                }
            }

            //****************************
            //Modified Trémaux's algorithm
            //****************************
            else //if there is not a straight path to the end, then a modified Trémaux's algorithm is applied for navigation
            {
                Transform UnmarkedNode = null;
                Transform MarkedNode = null;

                //Opponent determines inital movement when game starts
                if (firstmove == true) //if the opponent has yet to move
                {
                    GameObject[] destinationFinder = null;
                    destinationFinder = GameObject.FindGameObjectsWithTag("AINode");

                    if (destinationFinder.Length > 0)//if the opponent is not already at the node it decides to move towards
                    {
                        UnmarkedNode = CastRayToUnmarkedNodes(destinationFinder);
                        MarkedNode = CastRayToMarkedNodes(destinationFinder);

                        if (UnmarkedNode != null) //if there is a viable node to move towards that hasn't been visited before
                        {
                            //logic to determine how to mark previous node
                            if (lastNode != null) //if the hero has moved from a node already
                            {
                                if (lastNode.GetComponent<NodeScript>().firstnode == false) //if the last node the hero moved from wasn't the beginning
                                {
                                    if (lastNode.GetComponent<NodeScript>().unmarked == true) //if the last node hasn't been marked as visited to, then mark it as having been visited
                                    { lastNode.GetComponent<NodeScript>().unmarked = false; }
                                }
                            }

                            //move in the direction of the closest node that hasn't been visted to yet and remember the location of the current node
                            whereToGo.x = UnmarkedNode.position.x;
                            whereToGo.y = UnmarkedNode.position.y;
                            lastNode = UnmarkedNode.gameObject;
                        }

                        else //if there isn't a viable node to move towards that hasn't been visited before
                        {
                            //move in the direction of the closest node that HAS already been visted to and remember the location of the current node
                            whereToGo.x = MarkedNode.position.x;
                            whereToGo.y = MarkedNode.position.y;
                            lastNode = MarkedNode.gameObject;
                        }
                    }

                    //disables detection of current node temporarily for navigation
                    lastNode.GetComponent<BoxCollider2D>().enabled = false;
                    firstmove = false;
                }

                float dist = Vector3.Distance(lastNode.transform.position, transform.position);

                //Opponent determines subsequent movement once it reaches destination
                if (dist < 1.05f) //if the last node visited is closer than 1.05 units
                {
                    GameObject tmpNode = lastNode;
                    GameObject[] destinationFinder = null;
                    destinationFinder = GameObject.FindGameObjectsWithTag("AINode");

                    if (destinationFinder.Length > 0) //if there is atleast one viable node to move towards
                    {
                        UnmarkedNode = CastRayToUnmarkedNodes(destinationFinder);
                        MarkedNode = CastRayToMarkedNodes(destinationFinder);

                        if (UnmarkedNode != null) //if there exists a node to move towards that hasn't been visited before
                        {
                            //logic to determine how to mark previous node
                            if (lastNode != null) //if the hero has previously visted a node
                            {
                                if (lastNode.GetComponent<NodeScript>().firstnode == false) //if the last node visited wasn't the initial node
                                {
                                    if (lastNode.GetComponent<NodeScript>().unmarked == true) //if the last node visted hasn't been visted before then
                                    { lastNode.GetComponent<NodeScript>().unmarked = false; } //mark the last node as having been visited
                                }
                            }
                            //move in the direction of the closest node that has not been visted to yet and remember the location of the current node
                            whereToGo.x = UnmarkedNode.position.x;
                            whereToGo.y = UnmarkedNode.position.y;
                            lastNode = UnmarkedNode.gameObject;
                        }

                        else //if there does not exist a node to move towards that hasn't been visited before
                        {
                            //logic to determine how to mark previous node
                            if (lastNode != null) //if the hero has previously visted a node
                            {
                                if (lastNode.GetComponent<NodeScript>().firstnode == false) //if the last node visited wasn't the initial node
                                {
                                    lastNode.GetComponent<NodeScript>().completelyUsed = true; //set the last node as unviable--the a.i. will not traverse using that node
                                }
                            }
                            //move in the direction of the closest node that has been visted already and remember the location of the current node
                            whereToGo.x = MarkedNode.position.x;
                            whereToGo.y = MarkedNode.position.y;
                            lastNode = MarkedNode.gameObject;
                        }
                    }
                    //disables detection of current node temporarily for navigation and reanbles detection of the previous node
                    tmpNode.GetComponent<BoxCollider2D>().enabled = true;
                    lastNode.GetComponent<BoxCollider2D>().enabled = false;
                    movementResetTimer = 0.5f;
                }

                //subfinal movement calculations
                Vector3 tmp = (whereToGo - transform.position).normalized;
                horizontal = tmp.x;
                vertical = tmp.y;

                //The following code is used to correct navigation if a wall is dected to be in the path
                RaycastHit2D HitInCurrentMovement = Physics2D.Raycast(transform.position, (whereToGo - transform.position).normalized, 6f, layerMask);
                if (HitInCurrentMovement && HitInCurrentMovement.collider.tag == "Floor") //if a wall is detected to be in the way for the current frame, keep correcting the movement
                {
                    keepCorrectingMovement = true; ;
                }

                if (movementResetTimer > 0f && keepCorrectingMovement) //if the a.i. opponent has attempted to correct movement for less than 0.5 seconds and
                {                                                      //has detected a wall in its path for atleast two consecutive frames
                    movementResetTimer -= Time.deltaTime;

                    if (Mathf.Abs(horizontal) < Mathf.Abs(vertical))//if the horizontal speed is less than the vertical speed
                    {
                        horizontal = horizontal * 6; //attempt to correct the movement by exagerting the horizontal speed and removing vertical speed
                        vertical = 0;
                    }

                    else if (Mathf.Abs(vertical) < Mathf.Abs(horizontal))//if the vertical speed is less than the horizontal speed
                    {
                        vertical = vertical * 6; //attempt to correct the movement by exagerting the vertical speed and removing horizontal speed
                        horizontal = 0;
                    }

                    if (movementResetTimer <= 0) //if movement correction has been attempted for 0.5 seconds or more, then stop attempting to correct movement
                    { keepCorrectingMovement = false; }
                }
            }

            //slightly randomizes frame-to-frame movement
            vertical += Random.Range(-0.5f, 0.5f); horizontal += Random.Range(-0.5f, 0.5f);
        }

        //final movement
        Vector2 PushVec = new Vector2(horizontal * runSpeed * Time.deltaTime, vertical * runSpeed * Time.deltaTime);
        Vector2 tmpVec = (body.velocity + PushVec);
        if (tmpVec.magnitude < 5f)
        { body.velocity = tmpVec; }
    }

    //this functions runs as-called and detects any viable navigation nodes that have NOT been previously visited for the A.I. hero to move towards
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

    //this functions runs as-called and detects any viable navigation nodes that have been previously visited for the A.I. hero to move towards
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

    //this functions runs as-called and detects any viable endpoints for the A.I. hero to move towards
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

    //this functions runs once per frame and is used to determine if the A.I. hero should be attack an enemy
    private void checkIfTimeToFire()
    {
        GameObject[] enemyFinder = null;
        enemyFinder = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemyFinder.Length > 0) //if there is atleast a single enemy that exists
        {
            Transform currentTarget = GetClosestEnemy(enemyFinder);

            if (currentTarget != null) //makes sure that the distance to the closest enemy isn't null
            {
                distanceToTarget = Vector3.Distance(currentTarget.position, transform.position);

                if (distanceToTarget != null && distanceToTarget < attackRange) //if the distance to the enemy is less than the maximum distance the A.I. hero can attack from
                {
                    float timeUntilNextAttack = (lastAttackTime + attackDelay);
                    if (Time.time > timeUntilNextAttack) //checks if enough time has passed since the last attack to allow the hero to attack again
                    {
                        RaycastHit2D Hit = Physics2D.Raycast(transform.position, (currentTarget.position - transform.position).normalized, attackRange, LayerMask.NameToLayer("Enemy"));

                        if (Hit.transform.tag == "Enemy") //a final check to make sure no other objects are obstructing the hero's view of the enemy
                        {
                            //calculates the location and motion vector to spawn the bullet object with
                            Vector3 tmp = (currentTarget.position - transform.position).normalized;
                            GameObject newBullet = Instantiate(bullet, transform.position, transform.rotation);
                            //spawns the bullet
                            newBullet.GetComponent<Rigidbody2D>().AddForce((tmp + new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f), 0)) * bulletForce, ForceMode2D.Impulse);
                            //sets the next time that a bullet should fire
                            lastAttackTime = Time.time;
                        }
                    }
                }
            }

        }
    }

    //this function is called every frame that an enemy exists and it returns the enemy that is closest to the hero and is within the hero's sight (eg. not behind a wall)
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

    //this function sets the value of the healthbar that is drawn to the screen to the correct value
    public void setHealthBar()
    {
        slider.value = health;
    }

    //this function is called either when the hero runs out of health or when the hero reaches the end destination
    private void gameEndHandler()
    {
        if (endgame == true) //if an end condition has been met
        {
            if (finalTimer == 2.0f) //if it is the frame the end condition has been met
            {
                //The following code and if statments are used to keep track of the score and to save the score if it is a new highscore
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

                //stops the music and plays a tone to indicate the game has ended
                Destroy(music);
                audioSource.Play();
            }

            //pauses the game
            Time.timeScale = 0f;
            finalTimer -= Time.unscaledDeltaTime;

            if (finalTimer < 1.1f) //fades the screen to black once some time has passed from the game's end
            {
                blackImage.GetComponent<fadeFromBlackScript>().fadeFromBlack = false;
            }

            if (finalTimer <= 0.0f) //ends the game completely and loads the score screen once two seconds has passed from the end condition being met
            {
                //loading stuff
                Time.timeScale = 1f;
                Destroy(Stage);
                SceneManager.LoadScene(3);
            }
        }
    }

    //this function is called whenever a game object begins touching the hero game object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Enemy"))//if an enemy touched the hero
        {
            if (collision.gameObject.name.Contains("Trap"))//if the enemy was a trap, it calculates corresponding values to be changed
            {
                health -= 2;
                GameObject.Find("EventSystem").GetComponent<GameControllerScript>().currency += 3;
                eventSystem.GetComponent<GameControllerScript>().score += 10;
            }
            else if (collision.gameObject.name.Contains("Zombie"))//if the enemy was a zombie, it calculates corresponding values to be changed
            {
                health -= 3;
                GameObject.Find("EventSystem").GetComponent<GameControllerScript>().currency += 5;
                eventSystem.GetComponent<GameControllerScript>().score += 15;
            }

            else if (collision.gameObject.name.Contains("EnemyBullet"))//if the enemy was a bullet, it calculates corresponding values to be changed
            {
                health -= 2;
                GameObject.Find("EventSystem").GetComponent<GameControllerScript>().currency += 4;
                eventSystem.GetComponent<GameControllerScript>().score += 10;
            }

            else if (collision.gameObject.name.Contains("Turret"))//if the enemy was a turret itself, it calculates corresponding values to be changed
            {
                health -= 1;
                GameObject.Find("EventSystem").GetComponent<GameControllerScript>().currency += 5;
                eventSystem.GetComponent<GameControllerScript>().score += 15;
            }



            Destroy(collision.gameObject);//deletes the object which touched the hero
            if (health <= 0)//if the hero's health is 0 or less
            {
                //adds points to the players score and sets the end condition as being met
                eventSystem.GetComponent<GameControllerScript>().score += 1000;
                endgame = true;
            }
        }

        if (collision.gameObject.tag == "EndTrigger" && sceneName == "AIScene") //if the hero touched the end of the dungeon
        {
            endgame = true; //sets the end condition as being met
        }
    }
}
