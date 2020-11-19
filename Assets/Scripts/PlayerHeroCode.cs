using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHeroCode : MonoBehaviour
{
    public Slider slider;

    Rigidbody2D body;
    float horizontal;
    float vertical;

    float timer = 0.0f;
    float seconds = 0;

    public float runSpeed = 10f;
    public float health;

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

    void Update()
    {
        //health
        timer += Time.deltaTime;
        seconds = timer % 60;

        

        for (seconds = seconds; seconds > 2f; seconds = seconds - 2f)
        {
            attackDelay -= 0.01f;
            if (attackDelay < 0.2f) { attackDelay = 0.2f; }
            timer = 0;
            if (health < 100f) { health += 1f; }
            if (health > 100f) { health = 100f; }
        }

        if (sceneName == "AIScene") { setHealthBar(); }

        checkIfTimeToFire();
        
        //movement
        vertical = 0; horizontal = 0;
        if (Input.GetKey(KeyCode.W)) { vertical += 1; }
        if (Input.GetKey(KeyCode.S)) { vertical -= 1; }
        if (Input.GetKey(KeyCode.A)) { horizontal -= 1; }
        if (Input.GetKey(KeyCode.D)) { horizontal += 1; }
        movePlayer();
    }

    private void movePlayer()
    {
        Vector2 PushVec = new Vector2(horizontal * runSpeed * Time.deltaTime, vertical * runSpeed * Time.deltaTime);
        Vector2 tmpVec = (body.velocity + PushVec);
        if (tmpVec.magnitude < 5f)
        { body.velocity = tmpVec; }
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
                            newBullet.GetComponent<Rigidbody2D>().AddForce((tmp + new Vector3(Random.Range(-0.12f, 0.12f), Random.Range(-0.12f, 0.2f), 0)) * bulletForce, ForceMode2D.Impulse);

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.gameObject.name.Contains("Zombie") || collision.gameObject.name.Contains("Trap"))
            {
                health -= 2;
                GameObject.Find("EventSystem").GetComponent<GameControllerScript>().currency += 2;
            }



            Destroy(collision.gameObject);
            if (health <= 0)
            {
                Destroy(Stage);
                SceneManager.LoadScene(0);
            } //eventually change this to a win screen
        }

        if (collision.gameObject.tag == "EndTrigger" && sceneName == "AIScene")
        {
            Destroy(Stage);
            SceneManager.LoadScene(0); //eventually change this to a lose screen
        }
    }
}
