/***************************************************************
 * This code is applied to stage objects.
 * It handles trivialthings needed with the stages
 **************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageScript : MonoBehaviour
{
    //*********************
    //Variable Declarations
    //*********************
    public bool staySpawned;
    int counterSinceSpawn;

    //This function only runs when the object is created
    //it is used to set variables to initial values
    void Start()
    {
        staySpawned = false;
        counterSinceSpawn = 5;

        if (this.gameObject.name == "StageStartingArea")
        {
            staySpawned = true;
        }
    }

    //This script runs sixty times per a second.
    void FixedUpdate()
    {
        //checks if a stage is spawned in the correction location. If not, it is deleted.
        if (counterSinceSpawn > 0)
        {
            counterSinceSpawn -= 1;
        }

        else if (staySpawned == false)
        {
            Destroy(gameObject);
        }
    }
}
