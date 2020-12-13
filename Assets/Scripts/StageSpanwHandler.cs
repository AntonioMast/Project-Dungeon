/***************************************************************
 * This code is applied to the game handler object.
 * It handles major things needed with the stages such as with
 * stage creation.
 **************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class StageSpanwHandler : MonoBehaviour
{
    //*********************
    //Variable Declarations
    //*********************
    GameObject Stage;
    public GameObject Stage1;
    public GameObject Stage2;
    public GameObject Stage3;
    public GameObject Stage4;
    public GameObject Stage5;
    public GameObject Stage6;
    public GameObject Stage7;
    public GameObject Stage8;

    //This function only runs when the object is created
    //it is used to set variables to initial values
    void Start()
    {
        Stage = null; //sets the index to null
    }

    //this function runs once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Input.mousePosition.x < Screen.width/1.3 && SceneManager.GetActiveScene().name == "GameSceen")//gets the location relative to the 
        {                                                                                                                                     //gameworld that the player clicks
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
            Vector3 finalVec = new Vector3(worldPoint.x, worldPoint.y, 0.5f);
            SpawnStage(finalVec);
        }
    }

    //this script is called when the mouse is clicked in the stage creation area--it handles spawning the correct stage.
    public void SpawnStage(Vector3 mPosition)
    {
        GameObject tmp = Instantiate(Stage) as GameObject;
        tmp.transform.position = mPosition;
        tmp.transform.parent = GameObject.Find("GameController").transform;
    }

    //*************************************************************
    //This function determines which stage to spawn on mouse click
    //-1 = none
    //0 - starting stage
    //1 - default
    //2 - horizontal straight
    //3 - vertical straight
    //4 - corner
    //5 - corner
    //6 - corner
    //7 - corner
    //*************************************************************
    public void  SetStageIndex(int i)
    {
        switch (i)
        {
            case 0:
                Stage = null;
                break;
            case 1:
                Stage = Stage1;
                break;
            case 2:
                Stage = Stage2;
                break;
            case 3:
                Stage = Stage3;
                break;
            case 4:
                Stage = Stage4;
                break;
            case 5:
                Stage = Stage5;
                break;
            case 6:
                Stage = Stage6;
                break;
            case 7:
                Stage = Stage7;
                break;
            case 8:
                Stage = Stage8;
                break;
            default:
                Stage = null;
                break;
        }
    }
}
