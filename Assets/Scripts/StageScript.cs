using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageScript : MonoBehaviour
{
    public bool staySpawned;
    int counterSinceSpawn;

    // Start is called before the first frame update
    void Start()
    {
        staySpawned = false;
        counterSinceSpawn = 5;

        if (this.gameObject.name == "StageStartingArea")
        {
            staySpawned = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
