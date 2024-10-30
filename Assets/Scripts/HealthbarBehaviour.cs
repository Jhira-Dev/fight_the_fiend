using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarBehaviour : MonoBehaviour
{
    public Slider slider;
    public Image bloodSplat;
    public Color low;
    public Color high;
    public Vector3 sliderOffset;
    public Vector3 bloodOffset;


    // Turn off blood splatter at start of game.

    public void Start()
    {
        bloodSplat.gameObject.SetActive(false);
    }

    // Set the health of the enemy using the passed through parameters and also set the colour gradient of the healthbar. Adapted from code written by Distorted Pixel Studios (2020).
    public void Sethealth(float health, float maxHealth)
    {
        slider.gameObject.SetActive(health < maxHealth);
        slider.value = health;
        slider.maxValue = maxHealth;

        slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(low, high, slider.normalizedValue);

    }

    // Show the bleed effect if function is called.

    public void BleedEffect()
    {
        bloodSplat.gameObject.SetActive(true);

    }

    // Update the health of the enemy every time this is called by the GameManager Update() function. Adapted from code written by Distorted Pixel Studios (2020).
   
    public void HealthUpdate()
    {

        
        bloodOffset.x = (float) -0.9;
        

        slider.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + sliderOffset);

        bloodSplat.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + bloodOffset);
        
    }
}

// Code from Distorted Pixel Studios (2020) can be accessed from: https://www.youtube.com/watch?v=v1UGTTeQzbo