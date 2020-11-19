using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProcessToAIScene : MonoBehaviour
{
    public GameObject Stage;
    public bool endPointExists = false;

    public void NextScene()
    {
        if (endPointExists)
        {
            DontDestroyOnLoad(Stage);
            SceneManager.LoadScene(2);
        }
    }
}
