using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public AudioSource efxSource;
    public AudioSource musicSource;
    public static SoundManager instance = null;

    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;


    // Makes sure to only have one instance of SoundManager active to enfore singleton. Adapted from code written by Matt Schell (2022).


    void Awake()
    {
        if(instance == null)
        {

            instance = this;
        }
        else if (instance !=null)
        {
            Destroy(gameObject);

        }

        DontDestroyOnLoad(gameObject);


    }

    //Plays single sound effects.

    public void PlaySingle(AudioClip clip) 

    {
        efxSource.clip = clip;
        efxSource.Play();

    }

    // Randomises audioclips that are passed through.

    public void RandomiseSfx (params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);

        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();

    }

    
}

// Code from Matt Schell (2022) can be accessed from: https://learn.unity.com/project/2d-roguelike-tutorial?uv=5.x