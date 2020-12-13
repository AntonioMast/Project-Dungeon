/***************************************************************
 * This code is flexible and to be applied to random gameobjects.
 * This scripts handles objects deleting when colliding with a bullet
 **************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class defaultObjectScript : MonoBehaviour
{
    //this functions runs when an object enters this objects collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("bullet")) //deletes the object and what it collided with if it collided with a bullet
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
