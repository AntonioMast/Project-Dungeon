/***************************************************************
 * This code can be applied to multiple game objects.
 * The purpose of this script is to handle the player returning
 * to the menu.
 **************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class returnToMenu : MonoBehaviour
{
    //*********************
    //Variable Declarations
    //*********************
    public GameObject Stage;
    bool foundStage;

    //This function only runs when the object is created
    void Start()
    {
        foundStage = false;
        Stage = GameObject.Find("GameController");
        if (Stage != null)
            { foundStage = true; }
    }

    //this functions runs when called,such as when clicking a button--it switches the scene to the main menu.
    public void SceneToMenu()
    {
        if (foundStage = true)
            { Destroy(Stage); }
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
