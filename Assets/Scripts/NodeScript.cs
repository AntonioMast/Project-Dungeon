/***************************************************************
 * This code is applied to bullet A.I. 'Nodes', which are used
 * for pathing.
 * This scripts handles trival tasks needed for the pathing
 * system to work.
 **************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour
{
    //*********************
    //Variable Declarations
    //*********************
    public bool firstnode = false;
    public bool unmarked;
    public bool completelyUsed = false;

    //This function only runs when the object is created
    //it is used to set variables to initial values
    void Start()
    {
        completelyUsed = false;
    }
}
