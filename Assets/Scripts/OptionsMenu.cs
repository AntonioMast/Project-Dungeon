/***************************************************************
 * This code is applied to the options menu button.
 * The scripts purpose is to handle operations needed in the
 * options menu.
 **************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    //This script handles the fullscreen toggle button in the options menu
    public void ToggleFullscreen()
    {
        // Toggle fullscreen
        Screen.fullScreen = !Screen.fullScreen;

        if (Screen.fullScreen == false) { Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true); }
        else { Screen.SetResolution(1280, 720, false); }
    }
}
