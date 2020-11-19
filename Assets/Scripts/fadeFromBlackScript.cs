using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fadeFromBlackScript : MonoBehaviour
{
    public bool fadeFromBlack;
    float alphaValue = 1f;
    public GameObject blackImage;
    public Color tempColor = Color.black;

    // Start is called before the first frame update
    void Start()
    {
        fadeFromBlack = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeFromBlack == true)
        {
            tempColor.a = alphaValue;
            blackImage.gameObject.GetComponent<Image>().color = tempColor;


            if (alphaValue > 0f)
            {
                alphaValue -= Time.unscaledDeltaTime;
                if (alphaValue < 0f)
                    alphaValue = 0f;
            }
        }

        else
        {
            tempColor.a = alphaValue;
            blackImage.gameObject.GetComponent<Image>().color = tempColor;


            if (alphaValue < 1f)
            {
                alphaValue += Time.unscaledDeltaTime;
                if (alphaValue > 1f)
                    alphaValue = 1f;
            }

        }
    }

}