using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour

{

    public GameObject gameManager;
    public GameObject soundManager;

    // Checks if an instance of either GameManager or SoundManager is null then instantiates if they are. Adapted from code written by Matt Schell (2022).  

    void Awake()
    {

        if(GameManager.instance == null)
        {
            Instantiate(gameManager);
        }

        if (SoundManager.instance == null)
        { 
            
            Instantiate(soundManager);
        }


}
 
}

// Code from Matt Schell (2022) can be accessed from: https://learn.unity.com/project/2d-roguelike-tutorial?uv=5.x