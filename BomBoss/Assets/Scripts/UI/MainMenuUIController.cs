using EazyTools.SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{

    [Header("UIComponents")]
    public GameObject[] buttons;
    public GameObject[] options;
    public Text systemInfoText;
    public GameObject instructions;

    [Header("Colors")]
    public Color selectedButtonColor;
    public Color unSelectedButtonColor;

    [Header("Panels")]
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    [Header("MainController")]
    public MyPlayer myPlayer;

    [Header("Audio")]
    public AudioClip backgroundMusic;
    public AudioClip creditsSFX;


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
        SetSystemInfoTxt();
        SetCreditsTxt();

        SoundManager.PlayMusic(backgroundMusic, volume, true, true, 2f, 1f);
    }

    private void Update()
    {
        ReadInputs();
        if ((optionsPanel.activeSelf == false) && (creditsPanel.activeSelf == false))
        {
            MainButtonsController();
            if (resetMusic == true)
            {
                SoundManager.PlayMusic(backgroundMusic, volume, true, true, 2f, 1f);
                resetMusic = false;
            }
        }
        else if (optionsPanel.activeSelf == true)
        {
            //in options panel
            OptionsController();
            if (resetMusic == true)
            {
                SoundManager.PlayMusic(backgroundMusic, volume, true, true, 2f, 1f);
                resetMusic = false;
            }
        }
        else
        {
            CreditsController();
            if (resetMusic == false)
            {
                SoundManager.PlayMusic(creditsSFX, volume, true, true, 2f, 1f);
                resetMusic = true;
            }
        }
    }

    public void PressNewGameButton()
    {
        SceneManager.LoadScene("SelectTeam");
    }

    public void PressTutorialsButton()
    {

    }

    public void PressOptionsButton()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
        if(optionsPanel == true)
        {
            SetOptionsColors();
            SetOptionsText();
        }
    }

    public void PressCreditsButton()
    {
        instructions.SetActive(!instructions.activeSelf); // hide the instructions when in credits
        creditsPanel.SetActive(!creditsPanel.activeSelf);
    }

    public void PressExitGameButton()
    {
        Application.Quit();
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

    private void SetOptionsColors()
    {
        for (int i = 0; i < options.Length; i++)
        {
            if (i != indexOfSelectedOption)
            {
                //unelected button
                options[i].GetComponentInChildren<Text>().color = unSelectedButtonColor;
            }
            else
            {
                //unelected button
                options[i].GetComponentInChildren<Text>().color = selectedButtonColor;
            }


        }

    }

    private void SetOptionsText()
    {
        //QualityLevel
        options[0].GetComponentInChildren<Text>().text = QualitySettings.GetQualityLevel().ToString();
        //Anti-Aliasing
        options[1].GetComponentInChildren<Text>().text = QualitySettings.antiAliasing.ToString();
        //Anisotropic
        options[2].GetComponentInChildren<Text>().text = QualitySettings.anisotropicFiltering.ToString();
        //Volume
        options[3].GetComponentInChildren<Text>().text = Mathf.RoundToInt((volume * 10)).ToString();

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
                case 0: //new game
                    PressNewGameButton();
                    break;
                case 1: //tutorial
                    PressTutorialsButton();
                    break;
                case 2: //options
                    PressOptionsButton();
                    break;
                case 3: //creadits
                    PressCreditsButton();
                    break;
                case 4: //exit
                    PressExitGameButton();
                    break;

            }
        }
    }

    private void OptionsController()
    {

        if(backInput == true)
        {
            PressOptionsButton();
        }

        if (verticalInput != 0)
        {
            if (verticalInput < 0)
            {
                indexOfSelectedOption++;
                if (indexOfSelectedOption >= options.Length)
                {
                    indexOfSelectedOption = 0;
                }
            }
            else
            {
                indexOfSelectedOption--;
                if (indexOfSelectedOption < 0)
                {
                    indexOfSelectedOption = options.Length - 1;
                }
            }

            //set selected option color
            SetOptionsColors();
            Input.ResetInputAxes();
        }

        if (horizontalInput != 0)
        {
            if (horizontalInput > 0)
            {
                //increase values
                switch (indexOfSelectedOption)
                {
                    case 0: //QualityLevel
                        QualitySettings.IncreaseLevel(true);
                        break;
                    case 1: //Anti-Aliasing
                        QualitySettings.antiAliasing = Mathf.Min(QualitySettings.antiAliasing + 2, 8);
                        break;
                    case 2: //Anisotropic
                        string temp = QualitySettings.anisotropicFiltering.ToString();
                        if (temp == AnisotropicFiltering.Disable.ToString())
                        {
                            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                        }
                        else
                        {
                            QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                        }
                        break;
                    case 3: //Volume
                        volume = Mathf.Min(volume + 0.1f, 1f);
                        PlayerPrefs.SetFloat("Volume", volume);
                        break;
                }
            }
            else
            {
                //decrease values
                switch (indexOfSelectedOption)
                {
                    case 0: //QualityLevel
                        QualitySettings.DecreaseLevel(true);
                        break;
                    case 1: //Anti-Aliasing
                        QualitySettings.antiAliasing = Mathf.Max(QualitySettings.antiAliasing - 2, 0);
                        break;
                    case 2: //Anisotropic
                        string temp = QualitySettings.anisotropicFiltering.ToString();
                        if (temp == AnisotropicFiltering.ForceEnable.ToString())
                        {
                            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                        }
                        else
                        {
                            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                        }
                        break;
                    case 3: //Volume
                        volume = Mathf.Max(volume - 0.1f, 0f);
                        PlayerPrefs.SetFloat("Volume", volume);
                        break;
                }
            }

            //set text of options
            SetOptionsText();
            Input.ResetInputAxes();
        }
    }

    private void CreditsController()
    {
        if (backInput == true)
        {
            PressCreditsButton();
        }
    }

    private void SetSystemInfoTxt()
    {
        systemInfoText.text = "Machine Information: \n GPU: " + SystemInfo.graphicsDeviceName +
                            "\n GPU Memory: " + SystemInfo.graphicsMemorySize + "MB" +
                            "\n CPU: " + SystemInfo.processorType +
                            "\n RAM: " + SystemInfo.systemMemorySize + "MB";
    }

    private void SetCreditsTxt()
    {
        string creditsTxt = @"




The Bomb Boss Team


<color=#000000FF>--Software Engineers--</color>

Sergios Stamatis
Andreas Andreou
Michalis Prodromou


<color=#000000FF>--Event Programmers--</color>

Sergios Stamatis
Andreas Andreou
Michalis Prodromou


<color=#000000FF>--Battle Programmers--</color>

Sergios Stamatis
Andreas Andreou
Michalis Prodromou


<color=#000000FF>--Combat Programmers--</color>

Sergios Stamatis
Andreas Andreou
Michalis Prodromou


<color=#000000FF>--Game Design--</color>

Theodore Constandinou
Sergios Stamatis
Andreas Andreou
Michalis Prodromou


<color=#000000FF>--Level Designer--</color>

Theodore Constandinou


<color=#000000FF>--3D artist--</color>

Theodore Constandinou


<color=#000000FF>--UI artist--</color>

Theodore Constandinou


<color=#000000FF>--Texture Artist--</color>

Theodore Constandinou


<color=#000000FF>--Concept Artist--</color>

Theodore Constandinou


<color=#000000FF>--Sound Producer--</color>

Andreas Giavroutas


<color=#000000FF>--Sound Designer--</color>

Andreas Giavroutas


<color=#000000FF>--Sound FX Artist--</color>

Andreas Giavroutas


<color=#000000FF>--Musicians / Composer--</color>


Andreas Giavroutas


<color=#000000FF>The Story:</color>

Once upon a time a boy named Bomboss was in a classroom and the teacher asked which is the fastest thing in the universe.
One student said that the fastest thing is light, another student said that the fastest thing was thought, and Bomboss
replied that the fastest thing in the universe is obviously diarrhea, because when it strikes you neither have time to think
nor switch on the light. Obviously this story has nothing to do with the game.


<color=#000000FF>GAME OVER!!!</color>";
        creditsPanel.GetComponentInChildren<Text>().text = creditsTxt;
    }
}
    