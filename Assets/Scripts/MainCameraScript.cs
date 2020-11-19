using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraScript : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 1f;
    float mouseWheel;
    float newPosition = 0;
    Vector3 move;

    public float dragSpeed = 10;
    private Vector3 dragOrigin;

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        ZoomCamera();
        DragCamera();

        Vector3 goalPos;
        goalPos.x = target.position.x;
        goalPos.y = target.position.y;
        goalPos.z = -10;
        transform.position = Vector3.SmoothDamp((transform.position+move), goalPos, ref velocity, smoothTime);
    }

    void FixedUpdate()
    {


    }

    void DragCamera()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1))
        { return; }

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        move = new Vector3(-pos.x * dragSpeed, -pos.y * dragSpeed, 0);

        transform.Translate(move, Space.World);
    }

    void ZoomCamera()
    {

        mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        mouseWheel = mouseWheel * 3;
        newPosition = mouseWheel + newPosition;

        GetComponent<Camera>().orthographicSize -= newPosition;

        if (GetComponent<Camera>().orthographicSize < 5f)
            GetComponent<Camera>().orthographicSize = 5f;

        else if (GetComponent<Camera>().orthographicSize > 32f)
            GetComponent<Camera>().orthographicSize = 32f;



        newPosition = newPosition / 1.15f;
        if (Mathf.Abs(newPosition) < 0.2f)
        { newPosition = 0; }

    }

}
