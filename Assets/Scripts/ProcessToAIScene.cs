/************************************************************************
 * This code is applied to the play button on the level creation screen.
 * The purpose of this script is to change the scene the main game scene
 ***********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProcessToAIScene : MonoBehaviour
{
    //*********************
    //Variable Declarations
    //*********************
    public GameObject Stage;
    public bool endPointExists = false;

    //this script runs when the play button is clicked
    public void NextScene()
    {
        if (endPointExists)
        {
            DontDestroyOnLoad(Stage);
            SceneManager.LoadScene(2);
        }
    }
}
