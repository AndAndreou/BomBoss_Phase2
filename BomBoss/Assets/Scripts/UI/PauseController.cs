using EazyTools.SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{

    [Header("UIComponents")]
    public GameObject[] buttons;

    [Header("Colors")]
    public Color selectedButtonColor;
    public Color unSelectedButtonColor;

    [Header("MainController")]
    public MyPlayer myPlayer;

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
    private float repeat = 0.1f;  // reduce to speed up auto-repeat input

    public GameManagerBomb gameManagerBomb;

    private void Start()
    {
        indexOfSelectedButton = 0;
        indexOfSelectedOption = 0;
        SetButtonColors();
    }

    private void Update()
    {
        ReadInputs();
        MainButtonsController();

    }

    public void PressResume()
    {
        gameManagerBomb.UnPauseRound();
    }

    public void PressRestart()
    {
        Time.timeScale = 1.0f;
        SoundManager.StopAllMusic();
        SceneManager.LoadScene("newMergeScene4");
    }

    public void PressReturnToMainMenu()
    {
        Time.timeScale = 1.0f;
        SoundManager.StopAllMusic();
        SceneManager.LoadScene("MainMenu");
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
        else { debounceVertical += Time.unscaledDeltaTime; }
        if (Mathf.Abs(horizontalInput) < 0.1f) { debounceHorizontal = 0.0f; }
        else { debounceHorizontal += Time.unscaledDeltaTime; }

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
            if (verticalInput > 0)
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
            //Input.ResetInputAxes();
        }

        if (selectInput == true)
        {
            switch (indexOfSelectedButton)
            {
                case 0: //pause
                    PressResume();
                    break;
                case 1: //restart
                    PressRestart();
                    break;
                case 2: //return to main menu
                    PressReturnToMainMenu();
                    break;
            }
        }

        if (backInput == true)
        {
            PressResume();
        }
    }
}
