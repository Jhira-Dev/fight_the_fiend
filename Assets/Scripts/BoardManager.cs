using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.SceneManagement;

// Adapted from code written by Matt Schell (2022), link to original code at bottom

public class BoardManager : MonoBehaviour
{

    [System.Serializable]

    // Minimum and Maximum variables which can be adjuseted in the inspector
    public class Count
    {
        public int minimum;
        public int maximum;



        public Count(int min, int max)
        {

            minimum = min;
            maximum = max;
        }

    }

    public int columns;
    public int rows;

    public List<Transform> excludedObjects = new List<Transform>();

    public Count potionCount = new Count(0, 2);
    public Count coinCount = new Count(0, 2);
    public Count weaponCount = new Count(0, 2);
    public Count skullCount = new Count(0, 0);
    public Count keyCount = new Count(0, 2);
    public GameObject[] potionTiles;
    public GameObject[] enemyTiles;
    public GameObject[] coinTiles;
    public GameObject[] weaponTiles;
    public GameObject[] skullTiles;
    public GameObject[] keyTiles;
    public Transform playerposition;
    public GameObject demonBoss;
    public GameObject bossSpawner;


    private List<Vector3> gridPositions = new List<Vector3>();


    // Clears any previous floor's grid list and then creates a new viable grid list after removing unusable grid positions.
    void InitialiseList()
    {

        gridPositions.Clear();

        for (float x = 0.5f; x < columns; x++)

        {
            for (float y = 0.5f; y < rows; y++)
            {
                // Set up list of grid positions of whole rectangluar shape.
                gridPositions.Add(new Vector3(x, y, 0f));
            }

        }

        List<Vector3> excludedTiles = new List<Vector3>();

        foreach (Transform transform in excludedObjects)
        {
            // Assigning elements of all unvible grid positions to separate list.
            excludedTiles.Add(new Vector3(transform.position.x, transform.position.y, 0f));



        }
        // Adjust the list by removing any of unviable grid positions.
        gridPositions = gridPositions.Except(excludedTiles).ToList();
    }

    // Generates a random position from viable list and removes it so that it cannot be re-used.
    Vector3 RandomPosition()

    {

        int randomIndex = Random.Range(0, gridPositions.Count);

        Vector3 randomPosition = gridPositions[randomIndex];

        gridPositions.RemoveAt(randomIndex);

        return randomPosition;

    }

    // Function to spawn items and enemies onto a grid position. Takes an array of GameObjects to select one from as well as the min and max range.
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();

            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            Instantiate(tileChoice, randomPosition, Quaternion.identity);

        }


    }

    // Separate function to instantiate weapons on the floor grid.
    public void WeaponSpawn(GameObject weaponToSpawn)

    {

     Instantiate(weaponToSpawn, playerposition.position, Quaternion.identity);

    }

    // Responsible for calling above function with each different type of GameObject to spawn. Also is responsible for spawning the final boss only on floor 5.
    public void SetupScene(int level)

    {
        InitialiseList();

        LayoutObjectAtRandom(potionTiles, potionCount.minimum, potionCount.maximum);

        LayoutObjectAtRandom(coinTiles, coinCount.minimum, coinCount.maximum);

        LayoutObjectAtRandom(weaponTiles, weaponCount.minimum, weaponCount.maximum);

        LayoutObjectAtRandom(skullTiles, skullCount.minimum, skullCount.maximum);

        LayoutObjectAtRandom(keyTiles, keyCount.minimum, keyCount.maximum);


        int enemyCount = level + 1;

        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level 5"))

        {
            Instantiate(demonBoss, bossSpawner.transform.position, Quaternion.identity);

        }

    }
}




//  Code from Matt Schell (2022) can be accessed from: https://learn.unity.com/project/2d-roguelike-tutorial?uv=5.x