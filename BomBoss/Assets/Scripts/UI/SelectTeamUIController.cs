using EazyTools.SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectTeamUIController : MonoBehaviour {

    [Header("GameObjects")]
    public GameObject loadingGO;
    public GameObject instructionsGO;

    [Header("Animators")]
    public Animator[] controllers = new Animator[4];

    [Header("UIComponent")]
    public Text[] numberOfControllers = new Text[4];

    [Header("Colors")]
    public Color readyColor;
    public Color unReadyColor;

    [Header("Audio")]
    public AudioClip backgroundMusic;

    private int[] teamOfEachController = new int[4];
    private bool[] playerReady = new bool[4];

    private float volume;

    private float[] debounceHorizontal = new float[4];
    private float repeat = 0.15f;  // reduce to speed up auto-repeat input

    private bool callOtherScene;
    // Use this for initialization
    void Start () {
        callOtherScene = false;

        volume = PlayerPrefs.GetFloat("Volume", 1f);
        SoundManager.PlayMusic(backgroundMusic, volume, true, true, 2f, 1f);
    }
	
	// Update is called once per frame
	void Update () {

        if(callOtherScene == true)
        {
            return;
        }

        ReadInputs();
        CheckIfAllReadyOrGoBack();
    }

    private void ReadInputs()
    {
        for(int i = 0; i < teamOfEachController.Length; i++)
        {

            //ready button
            bool readyInput = Input.GetButtonDown("ShootPlayer" + (i + 1).ToString());
            if((readyInput == true) && (teamOfEachController[i] != 0))
            {
                playerReady[i] = true;  
            }

            //back button
            bool backInput = Input.GetButtonDown("MagnetPlayer" + (i + 1).ToString());
            if (backInput == true)
            {
                //if all player is unready and some one press back button, then go to previous sscene
                if(!playerReady[0] && !playerReady[0] && !playerReady[0] && !playerReady[0] && backInput)
                {
                    callOtherScene = true;
                    GoPrevScene();
                }
                playerReady[i] = false;
            }

            if (playerReady[i] == true)
            {
                continue;
            }

            float horizontalInput = Input.GetAxis("HorizontalPlayer" + (i+1).ToString());
            // BEGIN Debounce the input
            if (Mathf.Abs(horizontalInput) < 0.1f) { debounceHorizontal[i] = 0.0f; }
            else { debounceHorizontal[i] += Time.deltaTime; }
            if (debounceHorizontal[i] < repeat) { horizontalInput = 0; }
            else { debounceHorizontal[i] = 0; }
            // END Debounce the input

            if (horizontalInput != 0)
            {
                if (horizontalInput > 0.5f)
                {
                    teamOfEachController[i] = Mathf.Min(teamOfEachController[i] + 1, 1);
                }
                else if (horizontalInput < -0.5f)
                {
                    teamOfEachController[i] = Mathf.Max(teamOfEachController[i] - 1, -1);
                }
            }
        }
        SetUIControllersAnimator();
        //Input.ResetInputAxes();
    }

    private void SetUIControllersAnimator()
    {
        for (int i = 0; i < teamOfEachController.Length; i++)
        {
            controllers[i].SetInteger("Team", teamOfEachController[i]);
            if (playerReady[i] == true)
            {
                numberOfControllers[i].color = readyColor;
            }
            else
            {
                numberOfControllers[i].color = unReadyColor;
            }
        }
    }

    private void CheckIfAllReadyOrGoBack()
    {
        bool isAllReady = true;
        bool isAllUnReady = true;
        int teamsCounter = 0;
        
        if (!UnityEngine.Debug.isDebugBuild) 
        {
            // Check all 4 players if ready
            for (int i = 0; i < playerReady.Length; i++)
            {
                isAllReady = isAllReady && playerReady[i];
                isAllUnReady = isAllUnReady && !playerReady[i];
                teamsCounter += teamOfEachController[i];
            }
        }
        else
        {
            // To allow playing in debug mode with connected controllers
            for (int i = 0; i < UnityEngine.Input.GetJoystickNames().Length; i++)
            {
                isAllReady = isAllReady && playerReady[i];
                isAllUnReady = isAllUnReady && !playerReady[i];
                teamsCounter = 0;
            }
        }

        if (isAllReady == true)
        {
            if(teamsCounter == 0) //means teams is balance
            {
                callOtherScene = true;
                instructionsGO.SetActive(false);
                loadingGO.SetActive(true);
                StartCoroutine(GoNextScene());
            }
            return;
        }

    }

    private IEnumerator GoNextScene()
    {
        //blue team is -1
        //red team is 1
        //none team is 0

        PlayerPrefs.SetString("FromScene", "SelectTeam"); // To tell if we came from select team scene

        for (int i = 0; i < teamOfEachController.Length; i++)
        {
            PlayerPrefs.SetInt("Controller" + i, teamOfEachController[i]);
        }
        
        SceneManager.LoadSceneAsync("newMergeScene4");
        yield return null;
    }

    private void GoPrevScene()
    {
        SceneManager.LoadScene("SelectLevel");
    }

}
