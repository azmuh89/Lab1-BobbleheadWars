using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Player; // represents the bobblehead warrior
    public GameObject[] spawnPoints; // locations where aliens will spawn in the arena
    public GameObject alien; // represents the prefab for the alien
    public int maxAliensOnScreen; // will determine how many aliens appear on screen at once
    public int totalAliens; // will represent the total number of aliens player has to kill to win
    public float minSpawnTime; // the rate at which aliens appear
    public float maxSpawnTime;
    public int aliensPerSpawn; // how many aliens appead during spawning

    public GameObject upgradePrefab;
    public Gun gun;
    public float upgradeMaxTimeSpawn = 7.5f;

    public GameObject deathFloor;

    public Animator arenaAnimator;

    private int aliensOnScreen = 0; // will track total number of aliens currently displayed
    private float generatedSpawnTime = 0; // will track time between spawns
    private float currentSpawnTime = 0; // will track milliseconds since last spawn

    private bool spawnedUpgrade = false;
    private float actualUpgradeTime = 0;
    private float currentUpgradeTime = 0;

    private void endGame()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.
        elevatorArrived);
        arenaAnimator.SetTrigger("PlayerWon");

        if (totalAliens == 0)
        {
            Invoke("endGame", 2.0f);
        }
    }

    void Start()
    {
        actualUpgradeTime = Random.Range(upgradeMaxTimeSpawn - 3.0f, upgradeMaxTimeSpawn);
        actualUpgradeTime = Mathf.Abs(actualUpgradeTime);
    }

    void Update()
    {
        if (Player == null)
        {
            return;
        }

        currentUpgradeTime += Time.deltaTime;

        if (currentUpgradeTime > actualUpgradeTime)
        {
            // checks if upgrade has already spawned
            if (!spawnedUpgrade)
            {
                // increases the risk-and-reward dynamic for player
                int randomNumber = Random.Range(0, spawnPoints.Length - 1);
                GameObject spawnLocation = spawnPoints[randomNumber];
                // handles spawning the upgrade and associating gun with it
                GameObject upgrade = Instantiate(upgradePrefab) as GameObject;
                Upgrade upgradeScript = upgrade.GetComponent<Upgrade>();
                upgradeScript.gun = gun;
                upgrade.transform.position = spawnLocation.transform.position;
                // informs the code the upgrade has spawned
                spawnedUpgrade = true;
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.powerUpAppear);
            }
        }

        currentSpawnTime += Time.deltaTime;

        if (currentSpawnTime > generatedSpawnTime) // spawn time randomizer
        {
            currentSpawnTime = 0; // resets timer after spawn occurs
            generatedSpawnTime = Random.Range(minSpawnTime, maxSpawnTime); // spawn time randomizer

            if (aliensPerSpawn > 0 && aliensOnScreen < totalAliens) // determines whether to spawn
            {
                List<int> previousSpawnLocations = new List<int>(); // array to keep track of where you spawn aliens each wave

                if (aliensPerSpawn > spawnPoints.Length)
                {
                    aliensPerSpawn = spawnPoints.Length - 1; // limits number of aliens to spawn by number of spawn points
                }

                aliensPerSpawn = (aliensPerSpawn > totalAliens) ? aliensPerSpawn - totalAliens : aliensPerSpawn;

                for (int i = 0; i < aliensPerSpawn; i++)
                {
                    if (aliensOnScreen < maxAliensOnScreen)
                    {
                        aliensOnScreen += 1;

                        //generated spawn point number
                        int spawnPoint = -1;
                        //runs unitl it finds a spawn point or spawn point is no longer -1
                        while (spawnPoint == -1)
                        {
                            //produces a random number as a possible spawn point
                            int randomNumber = Random.Range(0, spawnPoints.Length - 1);
                            //checks array if random number is active spawn point
                            if (!previousSpawnLocations.Contains(randomNumber))
                            {
                                previousSpawnLocations.Add(randomNumber);
                                spawnPoint = randomNumber;
                            }
                        }

                        GameObject spawnLocation = spawnPoints[spawnPoint]; // grabs the spawn point based on index generated in last code
                        GameObject newAlien = Instantiate(alien) as GameObject;
                        newAlien.transform.position = spawnLocation.transform.position;
                        Alien alienScript = newAlien.GetComponent<Alien>();
                        alienScript.target = Player.transform;
                        Vector3 targetRotation = new Vector3(Player.transform.position.x, newAlien.transform.position.y, Player.transform.position.z);
                        newAlien.transform.LookAt(targetRotation);
                        alienScript.OnDestroy.AddListener(AlienDestroyed);
                        alienScript.GetDeathParticles().SetDeathFloor(deathFloor);
                    }
                }
            }
        }
    }

    public void AlienDestroyed()
    {
        aliensOnScreen -= 1;
        totalAliens -= 1;
    }
}
