/***************************************************************
 * This code can be applied to any object, as it directly modifies
 * the game objects given to it through Unity editor.
 * The purpose of this script is to read high scores and give
 * them to the corresponding game object to display on screen.
 **************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardScript : MonoBehaviour
{
    //*********************
    //Variable Declarations
    //*********************
    public GameObject currentScoreObj;
    Text currentScoreText;
    public GameObject highScore1stObj;
    Text highScore1stText;
    public GameObject highScore2ndObj;
    Text highScore2ndText;
    public GameObject highScore3rdObj;
    Text highScore3rdText;

    //This function only runs when the object is created
    //it is used to set variables to initial values
    void Start()
    {
        currentScoreText = currentScoreObj.GetComponent<Text>();
        highScore1stText = highScore1stObj.GetComponent<Text>();
        highScore2ndText = highScore2ndObj.GetComponent<Text>();
        highScore3rdText = highScore3rdObj.GetComponent<Text>();

        currentScoreText.text = PlayerPrefs.GetInt("currentScore", 0).ToString();
        highScore1stText.text = PlayerPrefs.GetInt("HighScore1st", 0).ToString();
        highScore2ndText.text = PlayerPrefs.GetInt("HighScore2nd", 0).ToString();
        highScore3rdText.text = PlayerPrefs.GetInt("HighScore3rd", 0).ToString();
    }
}
