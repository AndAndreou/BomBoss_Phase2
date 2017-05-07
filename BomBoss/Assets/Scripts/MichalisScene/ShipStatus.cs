using EazyTools.SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipStatus : MonoBehaviour {

    ShipController shipController;
    MovementEngine movementEngine;
    Rigidbody rigidbody;

    //for Healing
    public float maxHealth = 100;
    public float currHealth;

    //for jumping
    Vector3 jumpVector = Vector3.up;
    public float jumpForce;
    public float jumpsLeft;
    public bool hasJump = true;

    //for shield
    public bool hasShield = false;
    public bool shieldActivated = false;
    public float shieldDuration = 5f;
    public float maxShieldHealth = 10;
    public float currShieldHealth;
    float shieldTimePassed = 0;

    //for boost
    public bool hasBoost = true;
    public float boostDuration;
    public float maxTurboSpeed;
    public float maxTurboAcceleration;

    public HoverControl hoverControl; // To fetch myPlayer

    public GameObject bbCarGO;
    public Material normalMaterial;
    public Material emissionMaterial;

    [Header("Audio")]
    public AudioClip shieldSFX;

    [System.Serializable]
    public struct HudComponents
    {
        public Text healthTxt;
        public Image boomIcon;
        public Image gunOverheatFill;
        public GameObject boostIcon;
        public GameObject shieldIcon;
        public GameObject jumpIcon;
    }

    [Header("UIComponent")]
    public HudComponents hudComponents;

    [Header("Sprites")]
    public Sprite bombRedSprite;
    public Sprite bombGreenSprite;
    public Sprite bombYellowSprite;

    private BombController bombController;

    private float volume;

    Renderer bbCarRendere;

    // Use this for initialization
    void Start () {

        bbCarRendere = bbCarGO.GetComponent<Renderer>();
        bbCarRendere.material = normalMaterial;

        volume = PlayerPrefs.GetFloat("Volume", 1f);

        rigidbody = GetComponent<Rigidbody>();
        currHealth = maxHealth;
        bombController = GameObject.FindGameObjectWithTag(GameRepository.bombTag).GetComponent<BombController>();
        shipController = gameObject.GetComponent<ShipController>();
        currShieldHealth = maxShieldHealth;
        //jumpVector = new Vector3(0.0f, 1.0f, 0.0f);
    }
	
	// Update is called once per frame
	void Update () {
        SetHud();

        if ((currShieldHealth <= 0))
        {
            currShieldHealth = 0;
            shieldActivated = false;
            CancelInvoke();
        }

        //Check input from user to activate the power-ups
        if (Input.GetButtonDown("Boost" + hoverControl.myPlayer.ToString()) && hasBoost)
        {
            MyLog("B boost key was pressed, activated boost");

            StartCoroutine(boostMe());
        }

        if (Input.GetButtonDown("Jump" + hoverControl.myPlayer.ToString()) && hasJump && jumpsLeft > 0)
        {
            MyLog("N jump key was pressed, activated jump");

            rigidbody.AddForceAtPosition(jumpVector * jumpForce, bbCarGO.transform.position, ForceMode.Force);
            jumpsLeft--;
            if (jumpsLeft == 0)
            {
                hasJump = false;
            }

        }

        if (Input.GetButtonDown("Shield" + hoverControl.myPlayer.ToString()) && hasShield)
        {
            MyLog("M shield key was pressed, activated shield"); //works

            shieldMe();
        }
    }

    void FixedUpdate()
    {
        // FixedUpdate isn't called every frame, and sometimes multiple times a frame, which means it'll screw up the GetButtonDown return
    }//END OF FIXED UPDATE

    //Used to substract health from the player and check if his HP reaches zero
    public void applyDamage(float damage, GameObject shooter)
    {
        currHealth -= damage;
        if (currHealth <= 0)
        {
            currHealth = 0;
            shipController.Die();

            if (shooter!=null)
            {
                PlayerStats playerStats = shooter.GetComponent<PlayerStats>();

                if (playerStats != null)
                {
                    playerStats.incrementKills();
                }
            }
        }
    }

    //Boost mechanics
    IEnumerator boostMe()
    {
        float timePassed = 0; //Used for time counting

        while (timePassed <= boostDuration) //check if the boost duration is over
        {
            gameObject.GetComponent<MovementEngine>().MaxForwardAcceleration = maxTurboAcceleration;
            gameObject.GetComponent<MovementEngine>().MaxSpeed = maxTurboSpeed;
            
            timePassed += Time.deltaTime; // increment our timer
            yield return null;
        }

        if (timePassed >= boostDuration) //used to return the value back to its original value over time
        {
            gameObject.GetComponent<MovementEngine>().MaxForwardAcceleration = 20;
            gameObject.GetComponent<MovementEngine>().MaxSpeed = 25;
            hasBoost = false;

            yield return null;
        }

    }//End of boostME IEnumerator

    //Shield mechanics
    public void shieldMe()
    {
        SoundManager.PlaySound(shieldSFX, volume);
        float shieldActivationMoment = Time.time; //Used for time counting   
        shieldActivated = true;
        hasShield = false;
        currShieldHealth = maxShieldHealth;
        Debug.Log("Msg before calling check shield function: " + shieldActivationMoment);
        shieldTimePassed = 0;
        checkshield();
        InvokeRepeating("checkshield", 0.01f, 1);

    }//End of shieldME

    public void checkshield()
    {
        shieldTimePassed++;
        //Debug.Log("Moment: " + shieldActivationMoment);
        Debug.Log("Time passed: " + shieldTimePassed);

        //Player has his shield activated still
        if (shieldTimePassed < shieldDuration)
        {
            Debug.Log("Time passed is less than shield duration: " + shieldTimePassed);

            if (bbCarRendere.material != emissionMaterial)
            {
                bbCarRendere.material = emissionMaterial;
            }
            
            return;
        }


        if ((shieldTimePassed >= shieldDuration) || (currShieldHealth <= 0))
        {
            Debug.Log("Time passed is greater than shield duration: " + shieldTimePassed);
            SoundManager.GetSoundAudio(shieldSFX).Stop();
            currShieldHealth = 0;
            shieldActivated = false;
            CancelInvoke();

            if (bbCarRendere.material != normalMaterial)
            {
                bbCarRendere.material = normalMaterial;
            }

            return;
        }

    }

    //get a power up
    public void GetPowerUp(PowerUpType powerUp, float value)
    {
        switch (powerUp)
        {
            case PowerUpType.health:
                currHealth = maxHealth;
                Debug.Log("Health: " + currHealth);
                break;
            case PowerUpType.shield:
                hasShield = true;
                Debug.Log("Shield: " + currShieldHealth); //Works
                break;
            case PowerUpType.jump:
                jumpsLeft = 2;
                hasJump = true;
                Debug.Log("Jump: " + jumpsLeft);
                break;
            case PowerUpType.boost:
                //this.hasBoost = true;
                hasBoost = true;
               // boostDuration += value;
                Debug.Log("Boost: " + boostDuration);
                break;
        }
    }

    private void SetHud()
    {
        hudComponents.jumpIcon.SetActive(hasJump);
        hudComponents.shieldIcon.SetActive(hasShield);
        hudComponents.boostIcon.SetActive(hasBoost);

        hudComponents.healthTxt.text = Mathf.FloorToInt(currHealth).ToString();


        //todo if player attaches bomb set active false hudComponents.boomIcon
        BombColor bombState = bombController.bombColor;
        switch (bombState)
        {
            case BombColor.Black:
                hudComponents.boomIcon.gameObject.SetActive(false);
                break;
            case BombColor.Yellow:
                hudComponents.boomIcon.gameObject.SetActive(true);
                hudComponents.boomIcon.sprite = bombYellowSprite;
                break;
            case BombColor.Red:
                hudComponents.boomIcon.gameObject.SetActive(true);
                hudComponents.boomIcon.sprite = bombRedSprite;
                break;
            case BombColor.Green:
                hudComponents.boomIcon.gameObject.SetActive(true);
                hudComponents.boomIcon.sprite = bombGreenSprite;
                break;
        }

        float fillAmountGunOverhead = ((shipController.overheatTotal - 0f) * ((1f - 0f) / (shipController.maxOverheat - 0f))) + 0f;
        hudComponents.gunOverheatFill.fillAmount = fillAmountGunOverhead;
    }

    private void MyLog(string msg)
    {
        Debug.Log(string.Format("{0}-{1}", hoverControl.myPlayer.ToString(), msg));
    }
}
