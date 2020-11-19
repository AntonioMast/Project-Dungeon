using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    public int health = 2;
    int layerMask;

    //for shooting
    public float attackDelay = 1.2f;
    float lastAttackTime;
    public float bulletForce;
    public GameObject bullet;
    private float distanceToTarget;
    public float attackRange;

    // Start is called before the first frame update
    void Start()
    {
        layerMask = ~(1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("AINode"));
    }

    // Update is called once per frame
    void Update()
    {
        checkIfTimeToFire();
    }

    private void checkIfTimeToFire()
    {
        GameObject Hero = GameObject.Find("Hero");
        float distanceToTarget = Vector3.Distance(Hero.transform.position, transform.position);

        //heres the rest
        if (distanceToTarget != null && distanceToTarget <= attackRange)
        {
            float timeUntilNextAttack = (lastAttackTime + attackDelay);
            if (Time.time >= timeUntilNextAttack)
            {
                RaycastHit2D Hit = Physics2D.Raycast(transform.position, (Hero.transform.position - transform.position).normalized, attackRange, layerMask);

                if (Hit.transform.tag == "Hero")
                {
                    Vector3 tmp = (Hero.transform.position - transform.position).normalized;
                    GameObject newBullet = Instantiate(bullet, transform.position, transform.rotation);
                    newBullet.GetComponent<Rigidbody2D>().AddForce((tmp + new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f), 0)) * bulletForce, ForceMode2D.Impulse);

                    lastAttackTime = Time.time;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "bullet")
        {
            Destroy(collision.gameObject);
            health -= 1;

            if (health <= 0)
            { Destroy(gameObject); }

        }
    }

}