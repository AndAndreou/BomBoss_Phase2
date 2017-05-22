﻿using EazyTools.SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipController : MonoBehaviour {

    GameManagerBomb gameManager;
   public bool DeathDisplay = false;
    //for firing
    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    public int overheatRate;
    public float overheatTotal;
    public int maxOverheat = 100;
    public float cooldownRate;
    public float cooldownAmount;
    public float respawnTime = 5.0f;

    private bool isOverheated = false;
    private float nextFire;

    public Transform shipSpawn; // Spawn position
    public Transform diePoint;

    public HoverControl hoverControl; // To fetch myPlayer

    public GameObject deathScreen;

    [Header("Audio")]
    public AudioClip piouSFX;

    private float volume;

    private string[] deathSlogans = new string[] { "Dead", "You suck", "Boom!", "Destroyed", "What a shame", "Too slow", "Disgrace", "Try again", "Too bad" };

    // Use this for initialization
    void Start () {

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerBomb>();
        volume = PlayerPrefs.GetFloat("Volume", 1f);
        //0.1seconds after the game starts the function " cannonColldown " will be called.
        //The function will be called every X seconds (as set by the "cooldownRate" variable in the player inspector)
        InvokeRepeating("cannonCooldown", 0.1f, cooldownRate);

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Shoot" + hoverControl.myPlayer.ToString()) && (Time.time > nextFire) && (isOverheated == false) && (gameManager.canUsePowerUPS) && (!DeathDisplay))
        {
            nextFire = Time.time + fireRate; //Set the next available time for firing a missile

            GameObject go = Instantiate(shot, shotSpawn.position, shotSpawn.rotation); //spawn a missile
            go.GetComponent<shot>().parentTransform = this.transform;


            overheatTotal += overheatRate; //Everytime the cannon fires, it's heat rises

            //play SFX
            SoundManager.PlaySound(piouSFX,volume);

            //check if the cannon has reached maximum overheat and needs to cool down
            if (overheatTotal >= maxOverheat)
            {
                overheatTotal = maxOverheat;
                isOverheated = true;
            }
        }

    }

    public void Die()
    {

        // print("this works");

        /*       
        gameObject.SetActive(false);
        // Move it to the spawn point
        this.transform.position = shipSpawn.position;
        this.transform.rotation = shipSpawn.rotation; // Fix it's rotation
        //yield return new WaitForSeconds(seconds);

        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        gameObject.SetActive(true);

        //re-initialize current health to max health when respawned
        ShipStatus shipStatus = GetComponent<ShipStatus>();
        shipStatus.currHealth = shipStatus.maxHealth;

        this.GetComponent<PlayerStats>().incrementDeaths();
        */
        DeathDisplay = true;
        deathScreen.SetActive(true);
        deathScreen.GetComponentInChildren<Text>().text = deathSlogans[Mathf.FloorToInt(Random.Range(0, deathSlogans.Length))];

        // Move it to the die point
        this.transform.position = diePoint.position;
        this.transform.rotation = diePoint.rotation; // Fix it's rotation

        StartCoroutine(Respawn(respawnTime));
    }

    
    IEnumerator Respawn(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Move it to the spawn point
        this.transform.position = shipSpawn.position;
        this.transform.rotation = shipSpawn.rotation; // Fix it's rotation
        //yield return new WaitForSeconds(seconds);

        deathScreen.SetActive(false);

        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        //gameObject.SetActive(true);

        //re-initialize current health to max health when respawned
        ShipStatus shipStatus = GetComponent<ShipStatus>();
        shipStatus.currHealth = shipStatus.maxHealth;
        DeathDisplay = false;
        this.GetComponent<PlayerStats>().incrementDeaths();
    }

    //used for cannon mechanics
    void cannonCooldown()
    {
        overheatTotal -= cooldownAmount;//reduce the overheat every second
        //When overheat reaches 0 (or below 0) then set the counter to 0 and change the flag. Player can now fire again
        if (overheatTotal <= 0)
        {
            overheatTotal = 0;
            isOverheated = false; // not the player can shoot with the cannon again
        }
    }

}//END OF MONOBEHAVOUR
