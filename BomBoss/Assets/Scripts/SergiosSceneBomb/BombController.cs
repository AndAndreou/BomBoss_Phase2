using EazyTools.SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombController : MonoBehaviour {

    // UI
    public Text bombInfoText;
    // Times
    public int minBombExplodeTime = 10;
    public int maxBombExplodeTime = 20;
    public int currentExplodeTime; // To see when to change color of bomb
    public float currentExplodeTimeRed; // Near explode time
    public float currentExplodeTimeYellow;
    public float timeElapsedArmed;
    public float timeElapsedExploding;
    public float timeElapsedExploded;
    public float maxTimeElapsedExploding = 2;
    public float maxTimeElapsedExploded = 3;
    public BombState currentState = BombState.paused; // Can be ARMED, EXPLODING, EXPLODED, PAUSED
    public BombState beforePauseState = BombState.isNull;
    public Vector3 beforePauseVelocity;
    public Vector3 beforePauseAngularVelocity;
    private bool unpausing;

    public Transform spawnPoint; // Spawn point
    private GameManagerBomb gameManager;

    public GameObject explosionEffect;

    List<GameObject> triggerList = new List<GameObject>();

    public GameObject lastTouchedRed;
    public GameObject lastTouchedBlue;

    public Material greenMaterial;
    public Material yellowMaterial;
    public Material redMaterial;

    [Header("Audio")]
    public AudioClip explotionSFX;
    public AudioClip preDatonationSFX;

    [HideInInspector]
    public BombColor bombColor = BombColor.Green;

    private float volume;

    // Use this for initialization
    void Awake () {

        volume = PlayerPrefs.GetFloat("Volume", 1f);

        gameManager = GameObject.FindWithTag(GameRepository.gameManagerTag).GetComponent<GameManagerBomb>();
        //currentState.myState = BombState.statePaused; //Moved this to variable declaration
        maxTimeElapsedExploding = explosionEffect.GetComponent<ParticleSystem>().main.duration;
        UpdateUI();
	}

    void ResetTimers ()
    {
        timeElapsedArmed = 0;
        timeElapsedExploding = 0;
        timeElapsedExploded = 0;
    }

    void IncrementTimers (float deltaTime)
    {
        if (currentState == BombState.armed)
        {
            timeElapsedArmed += deltaTime;
        }
        else if (currentState == BombState.exploding)
        {
            timeElapsedExploding += deltaTime;
        }
        else if (currentState == BombState.exploded)
        {
            timeElapsedExploded += deltaTime;
        }
    }

    void SetState (BombState newState)
    {
        // State Transition Validation Logic
        if (currentState == BombState.armed && newState != BombState.exploding && newState != BombState.paused)
        {
            MyLog("Invalid bomb state transition");
        }

        if (currentState == BombState.exploding && newState != BombState.exploded)
        {
            MyLog("Invalid bomb state transition");
        }

        if (currentState == BombState.exploded && newState != BombState.armed)
        {
            MyLog("Invalid bomb state transition");
        }

        if (currentState == BombState.paused && newState != BombState.unPausing && newState != BombState.armed)
        {
            MyLog("Invalid bomb state transition");
        }

        // Special validations
        if (newState == BombState.armed && gameManager.currentState == RoundState.finished)
        {
            // If round ended don't allow re-arming of bomb
            return;
        }

        // Store the previousState
        BombState previousState = currentState;
        // Update the currentState
        currentState = newState;

        gameManager.SetBombState(currentState);

        // Actions to do AFTER updating the currentState
        if (newState == BombState.armed)
        {
            MyLog(newState.ToString());
            if (! unpausing)
            {
                // Only reset timers when not called after unpause
                ResetTimers();
                currentExplodeTime = Random.Range(minBombExplodeTime, maxBombExplodeTime);
                currentExplodeTimeYellow = ((float)currentExplodeTime) / 3; // One third
                currentExplodeTimeRed = currentExplodeTimeYellow * 2; // Two thirds

                // Nobody has touched the bomb yet
                lastTouchedBlue = null;
                lastTouchedRed = null;

                if (previousState == BombState.exploded)
                {
                    ResetToSpawnPoint();
                }

                gameObject.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                unpausing = false;
            }
        }
        else if (newState == BombState.exploding)
        {
            bombColor = BombColor.Black;
            MyLog(newState.ToString());
            // Detach bomb from car
            //this.GetComponent<BombHover>().Respawn(spawnPoint.position);
            gameObject.GetComponent<Renderer>().enabled = false;
            explosionEffect.GetComponent<ExplosionController>().PlayEffect();

            //play sfx
            SoundManager.StopAllSounds();
            SoundManager.PlaySound(explotionSFX, volume);

            MyLog("Exploding " + triggerList.Count + " objects");

            foreach (GameObject go in triggerList)
            {
                if (go == null) { continue; } // Skip object if object has been destroyed
                ExplodeController expControl = null;
                expControl = go.GetComponent<ExplodeController>();

                if (expControl != null)
                {
                    if (go.transform.parent.tag == GameRepository.doorTag)
                    {
                        // Add score to last touched player
                        DoorController doorController = go.GetComponent<DoorController>();
                        if (doorController != null)
                        {
                            GameObject scorer = null;
                            if (doorController.team == Team.blue && lastTouchedRed != null)
                            {
                                scorer = lastTouchedRed;
                            }
                            else if (doorController.team == Team.red && lastTouchedBlue != null)
                            {
                                scorer = lastTouchedBlue;
                            }

                            if( scorer != null )
                            {
                                PlayerStats scorerStats = scorer.GetComponent<PlayerStats>();
                                if (scorerStats != null)
                                {
                                    scorerStats.incrementScore();
                                }
                            }
                        }
                    }
                    expControl.Explode();
                }
            }

            // Clear list of collided objects after explosion
            triggerList.Clear();

            //explosionEffect.GetComponent<ParticleSystem>().Play();
            //Debug.Log("Looping: " + explosionEffect.GetComponent<ParticleSystem>().main.loop);
            StopMovement();
            gameManager.BombExploded();
        }
        else if (newState == BombState.exploded)
        {
            MyLog(newState.ToString());
            explosionEffect.GetComponent<ExplosionController>().StopEffect();
            //explosionEffect.GetComponent<ParticleSystem>().Stop(); // Not needed because I disabled looping in all particle systems
        }
        else if (newState == BombState.paused)
        {
            MyLog(newState.ToString());
            if (beforePauseState == BombState.exploding)
            {
                explosionEffect.GetComponent<ExplosionController>().PauseEffect();
                //explosionEffect.GetComponent<ParticleSystem>().Pause();// playbackSpeed = 0f;
            }
        }
        else if (newState == BombState.unPausing)
        {
            MyLog(newState.ToString());
            if (beforePauseState == BombState.exploding)
            {
                explosionEffect.GetComponent<ExplosionController>().PlayEffect();
                //explosionEffect.GetComponent<ParticleSystem>().Play(); // playbackSpeed = 1f;
            }
            unpausing = true;
            SetState(beforePauseState);
        }
        else
        {
            MyLog("Unknown bomb state");
        }
    }

    public void Arm()
    {
        BombState newState = BombState.armed;
        SetState(newState);
    }

    public void Pause()
    {
        // Store the current values for pause state, velocity and angularVelocity
        beforePauseState = currentState;
        beforePauseVelocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x, this.GetComponent<Rigidbody>().velocity.y, this.GetComponent<Rigidbody>().velocity.z);
        beforePauseAngularVelocity = new Vector3(this.GetComponent<Rigidbody>().angularVelocity.x, this.GetComponent<Rigidbody>().angularVelocity.y, this.GetComponent<Rigidbody>().angularVelocity.z);
        // Stop all forces on bomb
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        SetState(BombState.paused);
    }

    public void UnPause()
    {
        if (beforePauseState != BombState.isNull)
        {
            this.GetComponent<Rigidbody>().velocity = beforePauseVelocity;
            this.GetComponent<Rigidbody>().angularVelocity = beforePauseAngularVelocity;

            SetState(BombState.unPausing);
        }
    }

    public void Explode()
    {
        SetState(BombState.exploding);
    }

    public void VoidCollided()
    {
        ResetToSpawnPoint();
    }

    void ResetToSpawnPoint()
    {
        // Hide bomb and disable collider
        this.GetComponent<Renderer>().enabled = false;
        this.GetComponent<SphereCollider>().enabled = false;
        // Move it to the spawn point
        this.transform.position = spawnPoint.position;
        StopMovement();
        // Show bomb and enable collider
        this.GetComponent<Renderer>().enabled = true;
        this.GetComponent<SphereCollider>().enabled = true;
    }

    void StopMovement()
    {
        // Stop all forces on bomb
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
	
    void HandleBombState()
    {
        if (currentState == BombState.armed)
        {
            if (timeElapsedArmed > currentExplodeTime)
            {
                SetState(BombState.exploding);
            }
            else
            {
                // Handle the updating of the bomb color
                if (timeElapsedArmed < currentExplodeTimeYellow)
                {
                    //this.GetComponent<Renderer>().material.color = Color.green;
                    this.GetComponent<Renderer>().material = greenMaterial;
                    bombColor = BombColor.Green;
                }
                else if (timeElapsedArmed < currentExplodeTimeRed && timeElapsedArmed >= currentExplodeTimeYellow)
                {
                    //this.GetComponent<Renderer>().material.color = Color.yellow;
                    this.GetComponent<Renderer>().material = yellowMaterial;
                    bombColor = BombColor.Yellow;
                }
                else 
                {
                    //this.GetComponent<Renderer>().material.color = Color.red;
                    
                    if(bombColor != BombColor.Red)
                    {
                        this.GetComponent<Renderer>().material = redMaterial;
                        bombColor = BombColor.Red;
                        SoundManager.PlayMusic(preDatonationSFX, volume, true, true, 2f, 1f);
                        //SoundManager.PlaySound(preDatonationSFX, volume);
                    }
                }
            }
        }
        else if (currentState == BombState.exploding)
        {
            if (timeElapsedExploding > maxTimeElapsedExploding)
            {
                SetState(BombState.exploded);
            }
        }
        else if (currentState == BombState.exploded)
        {
            if (timeElapsedExploded > maxTimeElapsedExploded)
            {
                SetState(BombState.armed);
            }
        }
    }

    // Update is called once per frame
    void Update () {
        IncrementTimers(Time.deltaTime);
        HandleBombState();
        UpdateUI();
	}

    //void FixedUpdate()
    //{

    //    if (currentState == BombState.armed) // bomb can move only when armed
    //    {

    //        // movement of bomb (only necessary for testing)
    //        float movehorizontal = Input.GetAxis("Horizontal");
    //        float movevertical = Input.GetAxis("Vertical");

    //        Vector3 movement = new Vector3(movehorizontal, 0.0f, movevertical);
    //        GetComponent<Rigidbody>().AddForce(movement * 10);
    //    }
    //}

    //called when something enters the trigger
    void OnTriggerEnter(Collider other)
    {
        Attach(other);
    }

    //called when something exits the trigger
    void OnTriggerExit(Collider other)
    {
        Detach(other);
    }

    public void Attach(Collider other)
    {
        //if the object is not already in the list and bomb is armed
        if (!triggerList.Contains(other.gameObject) && currentState == BombState.armed)
        {
            //add the object to the list
            triggerList.Add(other.gameObject);
            MyLog("BombAttach:" + other.gameObject.name);
        }
    }

    public void Detach(Collider other)
    {
        //if the object is in the list
        if (triggerList.Contains(other.gameObject))
        {
            //remove it from the list
            triggerList.Remove(other.gameObject);
            MyLog("BombDetach:" + other.gameObject.name);
        }
    }

    void UpdateUI ()
    {
        bombInfoText.text = string.Format("Bomb Status: {0} TimerArmed: {1:00.00} TimerExploding: {2:00.00} TimerExploded: {3:00.00}", currentState.ToString(), timeElapsedArmed, timeElapsedExploding, timeElapsedExploded);
    }

    void MyLog(string msg)
    {
        Debug.Log(string.Format("Bomb-{0}", msg));
    }

    public void SetLastTouched(GameObject go)
    {
        if (go != null)
        {
            HoverControl hoverControl = go.GetComponent<HoverControl>();
            if (hoverControl != null)
            {
                if (hoverControl.myTeam == Team.blue)
                {
                    lastTouchedBlue = go;
                }
                else if (hoverControl.myTeam == Team.red)
                {
                    lastTouchedRed = go;
                }
            }
        }
    }
}

public enum BombColor
{
    Red = 0,
    Green= 1,
    Yellow = 2,
    Black = 3
}
