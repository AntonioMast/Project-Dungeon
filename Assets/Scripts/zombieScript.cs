using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombieScript : MonoBehaviour
{
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


    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        tmpcurrentTarget = GameObject.FindGameObjectsWithTag("Hero");
        currentTarget = tmpcurrentTarget[0];
        lastKnownPosition = transform.position;
        layerMask = ~(1 << LayerMask.NameToLayer("AINode"));
    }

    // Update is called once per frame
    void Update()
    {
        moveZombie();
    }

    private void moveZombie()
    {
        //movement
        vertical = Random.Range(-0.5f, 0.5f); horizontal = Random.Range(-0.5f, 0.5f);

        

        RaycastHit2D Hit = Physics2D.Raycast(transform.position, (currentTarget.transform.position - transform.position).normalized, attackRange, layerMask);

        if (Hit && Hit.transform.tag == "Hero")
        {
            timer = 0.25f;
            lastKnownPosition = currentTarget.transform.position;
            Vector3 tmp = (currentTarget.transform.position - transform.position).normalized;
            horizontal += tmp.x;
            vertical += tmp.y;
        }
        
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

        else if ((transform.position - lastKnownPosition).magnitude > 1f)
        {
            horizontal -= transform.position.x - lastKnownPosition.x;
            vertical -= transform.position.y - lastKnownPosition.y;
        }

        Vector2 PushVec = new Vector2(horizontal * runSpeed * Time.deltaTime, vertical * runSpeed * Time.deltaTime);
        Vector2 tmpVec = (body.velocity + PushVec);

        
        if (tmpVec.magnitude < 5f)
        { body.velocity = tmpVec; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("bullet") == true)
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
