/***************************************************************
 * This code is applied to the zombie enemy objects.
 * It handles major things needed by them such as traversal.
 **************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombieScript : MonoBehaviour
{
    //*********************
    //Variable Declarations
    //*********************
    Rigidbody2D body;
    float horizontal;
    float vertical;
    public float runSpeed = 10f;
    public float health;
    GameObject[] tmpcurrentTarget;
    GameObject currentTarget;
    public float attackRange;
    Vector3 lastKnownPosition;
    float timer = 0;
    int layerMask;


    //This function only runs when the object is created
    //it is used to set variables to initial values
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        tmpcurrentTarget = GameObject.FindGameObjectsWithTag("Hero");
        currentTarget = tmpcurrentTarget[0];
        lastKnownPosition = transform.position;
        layerMask = ~(1 << LayerMask.NameToLayer("AINode"));
    }

    //this function runs once per frame
    void Update()
    {
        moveZombie();
    }

    //this function is called once per a frame and is used to move the zombie objects, both through traversal and through random movement.
    private void moveZombie()
    {
        //random movement
        vertical = Random.Range(-0.5f, 0.5f); horizontal = Random.Range(-0.5f, 0.5f);

        RaycastHit2D Hit = Physics2D.Raycast(transform.position, (currentTarget.transform.position - transform.position).normalized, attackRange, layerMask);

        if (Hit && Hit.transform.tag == "Hero") //checks if the A.I. hero opponent is in sight
        {
            timer = 0.25f;
            lastKnownPosition = currentTarget.transform.position;
            Vector3 tmp = (currentTarget.transform.position - transform.position).normalized;
            horizontal += tmp.x;
            vertical += tmp.y;
        }
        
        //the purpose of these lines is to attempt navigation if it tries to move into an object or wall
        else if ((transform.position - lastKnownPosition).magnitude > 1f && timer > 0 && Hit.transform.tag != "bullet" && Hit.transform.tag != "Enemy" && Hit.transform.tag != "bulletEnemy")
        {
            timer -= Time.deltaTime;

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
        }

        else if ((transform.position - lastKnownPosition).magnitude > 1f) //moves the zombie to the last know location of the A.I. hero when it loses sight of it
        {
            horizontal -= transform.position.x - lastKnownPosition.x;
            vertical -= transform.position.y - lastKnownPosition.y;
        }

        Vector2 PushVec = new Vector2(horizontal * runSpeed * Time.deltaTime, vertical * runSpeed * Time.deltaTime);
        Vector2 tmpVec = (body.velocity + PushVec);
        
        if (tmpVec.magnitude < 5f)
        { body.velocity = tmpVec; }
    }

    //this function runs when another objects collides with object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("bullet") == true) //kills the zombie when a bullet hits it.
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
