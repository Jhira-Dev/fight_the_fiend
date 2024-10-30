using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    // Camera is updated by GameManager update call. The camera moves itself to a fixed position to always show the player and follow them. Adapted from code written by Shinyclef (2015).
    public Transform player;
    public Vector3 offset;


    
    public void CameraUpdate()
    {
        transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z);

        
    }
}

// Code from Shinyclef (2015) can be accessed from: https://answers.unity.com/questions/878913/how-to-get-camera-to-follow-player-2d.html