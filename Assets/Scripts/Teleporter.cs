using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Script which is called if the player presses 'e' when on top of a teleporter. Returns the destination of other teleporter for the player to be taken to. Adapted from code written by Bendux (2021).

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform destination;

    public Transform GetDestination()
    {
        return destination;
    }
    
}

// Code from Bendux (2021) can be accessed from: https://www.youtube.com/watch?v=0JXVT28KCIg