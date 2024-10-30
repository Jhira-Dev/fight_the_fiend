using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Analytics;

public class Player : MovingObject
{
    // Adjustable variables from Inspector
    public int wepDamage = 1;
    public int pointsPerSmallPotion = 10;
    public int pointsPerBigpotion = 20;
    public int damageDoneToEnemy = 2;
    public bool hasWeapon;
    public bool hasKey = false;
    public int energyLevel;
    public static bool gamePaused;
    public float restartLevelDelay = 0.7f;

    // Variables which are assigned via the Inspector
    public Text energyText;
    public Text coinText;
    public Text keyText;
    public TMP_Text infoText;
    public GameObject fountainTop;
    public GameObject fountainBrick;
    public GameObject fountainTop2;
    public GameObject fountainBrick2;
    public GameObject sunSword;
    public GameObject currentWep;
    public GameObject barrier;
    public GameObject skull;
    public GameObject pedestal1;
    public GameObject pedestal2;
    public GameObject pedestal3;
    public GameObject purplePotion;
    public GameObject greenPotion;
    public GameObject redPotion;
    public GameObject arrowUp;
    public GameObject arrowDown;
    
    // Variables for game audio
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip pickUpSound1;
    public AudioClip coinSound1;
    public AudioClip coinSound2;
    public AudioClip keySound;
    public AudioClip chestOpenSound;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip teleportSound;
    public AudioClip gameOverSound;


    private Animator animator;
    private bool blessed;
    private GameObject currentTeleporter;
    private int skullCount;
    private int coinCounter;
    private int keyCounter;
    private string currentWepID;
    private bool armoured = false;
    private static List<string> oldSkulls = new List<string>() { "Skull of Ragnar the Headless found! ", "Skull of Cindy the Mysterious found!", "Skull of Rakkir the Merciful found! ", "Skull of Bran the Brilliant found! ", "Skull of Avalon the Brave found! ", "Skull of Filarion the Sureshot found! ", "Skull of Thorin the Selfless found! " };
    private static bool hasDagger;
    private static bool hasClub;
    private static bool hasAxe;
    private static bool hasSunSword;
    private static int killCounter;


    // Gets animator from Player GameObject and gets the current values of what the Player is holding and then calls the Start() function from MovingObject.cs. Adapted from code written by Matt Schell (2022).
    protected override void Start()
    {

        animator = GetComponent<Animator>();

        energyLevel = GameManager.instance.playerEnergyPoints;

        coinCounter = GameManager.instance.playerCoinCounter;

        keyCounter = GameManager.instance.playerKeyCounter;

        hasKey = GameManager.instance.playerHasKey;

        damageDoneToEnemy = GameManager.instance.playerDamage;

        armoured = GameManager.instance.playerIsArmoured;

        currentWepID = GameManager.instance.playerWeaponID;

        killCounter = GameManager.instance.playerKills;



        energyText.text = "" + energyLevel;

        coinText.text = "x " + coinCounter;

        keyText.text = "x " + keyCounter;

        arrowUp.SetActive(false);

        arrowDown.SetActive(false);


        base.Start();

    }

    // Stores the current variable values when transitioning between scenes. Adapted from code written by Matt Schell (2022).
    private void OnDisable()
    {
        GameManager.instance.playerEnergyPoints = energyLevel;

        GameManager.instance.playerCoinCounter = coinCounter;

        GameManager.instance.playerKeyCounter = keyCounter;

        GameManager.instance.playerHasKey = hasKey;

        GameManager.instance.playerWeaponID = currentWepID;

        GameManager.instance.playerDamage = damageDoneToEnemy;

        GameManager.instance.playerIsArmoured = armoured;

        GameManager.instance.playerKills = killCounter;
    }

    // Called by GameManager and checks what input the player has put in. Calculates what direction player has decided to move and calls AttemptMove().
    public void InputUpdate()
    {

        if (!GameManager.instance.playersTurn) return;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            gamePaused = true;

            GameManager.instance.PauseActivator();

        }

