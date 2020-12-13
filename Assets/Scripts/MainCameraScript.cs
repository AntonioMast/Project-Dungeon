/***************************************************************
 * This code is applied to the camera object in the game screen.
 * This code handles camerma movement and smoothing, making sure
 * that the camera always follows the A.I. opponent and that
 * the player has some control with the camera.
 **************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraScript : MonoBehaviour
{
    //*********************
    //Variable Declarations
    //*********************

    public Transform target;
    public float smoothTime = 1f;
    float mouseWheel;
    float newPosition = 0;
    Vector3 move;

    public float dragSpeed = 10;
    private Vector3 dragOrigin;

    private Vector3 velocity = Vector3.zero;

    //this functions runs once per frame
    void Update()
    {
        ZoomCamera();
        DragCamera();

        //smoothly moves the camera to the desired location
        Vector3 goalPos;
        goalPos.x = target.position.x;
        goalPos.y = target.position.y;
        goalPos.z = -10;
        transform.position = Vector3.SmoothDamp((transform.position+move), goalPos, ref velocity, smoothTime);
    }

    //this function runs once per frame and allows the player the drag the camera from the initial anchor (which is the A.I. opponent)
    void DragCamera()
    {
        if (Input.GetMouseButtonDown(1)) //if the player is holding right click
        {
            dragOrigin = Input.mousePosition; //saves the position the player is holding the mouse relative to the game world
            return;
        }

        else if (!Input.GetMouseButton(1)) //if the player is holding right click then it exits the function
        { return; }

        //calculates where to move the camera
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        move = new Vector3(-pos.x * dragSpeed, -pos.y * dragSpeed, 0);

        transform.Translate(move, Space.World);
    }

    //this function runs once per frame and allows the player to zoom the camera in and out
    void ZoomCamera()
    {
        //gets and saves the value of the mouse's scroll wheel
        mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        mouseWheel = mouseWheel * 3;
        newPosition = mouseWheel + newPosition;

        GetComponent<Camera>().orthographicSize -= newPosition;

        //zooms in the camera with the scroll wheel if it is not already the max
        if (GetComponent<Camera>().orthographicSize < 5f)
            GetComponent<Camera>().orthographicSize = 5f;

        //zooms out the camera with the scroll wheel if it is not already the max
        else if (GetComponent<Camera>().orthographicSize > 32f)
            GetComponent<Camera>().orthographicSize = 32f;

        //smooths the camera's zoom
        newPosition = newPosition / 1.15f;
        if (Mathf.Abs(newPosition) < 0.2f)
        { newPosition = 0; }

    }

}
