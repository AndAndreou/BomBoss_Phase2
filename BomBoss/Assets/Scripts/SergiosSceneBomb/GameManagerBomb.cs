using EazyTools.SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerBomb : MonoBehaviour {

    [Header("Players GameObjects")]
    public GameObject[] playersGO = new GameObject[4];

    public GameObject bomb;
    private Vector3 offset;

    // UI
    public Text roundInfoText;
    public Text goalInfoText;
    // Times
    public int maxRoundTime = 120; // secs
    public float timeElapsed;

    public RoundState currentState = RoundState.finished; // Can be STARTING, RUNNING, PAUSED, FINISHED

    private BombState bombState;

    [HideInInspector]
    public float gameVolume = 1f;

    [System.Serializable]
    public struct GoalStruct
    {
        public GameObject leftDoor;
        public GameObject centerDoor;
        public GameObject rightDoor;
    }

    public GoalStruct blueDoors;
    public GoalStruct redDoors;

    [System.Serializable]
    public struct HudComponents
    {
        public Text timeTxt;
        public GameObject redDoorLIcon;
        public GameObject redDoorCIcon;
        public GameObject redDoorRIcon;
        public GameObject blueDoorLIcon;
        public GameObject blueDoorCIcon;
        public GameObject blueDoorRIcon;
    }

    [System.Serializable]
    public struct ScoreBoardComponents
    {
        public Text playerName;
        public Text kills;
        public Text deaths;
        public Text score;
    }

    [Header("UIComponent")]
    public HudComponents[] hudComponents = new HudComponents[4];
    public ScoreBoardComponents[] blueTeamScoreBoard = new ScoreBoardComponents[2];
    public ScoreBoardComponents[] redTeamScoreBoard = new ScoreBoardComponents[2];
    public GameObject scoreBoardPanel;
    public GameObject textRedWinner;
    public GameObject textBlueWinner;

    [Header("Audio")]
    public AudioClip backgroundMusic;
    public AudioClip scoreBoardMusic;

    private float volume;

    private bool gameOverFlag;

    private void Awake()
    {
        SetControllersInHoverCarsGO();
    }


    // Use this for initialization
    void Start () {
        volume = PlayerPrefs.GetFloat("Volume", 1f);

        offset = transform.position - bomb.transform.position;
        //currentState = RoundStateFinished;
        SetState(RoundState.starting);

        gameOverFlag = false;

        SoundManager.PlayMusic(backgroundMusic,volume, true, true, 2f, 1f);
    }
	
	// Update is called once per frame
	void Update () {

        if(gameOverFlag == true)
        {
            if (Input.GetButton("ShootPlayer1".ToString()))
            {
                GoToMainMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePauseRound();
        }

        IncrementTimers(Time.deltaTime);
        HandleRoundState();
        UpdateUI();
    }

    void LateUpdate()
    {
        transform.position = bomb.transform.position + offset;
    }

    void UpdateUI()
    {
        roundInfoText.text = string.Format("Round Status: {0} Timer: {1:00.00}", currentState.ToString(), timeElapsed);
        goalInfoText.text = string.Format("Blue L:{0} C:{1} R:{2}       Red L:{3} C:{4} R:{5}", blueDoors.leftDoor.activeSelf, blueDoors.centerDoor.activeSelf, blueDoors.rightDoor.activeSelf, redDoors.leftDoor.activeSelf, redDoors.centerDoor.activeSelf, redDoors.rightDoor.activeSelf);
        SetHud();
    }

    void TogglePauseRound()
    {
        if (currentState == RoundState.running)
        {
            SetState(RoundState.paused);
        }
        else if (currentState == RoundState.paused)
        {
            SetState(RoundState.unPausing);
        }
    }

    void PauseRound()
    {
        bomb.GetComponent<BombController>().Pause();
    }

    void UnPauseRound()
    {
        bomb.GetComponent<BombController>().UnPause();
        SetState(RoundState.running);
    }

    void ResetTimers()
    {
        timeElapsed = 0;
    }

    void IncrementTimers(float deltaTime)
    {
        if (currentState == RoundState.running)
        {
            timeElapsed += deltaTime;
        }
    }

    void SetState(RoundState newState)
    {
        // State Transition Validation Logic
        if (currentState == RoundState.starting && newState != RoundState.running)
        {
            MyLog("Invalid round state transition");
        }

        if (currentState == RoundState.running && (newState != RoundState.paused && newState != RoundState.finished))
        {
            MyLog("Invalid round state transition");
        }

        if (currentState == RoundState.paused && newState != RoundState.unPausing)
        {
            MyLog("Invalid round state transition");
        }

        if (currentState == RoundState.unPausing && (newState != RoundState.running && newState != RoundState.finished))
        {
            MyLog("Invalid round state transition");
        }

        if (currentState == RoundState.finished && newState != RoundState.starting)
        {
            MyLog("Invalid round state transition");
        }

        // Update the currentState so we can have it when calling e.g. the bomb's functions
        currentState = newState;

        // Actions to do AFTER updating the currentState
        if (newState == RoundState.starting)
        {
            MyLog(newState.ToString());
            ResetTimers();
            bomb.GetComponent<BombController>().Arm();
        }
        else if (newState == RoundState.running)
        {
            MyLog(newState.ToString());
        }
        else if (newState == RoundState.paused)
        {
            MyLog(newState.ToString());
            PauseRound();
        }
        else if (newState == RoundState.unPausing)
        {
            MyLog(newState.ToString());
            UnPauseRound();
        }
        else if (newState == RoundState.finished)
        {
            MyLog(newState.ToString());
            bomb.GetComponent<BombController>().Explode();
            //if game finished and dont call game over function then i call it
            if(scoreBoardPanel.activeSelf == false)
            {
                GameOver();
            }
        }
        else
        {
            MyLog("Unknown round state");
        }
    }

    void HandleRoundState()
    {
        if (currentState == RoundState.starting)
        {
            SetState(RoundState.running);
        }
        else if (currentState == RoundState.running)
        {
            if (timeElapsed > maxRoundTime || DoorsExploded(redDoors) || DoorsExploded(blueDoors))
            {
                SetState(RoundState.finished);
            }
        }
    }

    void MyLog(string msg)
    {
        Debug.Log(string.Format("GameManager-{0}", msg));
    }

    public void SetBombState(BombState bs)
    {
        bombState = bs;
    }
    public BombState GetBombState()
    {
        return bombState;
    }

    void SetHud()
    {
        bool blueDoorRStatus = blueDoors.rightDoor.activeSelf;
        bool blueDoorCStatus = blueDoors.centerDoor.activeSelf;
        bool blueDoorLStatus = blueDoors.leftDoor.activeSelf;
        bool redDoorRStatus = redDoors.rightDoor.activeSelf;
        bool redDoorCStatus = redDoors.centerDoor.activeSelf;
        bool redDoorLStatus = redDoors.leftDoor.activeSelf;

        foreach (HudComponents hudComponent in hudComponents)
        {
            hudComponent.blueDoorLIcon.SetActive(blueDoorLStatus);
            hudComponent.blueDoorCIcon.SetActive(blueDoorCStatus);
            hudComponent.blueDoorRIcon.SetActive(blueDoorRStatus);
            hudComponent.redDoorLIcon.SetActive(redDoorLStatus);
            hudComponent.redDoorCIcon.SetActive(redDoorCStatus);
            hudComponent.redDoorRIcon.SetActive(redDoorRStatus);

            hudComponent.timeTxt.text = (maxRoundTime - timeElapsed).ToString("00");
        }
        
    }

    public void GameOver()
    {
        SetScoreBoardUI();
        gameOverFlag = true;
        //after some sec return to main scene
        //StartCoroutine(GoToMainMenu());
    }
    
    private Team WinnerDecider()
    {
        // Calculate winner based on who blew up all the opponents doors
        if (DoorsExploded(redDoors))
        {
            return Team.blue;
        }
        if (DoorsExploded(blueDoors))
        {
            return Team.red;
        }

        // Calculate winner based on score if round finished because of time
        int redTeamScore = 0;
        int blueTeamScore = 0;

        for (int i = 0; i < playersGO.Length; i++)
        {
            HoverControl hoverControl = playersGO[i].GetComponent<HoverControl>();
            PlayerStats playerStats = playersGO[i].GetComponent<PlayerStats>();
            if (hoverControl.myTeam == Team.blue)
            {
                blueTeamScore += playerStats.score;
            }
            else
            {
                redTeamScore += playerStats.score;
            }
        }

        if (blueTeamScore > redTeamScore)
        {
            return Team.blue;
        }
        else
        {
            return Team.red;
        }
    }

    private void SetScoreBoardUI()
    {
        SoundManager.PlayMusic(scoreBoardMusic, volume, true, true,2f,1f);

        scoreBoardPanel.SetActive(true);

        Team winner = WinnerDecider();

        if (winner == Team.blue)
        {
            // Hide/Unhide text based on who won
            textBlueWinner.SetActive(true);
            textRedWinner.SetActive(false);
        }
        else if (winner == Team.red)
        {
            // Hide/Unhide text based on who won
            textBlueWinner.SetActive(false);
            textRedWinner.SetActive(true);
        }

        int blueTeamIndex = 0;
        int redTeamIndex = 0;
        for(int i =0; i < playersGO.Length ; i++)
        {
            HoverControl hoverControl = playersGO[i].GetComponent<HoverControl>();
            PlayerStats playerStats = playersGO[i].GetComponent<PlayerStats>();
            if(hoverControl.myTeam == Team.blue)
            {
                blueTeamScoreBoard[blueTeamIndex].playerName.text = hoverControl.myPlayer.ToString();
                blueTeamScoreBoard[blueTeamIndex].kills.text = playerStats.kills.ToString();
                blueTeamScoreBoard[blueTeamIndex].deaths.text = playerStats.deaths.ToString();
                blueTeamScoreBoard[blueTeamIndex].score.text = playerStats.score.ToString();
                blueTeamIndex++;
            }
            else
            {
                redTeamScoreBoard[redTeamIndex].playerName.text = hoverControl.myPlayer.ToString();
                redTeamScoreBoard[redTeamIndex].kills.text = playerStats.kills.ToString();
                redTeamScoreBoard[redTeamIndex].deaths.text = playerStats.deaths.ToString();
                redTeamScoreBoard[redTeamIndex].score.text = playerStats.score.ToString();
                redTeamIndex ++;
            }
        }
    }

    private void SetControllersInHoverCarsGO()
    {
        int blueIndex = 0;
        int redIndex = 0;
        for (int i = 0; i < playersGO.Length; i++)
        {
            int controllerTeam = 0;
            if (PlayerPrefs.GetString("FromScene", "") == "SelectTeam")
            { // To tell if we came from select team scene

                controllerTeam = PlayerPrefs.GetInt("Controller" + i);
            }
            else
            {
                // Special handling if launching scene directly (skipping select team)
                if (i < 2)
                {
                    controllerTeam = -1; // Top players blue
                }
                else
                {
                    controllerTeam = 1; // Bottom players red
                }
            }
            
            if (controllerTeam == -1)//team blue
            {
                HoverControl hoverControl = playersGO[blueIndex].GetComponent<HoverControl>(); //playersGO 0 and 1 is blue team
                hoverControl.myPlayer = (MyPlayer)(i + 1);
                blueIndex++;
            }
            else if (controllerTeam == 1)//team red
            {
                HoverControl hoverControl = playersGO[redIndex + 2].GetComponent<HoverControl>(); //playersGO 2 and 3 is red team
                hoverControl.myPlayer = (MyPlayer)(i + 1);
                redIndex++;
            }
            else
            {
                Debug.LogError("Unknown team");
            }
                            
        }
    }

    private void GoToMainMenu()
    {
        //yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("MainMenu");
    }

    public void BombExploded()
    {
        SoundManager.PlayMusic(backgroundMusic, volume, true, true, 2f, 1f); // Reset music
    }

    private bool DoorsExploded(GoalStruct doors)
    {
        if (doors.leftDoor.activeSelf == false && doors.centerDoor.activeSelf == false && doors.rightDoor.activeSelf == false)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
