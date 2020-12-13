/************************************************************************
 * This code is applied to the refresh button on the level creation screen.
 * The purpose of this script is to refresh the scene
 ***********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class refreshScreen : MonoBehaviour
{
    //this script reloads the level designing scene
    public void ReloadDesignStage()
    {
        SceneManager.LoadScene(1);
    }
}
