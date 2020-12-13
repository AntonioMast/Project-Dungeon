/******************************************************
 * This code is applied to the player hero game object.
 * That game object is only used to give the player the
 * option to traverse the dungeon in the creation
 * screen, however, much code here is used to give 
 * compatability if a feature to let the player traverse
 * the dungeon in the main game was later added.
 * Much of this code is the same as the A.I. controlled
 * hero's script.
*******************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHeroCode : MonoBehaviour
{
    //*********************
    //Variable Declarations
    //*********************

    //for setting healthbar *only used if player traverses a dungeon*
    public Slider slider;

    //for movement
    Rigidbody2D body;
    float horizontal;
    float vertical;

    //for time keeping
    float timer = 0.0f;
    float seconds = 0;

    //for game-specific values
    public float runSpeed = 10f;
    public float health;

    //for scene 
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

    //This function only runs when the object is created
    //it is used to set variables to initial values
    void Start()
    { 
        health = 100f;
        body = GetComponent<Rigidbody2D>();

        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;

        Stage = GameObject.Find("GameController");

        if (sceneName == "AIScene")
        { this.gameObject.transform.position = GameObject.Find("StageStartingArea").transform.position + new Vector3(0, 0, -1f); }

        lastAttackTime = Time.time;
    }

    //this functions runs once per frame
    void Update()
    {
        //runs the timer that is used to slowly increase hero stats
        timer += Time.deltaTime;
        seconds = timer % 60;

        for (seconds = seconds; seconds > 2f; seconds = seconds - 2f) //increases the heros stats when enough time has passes
        {
            attackDelay -= 0.01f;
            if (attackDelay < 0.2f) { attackDelay = 0.2f; }
            timer = 0;
            if (health < 100f) { health += 1f; }
            if (health > 100f) { health = 100f; }
        }

        if (sceneName == "AIScene") { setHealthBar(); }

        checkIfTimeToFire();
        
        //Checks if the player is trying to move
        vertical = 0; horizontal = 0;
        if (Input.GetKey(KeyCode.W)) { vertical += 1; }
        if (Input.GetKey(KeyCode.S)) { vertical -= 1; }
        if (Input.GetKey(KeyCode.A)) { horizontal -= 1; }
        if (Input.GetKey(KeyCode.D)) { horizontal += 1; }
        //Calls movement function
        movePlayer();
    }

    //this function is called every frame and handles the math needed to move the player
    private void movePlayer()
    {
        Vector2 PushVec = new Vector2(horizontal * runSpeed * Time.deltaTime, vertical * runSpeed * Time.deltaTime);
        Vector2 tmpVec = (body.velocity + PushVec);
        if (tmpVec.magnitude < 5f)
        { body.velocity = tmpVec; }
    }

    //this function is called every frame and checks if the 'player hero' object should fire at an enemy
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

                if (distanceToTarget != null && distanceToTarget < attackRange) //if the distance to the enemy is less than the maximum distance the hero can attack from
                {
                    float timeUntilNextAttack = (lastAttackTime + attackDelay);
                    if (Time.time > timeUntilNextAttack) //checks if enough time has passed since the last attack to allow the hero to attack again
                    {
                        RaycastHit2D Hit = Physics2D.Raycast(transform.position, (currentTarget.position - transform.position).normalized, attackRange, LayerMask.NameToLayer("Enemy"));

                        if (Hit.transform.tag == "Enemy") //checks if the enemy is behind a wall/object
                        {
                            //calculates the location and motion vector to spawn the bullet object with
                            Vector3 tmp = (currentTarget.position - transform.position).normalized;
                            GameObject newBullet = Instantiate(bullet, transform.position, transform.rotation);
                            //spawns the bullet
                            newBullet.GetComponent<Rigidbody2D>().AddForce((tmp + new Vector3(Random.Range(-0.12f, 0.12f), Random.Range(-0.12f, 0.2f), 0)) * bulletForce, ForceMode2D.Impulse);
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

    //this function is called whenever a game object begins touching the player hero game object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy") //if the object is an enemy to the hero, the object is destroyed and corresponding values are calculated
        {
            if (collision.gameObject.name.Contains("Zombie") || collision.gameObject.name.Contains("Trap"))
            {
                health -= 2;
                GameObject.Find("EventSystem").GetComponent<GameControllerScript>().currency += 2;
            }

            Destroy(collision.gameObject);
            if (health <= 0) //if the hero's health is 0, the game is taken back to the main menu
            {
                Destroy(Stage);
                SceneManager.LoadScene(0);
            }
        }

        if (collision.gameObject.tag == "EndTrigger" && sceneName == "AIScene") //if the hero touches the end point, the game ends
        {
            Destroy(Stage);
            SceneManager.LoadScene(0);
        }
    }
}
