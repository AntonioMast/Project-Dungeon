using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class returnToMenu : MonoBehaviour
{
    public GameObject Stage;
    bool foundStage;

    void Start()
    {
        foundStage = false;
        Stage = GameObject.Find("GameController");
        if (Stage != null)
            { foundStage = true; }
    }

    public void SceneToMenu()
    {
        if (foundStage = true)
            { Destroy(Stage); }
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
