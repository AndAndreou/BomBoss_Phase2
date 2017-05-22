using EazyTools.SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerBomb : MonoBehaviour {

    public bool canUsePowerUPS = true;
    

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
        public GameObject playerHUD;
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
    public GameObject textGray;
    public GameObject textNobodyWins;
    public GameObject pausePanel;
    public GameObject endingScene;

    [Header("Audio")]
    public AudioClip backgroundMusic;
    public AudioClip scoreBoardMusic;
    public AudioClip level1_music;
    public AudioClip level2_music;

    private float volume;

    private bool gameOverFlag;

    private float defaultTimeScale = 1.0f;

    private void Awake()
    {
        SetLevel();
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

        if (Input.GetKeyDown(KeyCode.P) || Input.GetButtonDown("PausePlayer1"))
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
        // TODO: Pause the hovercraft power-ups and shoot
        bomb.GetComponent<BombController>().Pause();
        Time.timeScale = 0.0f;
        pausePanel.SetActive(true);

        canUsePowerUPS = false;
    }

    public void UnPauseRound()
    {
        bomb.GetComponent<BombController>().UnPause();
        SetState(RoundState.running);
        Time.timeScale = defaultTimeScale;
        pausePanel.SetActive(false);
        canUsePowerUPS = true;
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
        foreach (GameObject playerGO in playersGO)
        {
            Camera playerCamera = playerGO.GetComponentInChildren<Camera>();
            playerCamera.enabled = false;
        }
        foreach (HudComponents hudComponent in hudComponents)
        {
            hudComponent.playerHUD.SetActive(false);
        }
        endingScene.SetActive(true);
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
            else if (hoverControl.myTeam == Team.red)
            {
                redTeamScore += playerStats.score;
            }
        }
        //blue team scored more goals
        if (blueTeamScore > redTeamScore)
        {
            return Team.blue;
        }
        //red team scored more goals
        else if (blueTeamScore < redTeamScore)
        {
            return Team.red;
        }
        //The two teams are tied. Winner is the team with most kills /*if (blueTeamScore == redTeamScore)*/
        else if (blueTeamScore == redTeamScore)
        {
            // Calculate winner based on kills if round finished because of time
            int redTeamKills = 0;
            int blueTeamKills = 0;

            for (int i = 0; i < playersGO.Length; i++)
            {
                HoverControl hoverControl = playersGO[i].GetComponent<HoverControl>();
                PlayerStats playerStats = playersGO[i].GetComponent<PlayerStats>();
                if (hoverControl.myTeam == Team.blue)
                {
                    blueTeamKills += playerStats.kills;
                    Debug.Log("Blue player kills ++");
                }
                else if (hoverControl.myTeam == Team.red)
                {
                    redTeamKills += playerStats.kills;
                    Debug.Log("Red player kills ++");
                }
            }
            Debug.Log("Blue kills: " + blueTeamKills + "Red kills: " + redTeamKills);

            if (blueTeamKills > redTeamKills)
            {
                return Team.blue;
                Debug.Log("Blue team wins");
            }
            //red team scored more kills
            else if (blueTeamKills < redTeamKills)
            {
                return Team.red;
                Debug.Log("red team wins");
            }
            //The two teams are tied. Winnes is the team with most kills
            else if (blueTeamKills == redTeamKills)
            {
                // Calculate winner based on deaths if both teams have the same score
                int redTeamDeaths = 0;
                int blueTeamDeaths = 0;

                for (int i = 0; i < playersGO.Length; i++)
                {
                    HoverControl hoverControl = playersGO[i].GetComponent<HoverControl>();
                    PlayerStats playerStats = playersGO[i].GetComponent<PlayerStats>();
                    if (hoverControl.myTeam == Team.blue)
                    {
                        blueTeamDeaths += playerStats.deaths;
                        Debug.Log("Blue deaths kills ++");
                    }
                    else if (hoverControl.myTeam == Team.red)
                    {
                        redTeamDeaths += playerStats.deaths;
                        Debug.Log("Red deaths kills ++");
                    }
                }

                if (blueTeamDeaths > redTeamDeaths)
                {
                    return Team.red;
                    Debug.Log("Red team wins");
                }
                //red team scored more kills
                else if (blueTeamDeaths < redTeamDeaths)
                {
                    return Team.blue;
                    Debug.Log("Blue team wins");
                }
                else {
                    return Team.isNull;
                    Debug.Log("Nobody wins");
                }


                //return Team.isNull;
            }
        }

        return Team.isNull;

       
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
        else if(winner == Team.isNull)
        {
            textBlueWinner.SetActive(false);
            textRedWinner.SetActive(false);
            textGray.SetActive(false);
            textNobodyWins.SetActive(true);
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

    private void SetLevel()
    {
        // Pick the level that was set in SelectTeam screen
        int levelIndex = PlayerPrefs.GetInt("LevelIndex", 0);
        string levelGOPrefix = "LVL";

        if (levelIndex == 0)
        {
            GameObject.Find(levelGOPrefix + "1").SetActive(true);
            GameObject.Find(levelGOPrefix + "2").SetActive(false);
            backgroundMusic = level1_music;
        }
        else if (levelIndex == 1)
        {
            GameObject.Find(levelGOPrefix + "1").SetActive(false);
            GameObject.Find(levelGOPrefix + "2").SetActive(true);
            backgroundMusic = level2_music;
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
