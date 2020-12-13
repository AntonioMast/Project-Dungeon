/***************************************************************
 * This code is applied to bullet prefab objects.
 * This scripts handles bullet collision and despawning.
 **************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //This function only runs when the object is created
    //it is used to set variables to initial values
    void Start()
    {
        if (PlayerPrefs.HasKey("VolumeValue") == false) //attempts to get the volume used on last load, sets volume to default if nothing returns
        {
            PlayerPrefs.SetFloat("VolumeValue", 0.6f);
        }

        AudioListener.volume = PlayerPrefs.GetFloat("VolumeValue");
    }

    //this function run when the play button is clicked
    public void PlayGame()
    {
        SceneManager.LoadScene(1); //switches the scene
    }

    //this function runs when the exit button is clicked
    public void QuitGame()
    {
        Application.Quit(); //stops the game program
    }
}
