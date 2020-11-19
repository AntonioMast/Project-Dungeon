using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{

    public void ToggleFullscreen()
    {
        // Toggle fullscreen
        Screen.fullScreen = !Screen.fullScreen;

        if (Screen.fullScreen == false) { Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true); }
        else { Screen.SetResolution(1280, 720, false); }
    }
}
