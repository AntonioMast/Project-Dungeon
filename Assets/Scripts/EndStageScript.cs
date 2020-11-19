using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndStageScript : MonoBehaviour
{
    float timer = 0.25f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        { timer -= Time.deltaTime; }
        if (timer < 0 && SceneManager.GetActiveScene().name == "GameSceen")
        {
            GameObject.Find("PlayButton").GetComponent<ProcessToAIScene>().endPointExists = true;
        }
    }
}
