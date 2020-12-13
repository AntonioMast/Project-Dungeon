/***************************************************************
 * This code is applied to the turret enemy objects.
 * It handles major things needed by them such as shooting.
 **************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    //*********************
    //Variable Declarations
    //*********************
    public int health = 2;
    int layerMask;

    //for shooting
    public float attackDelay = 1.2f;
    float lastAttackTime;
    public float bulletForce;
    public GameObject bullet;
    private float distanceToTarget;
    public float attackRange;

    //This function only runs when the object is created
    //it is used to set variables to initial values
    void Start()
    {
        layerMask = ~(1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("AINode"));
    }

    //this function runs once per frame
    void Update()
    {
        checkIfTimeToFire();
    }

    //this functions is called once per frame and is used to handle shooting at the A.I. hero opponent
    private void checkIfTimeToFire()
    {
        GameObject Hero = GameObject.Find("Hero");
        float distanceToTarget = Vector3.Distance(Hero.transform.position, transform.position);

        
        if (distanceToTarget != null && distanceToTarget <= attackRange) //checks if the hero is in attack range.
        {
            float timeUntilNextAttack = (lastAttackTime + attackDelay);
            if (Time.time >= timeUntilNextAttack) //checks if the turret is on cooldown
            {
                RaycastHit2D Hit = Physics2D.Raycast(transform.position, (Hero.transform.position - transform.position).normalized, attackRange, layerMask);

                if (Hit.transform.tag == "Hero") //checks if any objects are in the way of the hero (such as walls)
                {
                    Vector3 tmp = (Hero.transform.position - transform.position).normalized;
                    GameObject newBullet = Instantiate(bullet, transform.position, transform.rotation);
                    newBullet.GetComponent<Rigidbody2D>().AddForce((tmp + new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f), 0)) * bulletForce, ForceMode2D.Impulse);

                    lastAttackTime = Time.time;
                }
            }
        }
    }

    //this function runs when another objects collides with object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "bullet") //if the turret is hit by the A.I. opponent, it loses health until it dies.
        {
            Destroy(collision.gameObject);
            health -= 1;

            if (health <= 0)
            { Destroy(gameObject); }

        }
    }

}