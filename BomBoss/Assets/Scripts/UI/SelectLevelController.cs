using EazyTools.SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectLevelController : MonoBehaviour
{

    [Header("UIComponents")]
    public GameObject[] buttons;

    [Header("Colors")]
    public Color selectedButtonColor;
    public Color unSelectedButtonColor;

    [Header("MainController")]
    public MyPlayer myPlayer;

    [Header("Audio")]
    public AudioClip backgroundMusic;

    private int indexOfSelectedButton;
    private int indexOfSelectedOption;

    //inputs
    private float verticalInput;
    private float horizontalInput;
    private bool selectInput;
    private bool backInput;

    //game manager
    private float volume;

    private bool resetMusic = false;

    private float debounceHorizontal = 0.0f;
    private float debounceVertical = 0.0f;
    private float repeat = 0.15f;  // reduce to speed up auto-repeat input

    private void Start()
    {

        volume = PlayerPrefs.GetFloat("Volume", 1f);

        indexOfSelectedButton = 0;
        indexOfSelectedOption = 0;
        SetButtonColors();

        SoundManager.PlayMusic(backgroundMusic, volume, true, true, 2f, 1f);
    }

    private void Update()
    {
        ReadInputs();
        MainButtonsController();
        if (resetMusic == true)
        {
            SoundManager.PlayMusic(backgroundMusic, volume, true, true, 2f, 1f);
            resetMusic = false;
        }
    }

    public void PressLevel(int levelIndex)
    {
        PlayerPrefs.SetInt("LevelIndex", levelIndex); // To tell which level to show
        SceneManager.LoadScene("SelectTeam");
    }

    private void SetButtonColors()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i != indexOfSelectedButton)
            {
                //unelected button
                buttons[i].GetComponentInChildren<Text>().color = unSelectedButtonColor;
            }
            else
            {
                //unelected button
                buttons[i].GetComponentInChildren<Text>().color = selectedButtonColor;
            }
        }
    }

    private void ReadInputs()
    {
        verticalInput = Input.GetAxis("Vertical" + myPlayer.ToString());
        horizontalInput = Input.GetAxis("Horizontal" + myPlayer.ToString());
        selectInput = Input.GetButton("Shoot" + myPlayer.ToString());
        backInput = Input.GetButton("Magnet" + myPlayer.ToString());

        // check if user let go of the stick; if so, reset the input bounce control
        if (Mathf.Abs(verticalInput) < 0.1f) { debounceVertical = 0.0f; }
        else { debounceVertical += Time.deltaTime; }
        if (Mathf.Abs(horizontalInput) < 0.1f) { debounceHorizontal = 0.0f; }
        else { debounceHorizontal += Time.deltaTime; }

        // if it's been long enough since the last input, then we allow it
        if (debounceVertical < repeat) { verticalInput = 0; }
        else { debounceVertical = 0; }
        if (debounceHorizontal < repeat) { horizontalInput = 0; }
        else { debounceHorizontal = 0; }
    }

    private void MainButtonsController()
    {
        //up and down
        if (verticalInput != 0)
        {
            if (verticalInput < 0)
            {
                indexOfSelectedButton++;
                if (indexOfSelectedButton >= buttons.Length)
                {
                    indexOfSelectedButton = 0;
                }
            }
            else
            {
                indexOfSelectedButton--;
                if (indexOfSelectedButton < 0)
                {
                    indexOfSelectedButton = buttons.Length - 1;
                }
            }

            //update selectet button color
            SetButtonColors();
            Input.ResetInputAxes();
        }

        if (selectInput == true)
        {
            switch (indexOfSelectedButton)
            {
                case 0: //level 1
                    PressLevel(0);
                    break;
                case 1: //level 2
                    PressLevel(1);
                    break;
            }
        }

        if (backInput == true)
        {
            GoPrevScene();
        }
    }

    private void GoPrevScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
    