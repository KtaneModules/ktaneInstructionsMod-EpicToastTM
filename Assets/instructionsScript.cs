using UnityEngine;
using KModkit;
using System.Collections;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public class instructionsScript : MonoBehaviour {

    public KMAudio Audio;
    public KMBombModule Module;
    public KMBombInfo Info;
    public KMSelectable[] buttons;
    public KMSelectable[] screenButtonSelectables;
    public MeshRenderer[] screenButtons;
    public TextMesh label1, label2, label3, label4, screen;
    public MeshRenderer buttonOne, buttonTwo, buttonThree, buttonFour;
    public GameObject ModuleObject;

    private static int _moduleIDCounter = 1;
    private int _moduleID = 0;
    private bool _solved = false, _lightsOn = false;

    private int counter;
    private string[] edgeworkPossibilities = { "BATTERIES", "BATTERY\nHOLDERS", "INDICATORS", "LIT INDICATORS", "UNLIT\nINDICATORS", "PORTS", "PORT PLATES", "DIGITS IN\nSERIAL NUMBER",
        "LETTERS IN\nSERIAL NUMBER", "MODULES", "TWO FACTORS", "SOLVED MODULES", "PORT TYPES", "STRIKES" };
    private string[] buttonPossibilities = { "RED", "GREEN", "YELLOW", "BLUE", "A", "B", "C", "D", "FIRST", "SECOND", "THIRD", "FOURTH" };
    private string[] labels = { "A", "B", "C", "D" };
    private int correctBtn;
    private int[] edgework = new int[14];
    private string[] startingScreens = { "WHO TURNED\nTHE LIGHTS OFF", "KAPOW KAPOW\nKAPOW KAPOW", "TEXT GOES\nHERE, I GUESS", "FUNNY JOKE\nHERE", "GG, YOU CAN\nREAD", "OH CRAP, THE TEXT\nGOES OFF THE SCREEN", "E", "OH SHOOT\nIT'S A BOMB", "HEY I'M YOUR\nFAVORITE MODULE,\nRIGHT?", "\n\n\n\n\n\n\n\n\n\n\n\nI'M DOWN HERE NOW", "CONGRATULATIONS\nYOU FOUND AN\nEASTER EGG", "ARE YOU GOING TO\nBLOW UP THIS\nBOMB?", "SUBSCRIBE TO\nPEWDIEPIE", "I'M GOING TO SAY\nTHE N WORD", "CHALLENGE.\n3, 2, 1.", "*DOES DEFAULT\nDANCE*", "IT'S TIME TO STOP", "STOP READING THE GITHUB" };
    private string[] solveMessages = { "GG", "NICE JOB", "MODULE\nDISARMED", ":D", "MODULE\nSOLVED", "*INSERT CLAP\nEMOJI HERE*", "WOOOOOOOO!", "OH CRAP YOU\nACTUALLY DID IT", "THAT WAS NICE\nOWO", "+6 POINTS!", "AW MAN, NOW\nNOBODY CARES\nABOUT ME :(", ":OK_HAND:", "A WINNER IS YOU", "I'M SO PROUD\nOF YOU", "NEVER MIND, IT\nSOLVED ITSELF.", "I'LL GET YOU NEXT\nTIME" };
    private string[] failureMessages = { "NOPE", "OOF", "+1 STRIKE!", "SMH", "JUST READ THE\nINSTRUCTIONS", "DEFINITELY NOT\nA BUG :)))))", "KABOOOOM!", ":(", "THIS IS SO SAD\nALEXA, PLAY\nDESPACITO", "BETTER CHECK\nTHE LOG!", "REVENGE!!!", "O\nO\nF", "YOU'RE DEAD TO\nME", "...", "YOU'VE VIOLATED AN\nAREA PROTECTED BY\nA SECURITY SYSTEM.", "OH NOOO" };
    private int[,] screens = { { 99, 0 }, { 99, 0 }, { 99, 0 }, { 99, 0 }, { 99, 0 } };
    private static readonly bool[] edgeworkScreens = { true, false, true, false, false };
    private int[,] buttonTypes = { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };
    // Use this for initialization (and putting memes on the screen)
    void Start () {
        _moduleID = _moduleIDCounter++;
        Module.OnActivate += Activate;
        screen.text = startingScreens[Random.Range(0, startingScreens.Length - 1)];
    }

    private void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            int j = i;
            buttons[i].OnInteract += delegate ()
            {
                ButtonPressed(j);
                buttons[j].AddInteractionPunch();
                return false;
            };
        }

        for (int i = 0; i < 5; i++)
        {
            int h = i;
            screenButtonSelectables[i].OnInteract += delegate ()
            {
                ScreenButtonPressed(h);
                screenButtonSelectables[h].AddInteractionPunch();
                return false;
            };
        }
    }

    void Activate()
    {
        Init();
        _lightsOn = true;

        GenerateEdgework();
    }

    void Init()
    {
        // Makes the first screen light green

        screenButtons[0].material.color = new Color32(0, 255, 0, 255);
        screenButtons[1].material.color = new Color32(0, 0, 0, 255);
        screenButtons[2].material.color = new Color32(0, 0, 0, 255);
        screenButtons[3].material.color = new Color32(0, 0, 0, 255);
        screenButtons[4].material.color = new Color32(0, 0, 0, 255);

        GenerateEdgework();
        GenerateButtons();
        GenerateScreens();
    }
    
    void ButtonPressed(int btnNumber)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform);

        if (!_lightsOn || _solved) return;

        GenerateAnswer();

        if (btnNumber == correctBtn)
        {
            _solved = true;

            Module.HandlePass();
            
            Audio.PlaySoundAtTransform("SolveSound", Module.transform);

            Debug.LogFormat("[Instructions #{0}] Button {1} was pressed. Module solved!!!!1!", _moduleID, btnNumber + 1);

            for (int i = 0; i < 5; i++)
            {
                screenButtons[i].material.color = new Color32(0, 255, 0, 255);
                screen.text = solveMessages[Random.Range(0, solveMessages.Length)];
            }
        }

        else
        {
            Module.HandleStrike();

            Debug.LogFormat("[Instructions #{0}] Button {1} was pressed. Strike.", _moduleID, btnNumber + 1);

            StartCoroutine("StrikeAnimation");
        }
    }

    IEnumerator StrikeAnimation()
    {
        screen.text = failureMessages[Random.Range(0, failureMessages.Length)];

        for (int i = 0; i < 5; i++)
        {
            
            screenButtons[i % 5].material.color = new Color32(255, 0, 0, 255);
            screenButtons[(i + 1) % 5].material.color = new Color32(0, 0, 0, 255);
            screenButtons[(i + 2) % 5].material.color = new Color32(0, 0, 0, 255);
            screenButtons[(i + 3) % 5].material.color = new Color32(0, 0, 0, 255);
            screenButtons[(i + 4) % 5].material.color = new Color32(0, 0, 0, 255);

            yield return new WaitForSeconds(.25f);

        }
        
        Init();
    }

    // Handles screen button presses

    void ScreenButtonPressed(int btnNumber)
    {
        if (!_lightsOn || _solved) return;

        for (int i = 0; i < 5; i++)
        {
            screenButtons[i].material.color = new Color32(0, 0, 0, 255);
        }

        screenButtons[btnNumber].material.color = new Color32(0, 255, 0, 255);

        if (edgeworkScreens[btnNumber])
        {
            screen.text = edgeworkPossibilities[screens[btnNumber, 0]];
        }

        else
        {
            screen.text = buttonPossibilities[screens[btnNumber, 0]];
        }
    }

    public string TwitchHelpMessage = "!{0} press 1 will press the first button at the bottom, !{0} press 2 will press the second button, etc. You can do !{0} cycle to cycle through all of the screens.";
    IEnumerator ProcessTwitchCommand(string command)
    {
        if (command.ToLower().Equals("press 1"))
        {
            yield return null;
            yield return new KMSelectable[] { buttons[0] };
        }

        else if (command.ToLower().Equals("press 2"))
        {
            yield return null;
            yield return new KMSelectable[] { buttons[1] };
        }

        else if (command.ToLower().Equals("press 3"))
        {
            yield return null;
            yield return new KMSelectable[] { buttons[2] };
        }

        else if (command.ToLower().Equals("press 4"))
        {
            yield return null;
            yield return new KMSelectable[] { buttons[3] };
        }

        else if (command.ToLower().Equals("cycle"))
        {
            yield return null;
            yield return new KMSelectable[] { screenButtonSelectables[0] };
            yield return new WaitForSeconds(.5f);
            yield return new KMSelectable[] { screenButtonSelectables[1] };
            yield return new WaitForSeconds(2);
            yield return new KMSelectable[] { screenButtonSelectables[2] };
            yield return new WaitForSeconds(2);
            yield return new KMSelectable[] { screenButtonSelectables[3] };
            yield return new WaitForSeconds(2);
            yield return new KMSelectable[] { screenButtonSelectables[4] };
            yield return new WaitForSeconds(2);
            yield return new KMSelectable[] { screenButtonSelectables[0] };
        }

        else
        {
            yield break;
        }
    }
    
    void GenerateAnswer()
    {
        if (screens[0, 1] == 0)
        {
            if (screens[4, 1] > screens[1, 1])
            {
                correctBtn = screens[4, 1];
            }

            else if (screens[4, 1] < screens[1, 1])
            {
                correctBtn = screens[1, 1];
            }

            else
            {
                correctBtn = screens[3, 1];
            }

            Debug.LogFormat("[Instructions #{0}] Rule 1 applied.", _moduleID);
        }

        else if (screens[0, 1] < screens[2, 1])
        {
            correctBtn = screens[1, 1];
            Debug.LogFormat("[Instructions #{0}] Rule 2 applied.", _moduleID);
        }

        else if (screens[1, 1] < screens[3, 1])
        {
            correctBtn = screens[4, 1];
            Debug.LogFormat("[Instructions #{0}] Rule 3 applied.", _moduleID);
        }

        else if (screens[2, 1] > 3)
        {
            correctBtn = screens[0, 1] % 4;
            Debug.LogFormat("[Instructions #{0}] Rule 4 applied.", _moduleID);
        }

        else if (screens[1, 1] != screens[3, 1] && screens[1, 1] != screens[4, 1] && screens[4, 1] != screens[3, 1])
        {
            for (int i = 0; i < 4; i++)
            {
                if (i != screens[1, 1] && i != screens[3, 1] && i != screens[4, 1])
                {
                    correctBtn = i;
                    break;
                }
            }

            Debug.LogFormat("[Instructions #{0}] Rule 5 applied.", _moduleID);
        }

        else
        {
            correctBtn = screens[3, 1];
            Debug.LogFormat("[Instructions #{0}] Rule 6 applied.", _moduleID);
        }

        Debug.LogFormat("[Instructions #{0}] Press button {1}.", _moduleID, correctBtn + 1);
    }

    void GenerateScreens()
    {
        // For the edgework screens, possible words are: BATTERIES, BATTERY HOLDERS, INDICATORS, LIT INDICATORS, UNLIT INDICATORS, PORTS, PORT PLATES, DIGITS IN SERIAL NUMBER, LETTERS IN SERIAL NUMBER, MODULES.
        // For the button screens, possible words are: RED, GREEN, YELLOW, BLUE, A, B, C, D, FIRST, SECOND, THIRD, FOURTH.
        for (int screenNum = 0; screenNum < 5; screenNum++)
        {
            if (edgeworkScreens[screenNum])
            {
                screens[screenNum, 0] = Random.Range(0, edgeworkPossibilities.Length);

                for (int i = 0; i < 5; i++)
                {
                    if (screens[i, 0] == screens[screenNum, 0] && screenNum != i)
                    {
                        screens[screenNum, 0] = (screens[screenNum, 0] + 1) % edgeworkPossibilities.Length;
                    }
                }
            }

            else
            {
                screens[screenNum, 0] = Random.Range(0, buttonPossibilities.Length);

                for (int x = 0; x < 2; x++)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (screens[i, 0] == screens[screenNum, 0] && screenNum != i)
                        {
                            screens[screenNum, 0] = (screens[screenNum, 0] + 1) % buttonPossibilities.Length;
                        }
                    }
                }
            }
        }

        for (int screenNum = 0; screenNum < 5; screenNum++)
        {
            if (edgeworkScreens[screenNum])
            {
                screens[screenNum, 1] = edgework[screens[screenNum, 0]];
            }

            else
            {
                if (screens[screenNum, 0] < 4)
                {
                    screens[screenNum, 1] = screens[screenNum, 0];
                }

                else if (screens[screenNum, 0] < 8)
                {
                    screens[screenNum, 1] = screens[screenNum, 0];
                }
            }
        }

        screen.text = edgeworkPossibilities[screens[0, 0]];
        
        for (int i = 0; i < 5; i++)
        {
            if (edgeworkScreens[i])
            {
                Debug.LogFormat("[Instructions #{0}] Screen {1} says {2}.", _moduleID, i + 1, edgeworkPossibilities[screens[i, 0]].Replace("\n"," "));
            }

            else
            {
                Debug.LogFormat("[Instructions #{0}] Screen {1} says {2}.", _moduleID, i + 1, buttonPossibilities[screens[i, 0]].Replace("\n", " "));
            }
        }

        // Generate values

        for (int i = 0; i < 5; i++)
        {
            if (edgeworkScreens[i])
            {
                screens[i, 1] = edgework[screens[i, 0]];
            }

            else
            {
                if (screens[i, 0] < 4)
                {
                    int btnNumber = 0;

                    for (int x = 0; x < 4; x++)
                    {
                        if (buttonTypes[x, 0] == screens[i, 1])
                        {
                            btnNumber = x;
                            break;
                        }
                    }

                    screens[i, 1] = btnNumber;
                }

                else if (screens[i, 0] < 8)
                {
                    int btnNumber = 0;

                    for (int x = 0; x < 4; x++)
                    {
                        if (buttonTypes[x, 1] == screens[i, 1] - 4)
                        {
                            btnNumber = x;
                            break;
                        }
                    }

                    screens[i, 1] = btnNumber;
                }

                else
                {
                    screens[i, 1] = screens[i, 0] - 8;
                }
            }
        }
    }

    void GenerateButtons()
    {
        // Button color generation

        for (int i = 0; i < 4; i++)
        {
            buttonTypes[i, 0] = Random.Range(0, 4);
        }

        // 0 = Red, 1 = Green, 2 = Yellow, 3 = Blue

        if (buttonTypes[0, 0] == 0)
        {
            buttonOne.material.color = new Color32(255, 0, 0, 255);
        }
        else if (buttonTypes[0, 0] == 1)
        {
            buttonOne.material.color = new Color32(0, 255, 0, 255);
        }
        else if (buttonTypes[0, 0] == 2)
        {
            buttonOne.material.color = new Color32(200, 200, 0, 255);
        }
        else
        {
            buttonOne.material.color = new Color32(0, 0, 255, 255);
        }

        if (buttonTypes[1, 0] == buttonTypes[0, 0])
        {
            buttonTypes[1, 0] = (buttonTypes[1, 0] + 1) % 4;
        }

        if (buttonTypes[1, 0] == 0)
        {
            buttonTwo.material.color = new Color32(255, 0, 0, 255);
        }
        else if (buttonTypes[1, 0] == 1)
        {
            buttonTwo.material.color = new Color32(0, 255, 0, 255);
        }
        else if (buttonTypes[1, 0] == 2)
        {
            buttonTwo.material.color = new Color32(200, 200, 0, 255);
        }
        else
        {
            buttonTwo.material.color = new Color32(0, 0, 255, 255);
        }

        for (int i = 0; i < 2; i++)
        {
            if (buttonTypes[2, 0] == buttonTypes[0, 0] || buttonTypes[2, 0] == buttonTypes[1, 0])
            {
                buttonTypes[2, 0] = (buttonTypes[2, 0] + 1) % 4;
            }
        }

        if (buttonTypes[2, 0] == 0)
        {
            buttonThree.material.color = new Color32(255, 0, 0, 255);
        }
        else if (buttonTypes[2, 0] == 1)
        {
            buttonThree.material.color = new Color32(0, 255, 0, 255);
        }
        else if (buttonTypes[2, 0] == 2)
        {
            buttonThree.material.color = new Color32(200, 200, 0, 255);
        }
        else
        {
            buttonThree.material.color = new Color32(0, 0, 255, 255);
        }

        for (int i = 0; i < 3; i++)
        {
            if (buttonTypes[3, 0] == buttonTypes[0, 0] || buttonTypes[3, 0] == buttonTypes[1, 0] || buttonTypes[3, 0] == buttonTypes[2, 0])
            {
                buttonTypes[3, 0] = (buttonTypes[3, 0] + 1) % 4;
            }
        }

        if (buttonTypes[3, 0] == 0)
        {
            buttonFour.material.color = new Color32(255, 0, 0, 255);
        }
        else if (buttonTypes[3, 0] == 1)
        {
            buttonFour.material.color = new Color32(0, 255, 0, 255);
        }
        else if (buttonTypes[3, 0] == 2)
        {
            buttonFour.material.color = new Color32(200, 200, 0, 255);
        }
        else
        {
            buttonFour.material.color = new Color32(0, 0, 255, 255);
        }

        Debug.LogFormat("[Instructions #{0}] The buttons, in reading order, are {1}, {2}, {3}, and {4}.",
            _moduleID, buttonPossibilities[buttonTypes[0, 0]], buttonPossibilities[buttonTypes[1, 0]], buttonPossibilities[buttonTypes[2, 0]], buttonPossibilities[buttonTypes[3, 0]]);

        // Button label generation

        buttonTypes[0, 1] = Random.Range(0, 4);

        label1.text = labels[buttonTypes[0, 1]];

        buttonTypes[1, 1] = Random.Range(0, 4);

        for (int i = 0; i < 4; i++)
        {
            if (buttonTypes[1, 1] == buttonTypes[0, 1])
            {
                if (buttonTypes[1, 1] == 3)
                {
                    buttonTypes[1, 1] = 0;
                }
                else
                {
                    buttonTypes[1, 1]++;
                }
            }
        }

        label2.text = labels[buttonTypes[1, 1]];

        buttonTypes[2, 1] = Random.Range(0, 4);

        for (int i = 0; i < 4; i++)
        {
            if (buttonTypes[2, 1] == buttonTypes[0, 1] || buttonTypes[2, 1] == buttonTypes[1, 1])
            {
                if (buttonTypes[2, 1] == 3)
                {
                    buttonTypes[2, 1] = 0;
                }
                else
                {
                    buttonTypes[2, 1]++;
                }
            }
        }

        label3.text = labels[buttonTypes[2, 1]];

        buttonTypes[3, 1] = Random.Range(0, 4);

        for (int i = 0; i < 4; i++)
        {
            if (buttonTypes[3, 1] == buttonTypes[0, 1] || buttonTypes[3, 1] == buttonTypes[1, 1] || buttonTypes[3, 1] == buttonTypes[2, 1])
            {
                if (buttonTypes[3, 1] == 3)
                {
                    buttonTypes[3, 1] = 0;
                }
                else
                {
                    buttonTypes[3, 1]++;
                }
            }
        }

        label4.text = labels[buttonTypes[3, 1]];

        Debug.LogFormat("[Instructions #{0}] The button labels, in reading order, are {1}, {2}, {3}, and {4}.",
            _moduleID, labels[buttonTypes[0, 1]], labels[buttonTypes[1, 1]], labels[buttonTypes[2, 1]], labels[buttonTypes[3, 1]]);
    }

    void GenerateEdgework()
    {
        // Edgework gets added to the edgework array

        edgework[0] = Info.GetBatteryCount();
        edgework[1] = Info.GetBatteryHolderCount();
        edgework[2] = Info.GetIndicators().Count();
        edgework[3] = Info.GetOnIndicators().Count();
        edgework[4] = Info.GetOffIndicators().Count();
        edgework[5] = Info.GetPortCount();
        edgework[6] = Info.GetPortPlateCount();
        edgework[7] = Info.GetSerialNumberNumbers().Count();
        edgework[8] = Info.GetSerialNumberLetters().Count();
        edgework[9] = Info.GetModuleNames().Count;
        edgework[10] = Info.GetTwoFactorCounts();
        edgework[11] = Info.GetSolvedModuleNames().Count();
        edgework[12] = Info.CountUniquePorts();
        edgework[13] = Info.GetStrikes();
    }
}
