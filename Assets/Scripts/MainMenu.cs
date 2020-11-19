using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.HasKey("VolumeValue") == false)
        {
            PlayerPrefs.SetFloat("VolumeValue", 0.6f);
        }

        AudioListener.volume = PlayerPrefs.GetFloat("VolumeValue");
    }

    public void PlayGame ()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
