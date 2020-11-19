using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class StageSpanwHandler : MonoBehaviour
{

    GameObject Stage;
    public GameObject Stage1;
    public GameObject Stage2;
    public GameObject Stage3;
    public GameObject Stage4;
    public GameObject Stage5;
    public GameObject Stage6;
    public GameObject Stage7;
    public GameObject Stage8;

    // Start is called before the first frame update
    void Start()
    {
        Stage = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Input.mousePosition.x < Screen.width/1.3 && SceneManager.GetActiveScene().name == "GameSceen")
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);

            Vector3 finalVec = new Vector3(worldPoint.x, worldPoint.y, 0.5f);

            SpawnStage(finalVec);
        }

    }

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
    //3+ - ????
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
