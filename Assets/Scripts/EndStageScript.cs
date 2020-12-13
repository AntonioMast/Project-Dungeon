/***************************************************************
 * This code is applied to end stage prefab objects.
 * This script is used to ensure that the player placed
 * a a endpoint for the A.I. to find before they attempt to
 * play their level.
 **************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndStageScript : MonoBehaviour
{
    float timer = 0.25f;

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        { timer -= Time.deltaTime; }
        if (timer <= 0 && SceneManager.GetActiveScene().name == "GameSceen") //lets the user play the level once the stage piece is confirmed
        {
            GameObject.Find("PlayButton").GetComponent<ProcessToAIScene>().endPointExists = true;
        }
    }
}
