/***************************************************************
 * This code can be applied to the volume slider object.
 * The purpose of this script is to handle the user changing 
 * the volume.
 **************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderScript : MonoBehaviour
{
    //variable declaration
    public Slider slider;

    //This function only runs when the object is created
    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("VolumeValue");
    }

    //this function runs when the player changes the volume slider.
    //it handles setting the actual volume to the value of the slider.
    public void changeVolume(float newVolume)
    {
        AudioListener.volume = newVolume;
        PlayerPrefs.SetFloat("VolumeValue", newVolume);
    }

    //this functions runs when the user exits the options menu.
    //it saves the volume value to the players harddrive to keep the volume for the next time the game is run
    public void savePlayerPrefs()
    {
        PlayerPrefs.Save();
    }
}
