using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;



public class GameManager : MonoBehaviour

//Public variables allows for dragging and dropping of certain variable values.
{
    public float levelStartDelay = 2f;
    public float gameOverScreenDelay = 4f;
    public float turnDelay = 0.1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public Player playerScript;
    public HealthbarBehaviour healthbar;


    public int playerEnergyPoints = 100;
    public int playerCoinCounter;
    public int playerKeyCounter;
    public int playerDamage;
    public bool playerHasKey;
    public bool playerIsArmoured;
    public int playerKills;
    public GameObject playerWeaponHolder;
    public string playerWeaponID;
    public GameObject dagger;
    public GameObject club;
    public GameObject axe;
    public GameObject goldenSword;
    public static bool noMovement;
    public int level = 0;

    [HideInInspector] public bool playersTurn = true;

    
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;
    private List<string> goodDefeatQuotes;
    private List<string> badDefeatQuotes;
    private GameObject levelImage;
    private TMP_Text levelText;
    private TMP_Text gameWinText;
    private GameObject goodQuotes;
    private TMP_Text goodText;
    private GameObject badQuotes;
    private TMP_Text badText;
    private GameObject infoImage;
    private GameObject pauseImage;
    private GameObject toolTip;
    private bool toolTipPause;
    private bool infoImageVisible;
    private CameraFollower cameraUpdater;
    private static bool firstStart;



    //Check to make sure there is only one instance of gamemanager and enforces singleton on script. Adapted from code written by Matt Schell (2022), link to original code at bottom. 
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)

        {
            
            Destroy(gameObject);

        }

        DontDestroyOnLoad(gameObject);


        enemies = new List<Enemy>();

        goodDefeatQuotes = new List<string>();

        badDefeatQuotes = new List<string>();

        // String list of end screen quotes which are initialised.
        string[] goodQuotes = { "You will not fail if it is worth it… do not give up!", "We are what we overcome!", "Improvise, adapt, overcome.", "Never surrender!", "Every river reaches the ocean in its own way.", "Ah, new beginnings!", "Do not tire, do not slow, push onward.", "There's nowhere left to go but up.", "Victory is sweetest when you have known defeat." };

        goodDefeatQuotes.AddRange(goodQuotes);

        string[] badQuotes = { "You come once more for death. When will you end your suffering.", "On and on you go. Come I will send you to oblivion!", "Give up.", "What do all stories have in common?...They end." };

        badDefeatQuotes.AddRange(badQuotes);


        // Check to load the first floor annd only the first floor, then being first set up.
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0) && firstStart == false)

        {
            firstStart = true;
            instance.boardScript = Object.FindObjectOfType<BoardManager>();
         
            InitGame();
        }

    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]

    static public void CallbackInitialisation()
    {

        SceneManager.sceneLoaded += OnSceneLoaded;
       
    }

    // Called every time a scene is loaded to increment the level number and call the initialising function. Adapted from code written by Matt Schell (2022).
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))

        {
            noMovement = false;
            instance.level = 0;

        }

        instance.level++;
        instance.boardScript = Object.FindObjectOfType<BoardManager>();
        instance.InitGame();

    }

    // Function to set up UI screen, also handles weapon ID system and then calls the scene set up function in BoardManager. Adapted from code written by Matt Schell (2022).
    void InitGame()
    {

        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");

        levelText = GameObject.Find("LevelText").GetComponent<TMP_Text>();

        gameWinText = GameObject.Find("GameWinText").GetComponent<TMP_Text>();

        goodQuotes = GameObject.Find("GoodQuotes");

        goodText = GameObject.Find("GoodText").GetComponent<TMP_Text>();

        badQuotes = GameObject.Find("BadQuotes");

        badText = GameObject.Find("BadText").GetComponent<TMP_Text>();

        infoImage = GameObject.Find("InfoImage");

        pauseImage = GameObject.Find("PauseImage");

        toolTip = GameObject.Find("Tooltip");

        cameraUpdater = Object.FindObjectOfType<CameraFollower>();

        


        goodQuotes.SetActive(false);

        badQuotes.SetActive(false);

        infoImage.SetActive(false);

        pauseImage.SetActive(false);

        toolTip.SetActive(false);

        levelText.text = "Floor " + level;

        levelImage.SetActive(true);

        Invoke(nameof(HideLevelImage), levelStartDelay);



        enemies.Clear();

        if (playerWeaponID != "")
        {
            if (dagger.CompareTag(playerWeaponID))
            {
                boardScript.WeaponSpawn(dagger);
            }

            else if (club.CompareTag(playerWeaponID))
            {
                boardScript.WeaponSpawn(club);
            }

            else if (axe.CompareTag(playerWeaponID))
            {
                boardScript.WeaponSpawn(axe);
            }

            else if (goldenSword.CompareTag(playerWeaponID))
            {
                boardScript.WeaponSpawn(goldenSword);
            }
        }


        boardScript.SetupScene(level);

    }


    private void HideLevelImage()
    {

        levelImage.SetActive(false);

        doingSetup = false;

        TutorialTipActivator();

    }

    // Shows the tutorial tips UI screen.
    public void TutorialTipActivator()
    {

        if (MainMenu.tutorialTips)
        {

            toolTipPause = true;

            Time.timeScale = 0f;
            toolTip.SetActive(true);
        }


    }

    // Pauses game completely for player.
    public void PauseActivator()

    {
        if (Player.gamePaused == true)
        {

            pauseImage.SetActive(true);

            Time.timeScale = 0f;

        }

    }
    
    // Resumes game for player.
    public void PauseDeactivator()
    {
        if (Player.gamePaused == false)
        {
            pauseImage.SetActive(false);

            Time.timeScale = 1f;

        }
    }
    public void InfoBoxActivator()

    {
        infoImage.SetActive(true);
        infoImageVisible = true;
    }



    // Begins process of showing player game loss screen and removes any movement that can occur.
    public void GameoOverScreen()
    {

        StartCoroutine(IntermediateText());

        noMovement = true;

    }

    // Delayed showing of game over screen and displays a random quote from quote list.
    IEnumerator IntermediateText()
    {

        doingSetup = true;

        levelText.text = "After " + level + " floors, you were defeated.";
        levelImage.SetActive(true);

        yield return new WaitForSeconds(2);

        int quoterandomiser = Random.Range(1, 6);

        if (quoterandomiser <= 4)
        {

            int randomIndex = Random.Range(0, goodDefeatQuotes.Count);

            goodText.text = goodDefeatQuotes[randomIndex];

            goodQuotes.SetActive(true);

        }

        else
        {
            int randomIndex = Random.Range(0, badDefeatQuotes.Count);

            badText.text = badDefeatQuotes[randomIndex];

            badQuotes.SetActive(true);

        }

        Invoke(nameof(GameOver), gameOverScreenDelay);

    }

    // Turns background music back on, resets level to 1 and destroys this instance of GameManager.
    public void GameOver()
    {
        SoundManager.instance.musicSource.Play();

        level = 1;

        SceneManager.LoadScene("Level 1");

        Player.gamePaused = false;

        Destroy(this);

    }

    // Begins process of showing user game won screen.
    public void GameWon()
    {

        StartCoroutine(GameWonText());


    }

    // Delayed showing of game won screen with fixed quote. Calls function to end and reset game.
    IEnumerator GameWonText()
    {
        levelText.text = "";
        gameWinText.text = "\"The Demonlord Halzor has been slain... You have succeeded where many have failed.\"\n\n" + "A rumbling grows louder as the path ahead clears onto new adventures.";
        levelImage.SetActive(true);

        yield return new WaitForSeconds(5);

        Invoke(nameof(GameOver), gameOverScreenDelay);

    }

    // Checks if player wants to pause game, checks who's turn it is to move, begins enemies movement if its their turn and also removes any info tool tip on screen with player input. Adapted from code written by Matt Schell (2022).
    void Update() 
    {
        if (toolTipPause)
        {
            if (Input.anyKey)
            {

                Time.timeScale = 1f;
                toolTip.SetActive(false);

                toolTipPause = false;
            }

        }
        cameraUpdater.CameraUpdate();
        playerScript.InputUpdate();
        //healthbar.HealthUpdate();


        if (playersTurn || enemiesMoving || doingSetup)
        {
            return;

        }

        StartCoroutine(MoveEnemies());


        if (Input.anyKey && infoImageVisible)
        {

            infoImage.SetActive(false);
        }

    }


    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);


    }

    public void DeleteEnemyFromList(Enemy enemy)
    {
        enemies.Remove(enemy);

    }



    // Coroutine which moves the enemy objects in the correct sequence that are in the list of enemies. Adapted from code written by Matt Schell (2022).
    IEnumerator MoveEnemies()

    {

        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0 || !this.gameObject.activeSelf)
        {
            yield return new WaitForSeconds(turnDelay);

        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();

            yield return new WaitForSeconds(turnDelay);


        }

        playersTurn = true;

        enemiesMoving = false;

    }



}

// Code from Matt Schell (2022) can be accessed from: https://learn.unity.com/project/2d-roguelike-tutorial?uv=5.x