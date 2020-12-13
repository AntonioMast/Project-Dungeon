/***************************************************************
 * This code is applied to bullet prefab objects.
 * This scripts handles bullet collision and despawning.
 **************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    //*********************
    //Variable Declarations
    //*********************
    float timer = 0.0f;

    //this function runs once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer % 60 > 1f)//delete self after 1 second
        { Destroy(gameObject); }
    }

    //this functions runs when an object enters this objects collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Floor" || collision.gameObject.tag.Contains("bullet") == true)//destroys the bullet if it comes in contact with a floor or another bullet
        {
            Destroy(gameObject);
        }
    }
}