        if(gamePaused)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                GameManager.instance.GameOver();

            }

            else if (Input.GetKeyDown(KeyCode.E))
            {
                gamePaused = false;

                GameManager.instance.PauseDeactivator();

            }

        }


        if (!gamePaused)
        {

            Analytics.CustomEvent("Start_of_level");

            int horizontal = 0;
            int vertical = 0;

            horizontal = (int)(Input.GetAxisRaw("Horizontal"));

            vertical = (int)(Input.GetAxisRaw("Vertical"));


            if (horizontal != 0)
            {
                vertical = 0;
            }

            if (horizontal != 0 || vertical != 0)
            {
                

                AttemptMove<Enemy>(horizontal, vertical);

            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (currentTeleporter != null)
                {
                    transform.position = currentTeleporter.GetComponent<Teleporter>().GetDestination().position;

                    SoundManager.instance.RandomiseSfx(teleportSound);

                }

            }
        }

        if (Input.anyKey)
        {

            arrowUp.SetActive(false);

            arrowDown.SetActive(false);

        }


    }


    // Overrides MovingObject base class and works to check the player is not colliding with something using raycast. Also decreases food on movement and checks if energy <= 0. Adapted from code written by Matt Schell (2022).
    protected override void AttemptMove<T>(int xDir, int yDir)
    {

        if (GameManager.noMovement == false)
        {
            energyLevel--;

            //healthbar.Sethealth(energyLevel, maxEnergyLevel);

            energyText.text = "" + energyLevel;


            base.AttemptMove<T>(xDir, yDir);

            RaycastHit2D hit;

            if (Move(xDir, yDir, out hit))

            {
                SoundManager.instance.RandomiseSfx(moveSound1, moveSound2);

            }

            CheckIfGameOver();

            GameManager.instance.playersTurn = false;
        }

    }
    private void CompleteLevel()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);


    }

    // Main collision checker in game. Compares tag of collided object and then carries out specific action. Adapted from code written by Matt Schell (2022).
    private void OnTriggerEnter2D(Collider2D collision)
   {
        // If Player collides with exit GameObject then call the CompleteLevel() function to load the next scene.



        if (collision.CompareTag("Exit"))
        {
            Invoke(nameof(CompleteLevel), restartLevelDelay);

            enabled = false;

            Analytics.CustomEvent("End_of_level");

        }

        // Large potion increases energy a fair amount. Shows corresponding UI screen.

        if (collision.CompareTag("Energy up (big)"))
        {

            arrowUp.SetActive(true);

            energyLevel += pointsPerBigpotion;

            SoundManager.instance.RandomiseSfx(drinkSound1, drinkSound2);

            collision.gameObject.SetActive(false);

        }

        // Small potion increases energy slightly. Shows corresponding UI screen.

        else if (collision.CompareTag("Energy up (small)"))
        {

            arrowUp.SetActive(true);

            energyLevel += pointsPerSmallPotion;

            SoundManager.instance.RandomiseSfx(drinkSound1, drinkSound2);

            collision.gameObject.SetActive(false);

        }

        // Red fountain increases energy massively and blesses player so no other fountain can be used. Shows corresponding UI screen.

        else if (collision.CompareTag("Red Fountain") && blessed == false)
        {
            energyLevel *= 2;

            infoText.text = "Major blessing of Melora: energy massively increased!";

            GameManager.instance.InfoBoxActivator();

            blessed = true;

            SoundManager.instance.RandomiseSfx(drinkSound1, drinkSound2);

            collision.gameObject.SetActive(false);

            fountainTop.SetActive(false);

            fountainBrick.SetActive(false);


        }

        // Blue fountain boosts the players damage with any weapon. Shows corresponding UI screen.

        else if (collision.CompareTag("Blue Fountain") && blessed == false)
        {


            infoText.text = "Major blessing of Kord: damage massively increased!";

            GameManager.instance.InfoBoxActivator();

            blessed = true;

            SoundManager.instance.RandomiseSfx(drinkSound1, drinkSound2);

            collision.gameObject.SetActive(false);

            damageDoneToEnemy += 4;

            fountainTop2.SetActive(false);

            fountainBrick2.SetActive(false);
        }


        else if (collision.CompareTag("Wormhole"))
        {
            currentTeleporter = collision.gameObject;


        }

        // Chest opens and instantiates randomised loot. Only opens with key and removes key from player once opened. Shows corresponding UI screen.

        else if (collision.CompareTag("Chest") && hasKey == true)
        {

            collision.gameObject.GetComponent<Animator>().SetTrigger("OpenChestFull");

            hasKey = false;

            keyCounter--;

            SoundManager.instance.RandomiseSfx(chestOpenSound);

            int randomNumber = Random.Range(1, 100);

            Vector3 spawnLocation = collision.gameObject.transform.position;

            float newY = (float)(spawnLocation.y + -1);

            spawnLocation.y = newY;


            if (randomNumber <= 6)

            {
                Instantiate(sunSword, spawnLocation, Quaternion.identity);

            }

            else if (randomNumber <= 53 && randomNumber > 6)
            {

                Instantiate(purplePotion, spawnLocation, Quaternion.identity);
            }

            else if (randomNumber <= 100 && randomNumber > 53)

            {
                Instantiate(greenPotion, spawnLocation, Quaternion.identity);

            }

            keyText.text = "x " + keyCounter;


        }

        // Increments skull counter and spawns skulls on pedestal at top of floor - removes mist barrier at 3 skulls. Shows corresponding UI screen.

        else if (collision.CompareTag("Skull"))
        {


            skullCount++;

            int randomIndex = Random.Range(0, oldSkulls.Count);

            if (skullCount == 1)
            {
                Vector3 pedestal1Offset = pedestal1.transform.position;


                float newY = (float)(pedestal1Offset.y + 0.3);

                pedestal1Offset.y = newY;

                infoText.text = oldSkulls[randomIndex] + "(" + skullCount + "/3)";

                GameManager.instance.InfoBoxActivator();

                oldSkulls.RemoveAt(randomIndex);

                Instantiate(skull, pedestal1Offset, Quaternion.identity);

            }

            else if (skullCount == 2)
            {

                Vector3 pedestal2Offset = pedestal2.transform.position;


                float newY = (float)(pedestal2Offset.y + 0.3);

                pedestal2Offset.y = newY;


                infoText.text = oldSkulls[randomIndex] + "(" + skullCount + "/3)";

                GameManager.instance.InfoBoxActivator();

                oldSkulls.RemoveAt(randomIndex);


                Instantiate(skull, pedestal2Offset, Quaternion.identity);

            }


            else if (skullCount == 3)

            {
                Vector3 pedestal3Offset = pedestal3.transform.position;


                float newY = (float)(pedestal3Offset.y + 0.3);

                pedestal3Offset.y = newY;

                infoText.text = oldSkulls[randomIndex] + "(" + skullCount + "/3)";

                GameManager.instance.InfoBoxActivator();

                Instantiate(skull, pedestal3Offset, Quaternion.identity);

                oldSkulls.RemoveAt(randomIndex);

                barrier.SetActive(false);

                infoText.text = "The Shadow barrier has fallen!";

                GameManager.instance.InfoBoxActivator();

            }

            SoundManager.instance.RandomiseSfx(pickUpSound1);

            collision.gameObject.SetActive(false);

        }

        // Compares tag to check which weapon has been picked up. Destroys any current weapon in hand. Stores the weapons ID. Shows corresponding UI screen. Removes weapon collider.

        else if (collision.CompareTag("Club") || collision.CompareTag("Axe") || collision.CompareTag("Dagger") || collision.CompareTag("Sun Sword"))
        {
            Analytics.CustomEvent("Pickup_Weapon");

            if (hasWeapon == true)
            {
                Destroy(transform.GetChild(0).gameObject);

            }

            
            

            currentWepID = collision.tag;

            Vector3 offsetPosition = transform.position;


            float newX = (float)(offsetPosition.x + 0.4);

            float newY = (float)(offsetPosition.y + 0.6);


            offsetPosition.x = newX;
            offsetPosition.y = newY;


            currentWep = Instantiate(collision.gameObject, offsetPosition, Quaternion.Euler(new Vector3 (0, 0, -19)), transform);



            if (currentWep.CompareTag("Dagger") && !hasDagger)
            {

                infoText.text = "Picked up " + collision.tag;
                GameManager.instance.InfoBoxActivator();

                hasDagger = true;
            }
              
            else if(currentWep.CompareTag("Club") && !hasClub)
            {
                infoText.text = "Picked up " + collision.tag;
                GameManager.instance.InfoBoxActivator();

                hasClub = true;


            }

            else if (currentWep.CompareTag("Axe") && !hasAxe)
            {
                infoText.text = "Picked up " + collision.tag;
                GameManager.instance.InfoBoxActivator();

                hasAxe = true;


            }

            else if (currentWep.CompareTag("Sun Sword") && !hasSunSword)
            {
                infoText.text = "Picked up " + collision.tag;
                GameManager.instance.InfoBoxActivator();

                hasSunSword = true;


            }


            SoundManager.instance.RandomiseSfx(pickUpSound1);

            collision.gameObject.SetActive(false);


            hasWeapon = true;


            Collider2D wepCollider = transform.GetChild(0).GetComponent<Collider2D>();

            wepCollider.enabled = false;

        }

        // Increments coin counter. On 6 coins the Sun Sword is spawn in front of player to pick up. Shows corresponding UI screen.

        else if (collision.CompareTag("Coin"))
        {
            coinCounter++;

            coinText.text = "x " + coinCounter;

            if (coinCounter == 1)

            { infoText.text = coinCounter + " coin collected";

                GameManager.instance.InfoBoxActivator();
            }

            else
            {

                infoText.text = coinCounter + " coins collected";
                GameManager.instance.InfoBoxActivator();
            }

            if(coinCounter == 6)
            {

                infoText.text = "Pelor has gifted you the Sun Sword!";

                GameManager.instance.InfoBoxActivator();

                Vector3 spawnLocation = transform.position;

                float newX = (float)(spawnLocation.x + 1);

                spawnLocation.x = newX;

                Instantiate(sunSword, spawnLocation, Quaternion.identity);

            }

            SoundManager.instance.RandomiseSfx(coinSound1, coinSound2);

            collision.gameObject.SetActive(false);
        }

        // Picks up Key. Shows corresponding UI screen.

        else if (collision.CompareTag("Key"))

        {
            keyCounter++;

            hasKey = true;
            keyText.text = "x " + keyCounter;

            SoundManager.instance.RandomiseSfx(keySound);

            infoText.text = keyCounter + " key collected";
            GameManager.instance.InfoBoxActivator();

            collision.gameObject.SetActive(false);


        }

        // Calls function that decreases current player energy.

        else if(collision.CompareTag("FloorSpikes"))
        {

            LoseEnergy(10);

        }

        // Increases player weapon damage slight. Shows corresponding UI screen.

        else if (collision.CompareTag("DamagePotion"))

        {

            damageDoneToEnemy += 2;

            infoText.text = "Minor blessing of Kord: Damage slightly increased!";
            GameManager.instance.InfoBoxActivator();

            SoundManager.instance.RandomiseSfx(drinkSound1, drinkSound2);

            collision.gameObject.SetActive(false);

        }

        // Decreases amount of damage taken slightly by any enemy. Shows corresponding UI screen.

        else if (collision.CompareTag("DefensePotion"))

        {
            armoured = true;

            infoText.text = "Minor blessing of Moradin: Armoured against enemy attacks!";
            GameManager.instance.InfoBoxActivator();

            SoundManager.instance.RandomiseSfx(drinkSound1, drinkSound2);

            collision.gameObject.SetActive(false);

        }
        


    }



    // Leaving a collision causes the current teleporter to be set to null. Adapted from code written by Bendux (2021).
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Wormhole"))
        {
            if (collision.gameObject == currentTeleporter)
            {
                currentTeleporter = null;
            }
        }
    }

    // Increases player energy when two enemies are killed. Shows corresponding UI screen.

    public void EnergyOnKill()

    {

        killCounter++;
        if (killCounter % 2 == 0)
        {
            arrowUp.SetActive(true);
            energyLevel += 10;
        }

    }




    // Checks if player is armoured from potion and then on any collision that would reduce energy this function is called to decrease from current player energy. Shows corresponding UI screen. Adapted from code written by Matt Schell (2022).

    public void LoseEnergy(int loss)

  {
        arrowDown.SetActive(true);


        if (armoured == true)
        {
            loss -= 5;

        }
        animator.SetTrigger("playerHit");

        energyLevel -= loss;

        CheckIfGameOver();

    }

    // Overriding MovingObject base class to attack enemy, damage and effect is dependent on weapon equipped.
    protected override void onCantMove<T>(T component)
    {

        Enemy hitEnemy = component as Enemy;

        
        if (hasWeapon == true)

        {
            // Checks for club weapon and then checks what direction the enemy was attacked from. Then attempts to push enemy in opposite direction to attack by calling ClubAttack() in Enemy script.

            if(currentWep.CompareTag("Club"))
            {

                hitEnemy.HitByPlayer(damageDoneToEnemy);

                animator.SetTrigger("playerHit");

                if ((hitEnemy.transform.position.x - transform.position.x) == -1)
                {
                    //left

                    hitEnemy.ClubAttack('x', -1);

                }

                if ((hitEnemy.transform.position.x - transform.position.x) == 1)

                {
                    //right

                    hitEnemy.ClubAttack('x', 1);

                }

                if ((hitEnemy.transform.position.y - transform.position.y) == -1)
                {
                    //down

                    hitEnemy.ClubAttack('y', -1);

                }
                if ((hitEnemy.transform.position.y - transform.position.y) == 1)
                {
                    //up

                    hitEnemy.ClubAttack('y', 1);

                }


            }

            // If attack with axe then set the enemy to have bleeding condition by calling AxeAttack() in Enemy script.

            else if(currentWep.CompareTag("Axe"))
            {
                hitEnemy.HitByPlayer(damageDoneToEnemy);

                animator.SetTrigger("playerHit");

                hitEnemy.AxeAttack();

               

            }

            // If Dagger attack then call HitByplayer() from Enemy script and increase current player energy by 3.

            else if(currentWep.CompareTag("Dagger"))
            {

                arrowUp.SetActive(true);

                hitEnemy.HitByPlayer(damageDoneToEnemy);

                energyLevel += 3;

                
            }

            // If Sun Sword attack then call each corresponding function as though attacked by all 3 weapons above.

            else if (currentWep.CompareTag("Sun Sword"))

            {
                hitEnemy.HitByPlayer(damageDoneToEnemy);

                animator.SetTrigger("playerHit");

                if ((hitEnemy.transform.position.x - transform.position.x) == -1)
                {
                    //left

                    hitEnemy.ClubAttack('x', -1);

                }

                if ((hitEnemy.transform.position.x - transform.position.x) == 1)

                {
                    //right

                    hitEnemy.ClubAttack('x', 1);

                }

                if ((hitEnemy.transform.position.y - transform.position.y) == -1)
                {
                    //down

                    hitEnemy.ClubAttack('y', -1);

                }
                if ((hitEnemy.transform.position.y - transform.position.y) == 1)
                {
                    //up

                    hitEnemy.ClubAttack('y', 1);

                }

                arrowUp.SetActive(true);

                hitEnemy.AxeAttack();

                energyLevel += 3;

            }



        }
            

    }

    // Checks whether the game should end and then does so if needed. Adapted from code written by Matt Schell (2022).

    private void CheckIfGameOver()
    {
        if (energyLevel <= 0)
        {
            List<string> oldSkulls = new List<string>() { "Skull of Ragnar the Headless found! ", "Skull of Cindy the Mysterious found!", "Skull of Rakkir the Merciful found! ", "Skull of Bran the Brilliant found! ", "Skull of Avalon the Brave found! ", "Skull of Filarion the Sureshot found! ", "Skull of Thorin the Selfless found! " };

            SoundManager.instance.PlaySingle(gameOverSound);

            SoundManager.instance.musicSource.Stop();

            GameManager.instance.GameoOverScreen();

        }

    }
}

// Code from Matt Schell (2022) can be accessed from: https://learn.unity.com/project/2d-roguelike-tutorial?uv=5.x
// Code from Bendux (2021) can be accessed from: https://www.youtube.com/watch?v=0JXVT28KCIg