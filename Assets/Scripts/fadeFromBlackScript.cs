/***************************************************************
 * This code can be applied to the black image object.
 * The purpose of this script is to fade the black image out
 * or in over time.
 **************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fadeFromBlackScript : MonoBehaviour
{
    //*********************
    //Variable Declarations
    //*********************
    public bool fadeFromBlack;
    float alphaValue = 1f;
    public GameObject blackImage;
    public Color tempColor = Color.black;

    //This function only runs when the object is created
    //it is used to set variables to initial values
    void Start()
    {
        fadeFromBlack = true; //the purpose of setting the variable to true here is so that the value can change easily in editor
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeFromBlack == true) //if it should fade from black
        {
            tempColor.a = alphaValue;
            blackImage.gameObject.GetComponent<Image>().color = tempColor;

            if (alphaValue > 0f) //if the image isn't fully transparent, lower the image alpha value
            {
                alphaValue -= Time.unscaledDeltaTime;
                if (alphaValue < 0f)
                    alphaValue = 0f;
            }
        }

        else //if it should fade to black
        {
            tempColor.a = alphaValue;
            blackImage.gameObject.GetComponent<Image>().color = tempColor;

            if (alphaValue < 1f) //if the image isn't fully opaque, increase the image alpha value
            {
                alphaValue += Time.unscaledDeltaTime;
                if (alphaValue > 1f)
                    alphaValue = 1f;
            }
        }
    }
}