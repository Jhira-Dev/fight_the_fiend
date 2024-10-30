using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{

    public GameObject overlay;
    public AudioMixer audioMixer;
    private static float currentVolume;
    public static bool tutorialTips = true;
    public GameObject slider;
    public GameObject toggler;

    // At start of game call Main Menu show function.

    private void Awake()    
    {

        ShowLaunchScreen();
    }


    // If Main Menu is reloaded then set values of volume and tutorial toggle to what was picked before playing. Adapted from code written by Brackeys (2017).
    private void Start()
    {
        slider.GetComponent<Slider>().SetValueWithoutNotify(currentVolume);

        toggler.GetComponent<Toggle>().SetIsOnWithoutNotify(tutorialTips);
        
    }

    // Show the Main Menu screen and pause the game.

    public void ShowLaunchScreen()
    {

        Time.timeScale = 0f;
        overlay.SetActive(true);
    }

    // If the Play button is pressed then disable the Main Menu screen and resume game time. Adapted from code written by Brackeys (2017).

    public void Play()
    {
        overlay.SetActive(false);
        Time.timeScale = 1f;
        
        
    }

    // If Quit button is pressed then exit from current application. Adapted from code written by Brackeys (2017).

    public void Quit()
    {
        Application.Quit();

    }

    // Take the value of the interactble slider GameObject and set that value as the current volume. Adapted from code written by Brackeys (2017).

    public void SetVolume(float volume)

    {
        currentVolume = volume;

        audioMixer.SetFloat("volume", currentVolume);

    }    

    // Take the bool value of the GameObject tutorial toggle and set the value to tutorialTips variable.

    // Code adapted from Kelly (2015)
    public void TutorialToggle(bool tog)
    {

        tutorialTips = tog;

    }
}

// Code from Brackeys (2017) can be accessed from: https://www.youtube.com/watch?v=YOaYQrN1oYQ

// Code from Kelly, S. (2015) can be accessed from: https://www.youtube.com/watch?v=0ewSSlTG2xo