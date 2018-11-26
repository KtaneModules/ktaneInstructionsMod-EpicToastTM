using UnityEngine;
using KMHelper;
using System.Collections;

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

    private int screen1, screen2, screen3, screen4, screen5, counter, buttonOneColor, buttonTwoColor, buttonThreeColor, buttonFourColor, screen2Position, screen4Position, screen5Position = 0;
    private string[] screens13 = { "BATTERIES", "BATTERY\nHOLDERS", "INDICATORS", "LIT INDICATORS", "UNLIT\nINDICATORS", "PORTS", "PORT PLATES", "DIGITS IN\nSERIAL NUMBER",
        "LETTERS IN\nSERIAL NUMBER", "MODULES" };
    private string[] screens245 = { "RED", "GREEN", "YELLOW", "BLUE", "A", "B", "C", "D", "FIRST", "SECOND", "THIRD", "FOURTH" };
    private string[] labels = { "A", "B", "C", "D" };
    private int randomLabel1, randomLabel2, randomLabel3, randomLabel4, correctBtn = 1;
    private int[] edgework = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };  
    private string buttonOneColorString, buttonTwoColorString, buttonThreeColorString, buttonFourColorString = "oof here's some placeholder text";
    private string[] startingScreens = { "WHO TURNED\nTHE LIGHTS OFF", "KAPOW KAPOW\nKAPOW KAPOW", "TEXT GOES\nHERE, I GUESS", "FUNNY JOKE\nHERE", "GG, YOU CAN\nREAD", "OH CRAP, THE TEXT\nGOES OFF THE SCREEN", "E", "OH SHOOT\nIT'S A BOMB", "HEY I'M YOUR\nFAVORITE MODULE,\nRIGHT?", "\n\n\n\n\n\n\n\n\n\n\n\nI'M DOWN HERE NOW", "STOP READING THE GITHUB" };
    private string[] solveMessages = { "GG", "NICE JOB", "MODULE\nDISARMED", ":D", "MODULE\nSOLVED", "*INSERT CLAP\nEMOJI HERE*", "WOOOOOOOO!", "OH CRAP YOU\nACTUALLY DID IT", "THAT WAS NICE\nOWO", "+6 POINTS!" };
    private string[] failureMessages = { "NOPE", "OOF", "+1 STRIKE!", "SMH", "JUST READ THE\nINSTRUCTIONS", "DEFINITELY NOT\nA BUG :)))))", "KABOOOOM!", ":(", "THIS IS SO SAD\nALEXA, PLAY\nDESPACITO", "BETTER CHECK\nTHE LOG!" };
    private int variableThatImTooLazyToNameProperlyDontWorryAboutItMan = 0;

    // Use this for initialization (and putting memes on the screen)
    void Start () {
        _moduleID = _moduleIDCounter++;
        Module.OnActivate += Activate;
        screen.text = startingScreens[Random.Range(0, 10)];
    }

    private void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            int j = i;
            buttons[i].OnInteract += delegate ()
            {
                ButtonPressed(j);
                return false;
            };
        }

        for (int i = 0; i < 5; i++)
        {
            int h = i;
            screenButtonSelectables[i].OnInteract += delegate ()
            {
                ScreenButtonPressed(h);
                return false;
            };
        }
    }
    void Activate()
    {
        Init();
        _lightsOn = true;
        
    }
    void Init()
    {
        // Makes the first screen light green

        screenButtons[0].material.color = new Color32(0, 255, 0, 255);
        screenButtons[1].material.color = new Color32(0, 0, 0, 255);
        screenButtons[2].material.color = new Color32(0, 0, 0, 255);
        screenButtons[3].material.color = new Color32(0, 0, 0, 255);
        screenButtons[4].material.color = new Color32(0, 0, 0, 255);

        // Screen generation

        screen1 = Random.Range(0, 10);

        screen2 = Random.Range(0, 12);

        screen3 = Random.Range(0, 10);

        // Prevents duplicates
        if (screen3 == screen1)
        {
            if (screen3 == 9)
            {
                screen3 = 1;
            }

            else
            {
                screen3++;
            }
        }

        screen4 = Random.Range(0, 12);

        if (screen2 == screen4)
        {
            if (screen4 == 11)
            {
                screen4 = 0;
            }

            else
            {
                screen4++;
            }
        }

        screen5 = Random.Range(0, 10);

        for (int i = 0; i < 4; i++)
        {
            if (screen2 == screen5 || screen4 == screen5)
            {
                if (screen5 == 11)
                {
                    screen5 = 0;
                }

                else
                {
                    screen5++;
                }
            }
        }

        // Puts the first word on the screen

        screen.text = screens13[screen1];

        Debug.LogFormat("[Instructions #{0}] The first screen says {1}.", _moduleID, screens13[screen1].Replace("\n", " "));
        Debug.LogFormat("[Instructions #{0}] The second screen says {1}.", _moduleID, screens245[screen2].Replace("\n", " "));
        Debug.LogFormat("[Instructions #{0}] The third screen says {1}.", _moduleID, screens13[screen3].Replace("\n", " "));
        Debug.LogFormat("[Instructions #{0}] The fourth screen says {1}.", _moduleID, screens245[screen4].Replace("\n", " "));
        Debug.LogFormat("[Instructions #{0}] The fifth screen says {1}.", _moduleID, screens245[screen5].Replace("\n", " "));

        // Edgework gets added to the edgework array

        edgework[0] = Info.GetBatteryCount();
        edgework[1] = Info.GetBatteryHolderCount();
        edgework[5] = Info.GetPortCount();
        edgework[6] = Info.GetPortPlateCount();
        edgework[9] = Info.GetModuleNames().Count;

        // Scripts to count indicators, letters and digits
        foreach (var x in Info.GetIndicators())
        {
            counter++;
        }
        edgework[2] = counter;

        counter = 0;
        foreach (var x in Info.GetOnIndicators())
        {
            counter++;
        }
        edgework[3] = counter;

        counter = 0;
        foreach (var x in Info.GetOffIndicators())
        {
            counter++;
        }
        edgework[4] = counter;

        counter = 0;
        foreach (var x in Info.GetSerialNumberNumbers())
        {
            counter++;
        }
        edgework[7] = counter;

        counter = 0;
        foreach (var x in Info.GetSerialNumberLetters())
        {
            counter++;
        }
        edgework[8] = counter;
        

        // For screens 1 and 3, possible words are: BATTERIES, BATTERY HOLDERS, INDICATORS, LIT INDICATORS, UNLIT INDICATORS, PORTS, PORT PLATES, DIGITS IN SERIAL NUMBER, LETTERS IN SERIAL NUMBER, MODULES.
        // For screens 2, 4, and 5, possible words are: RED, GREEN, YELLOW, BLUE, A, B, C, D, FIRST, SECOND, THIRD, FOURTH.
        
        // Button color generation

        buttonOneColor = Random.Range(0, 4);
        
        // 0 = Red, 1 = Green, 2 = Yellow, 3 = Blue

        if (buttonOneColor == 0)
        {
            buttonOne.material.color = new Color32(255, 0, 0, 255);
            buttonOneColorString = "red";
        }
        else if (buttonOneColor == 1)
        {
            buttonOne.material.color = new Color32(0, 255, 0, 255);
            buttonOneColorString = "green";
        }
        else if (buttonOneColor == 2)
        {
            buttonOne.material.color = new Color32(200, 200, 0, 255);
            buttonOneColorString = "yellow";
        }
        else
        {
            buttonOne.material.color = new Color32(0, 0, 255, 255);
            buttonOneColorString = "blue";
        }

        buttonTwoColor = Random.Range(0, 4);

        for (int i = 0; i < 4; i++)
        {
            if (buttonTwoColor == buttonOneColor)
            {
                if (buttonTwoColor == 3)
                {
                    buttonTwoColor = 0;
                }
                else
                {
                    buttonTwoColor++;
                }
            }
        }
    

        if (buttonTwoColor == 0)
        {
            buttonTwo.material.color = new Color32(255, 0, 0, 255);
            buttonTwoColorString = "red";
        }
        else if (buttonTwoColor == 1)
        {
            buttonTwo.material.color = new Color32(0, 255, 0, 255);
            buttonTwoColorString = "green";
        }
        else if (buttonTwoColor == 2)
        {
            buttonTwo.material.color = new Color32(200, 200, 0, 255);
            buttonTwoColorString = "yellow";
        }
        else
        {
            buttonTwo.material.color = new Color32(0, 0, 255, 255);
            buttonTwoColorString = "blue";
        }

        buttonThreeColor = Random.Range(0, 4);

        for (int i = 0; i< 4; i++)
        {
            if (buttonThreeColor == buttonOneColor || buttonThreeColor == buttonTwoColor)
            {
                if (buttonThreeColor == 3)
                {
                    buttonThreeColor = 0;
                }
                else
                {
                    buttonThreeColor++;
                }
            }
        }

        if (buttonThreeColor == 0)
        {
            buttonThree.material.color = new Color32(255, 0, 0, 255);
            buttonThreeColorString = "red";
        }
        else if (buttonThreeColor == 1)
        {
            buttonThree.material.color = new Color32(0, 255, 0, 255);
            buttonThreeColorString = "green";
        }
        else if (buttonThreeColor == 2)
        {
            buttonThree.material.color = new Color32(200, 200, 0, 255);
            buttonThreeColorString = "yellow";
        }
        else
        {
            buttonThree.material.color = new Color32(0, 0, 255, 255);
            buttonThreeColorString = "blue";
        }

        buttonFourColor = Random.Range(0, 4);

        for (int i = 0; i < 4; i++)
        {
            if (buttonFourColor == buttonOneColor || buttonFourColor == buttonTwoColor || buttonFourColor == buttonThreeColor)
            {
                if (buttonFourColor == 3)
                {
                    buttonFourColor = 0;
                }
                else
                {
                    buttonFourColor++;
                }
            }
        }

        if (buttonFourColor == 0)
        {
            buttonFour.material.color = new Color32(255, 0, 0, 255);
            buttonFourColorString = "red";
        }
        else if (buttonFourColor == 1)
        {
            buttonFour.material.color = new Color32(0, 255, 0, 255);
            buttonFourColorString = "green";
        }
        else if (buttonFourColor == 2)
        {
            buttonFour.material.color = new Color32(200, 200, 0, 255);
            buttonFourColorString = "yellow";
        }
        else
        {
            buttonFour.material.color = new Color32(0, 0, 255, 255);
            buttonFourColorString = "blue";
        }

        Debug.LogFormat("[Instructions #{0}] The buttons, in reading order, are {1}, {2}, {3}, and {4}.", 
            _moduleID, buttonOneColorString, buttonTwoColorString, buttonThreeColorString, buttonFourColorString);

        // Button label generation

        randomLabel1 = Random.Range(0, 4);

        label1.text = labels[randomLabel1];

        randomLabel2 = Random.Range(0, 4);

        for (int i = 0; i < 4; i++)
        {
            if (randomLabel2 == randomLabel1)
            {
                if (randomLabel2 == 3)
                {
                    randomLabel2 = 0;
                }
                else
                {
                    randomLabel2++;
                }
            }
        }

        label2.text = labels[randomLabel2];

        randomLabel3 = Random.Range(0, 4);

        for (int i = 0; i < 4; i++)
        {
            if (randomLabel3 == randomLabel1 || randomLabel3 == randomLabel2)
            {
                if (randomLabel3 == 3)
                {
                    randomLabel3 = 0;
                }
                else
                {
                    randomLabel3++;
                }
            }
        }

        label3.text = labels[randomLabel3];

        randomLabel4 = Random.Range(0, 4);

        for (int i = 0; i < 4; i++)
        {
            if (randomLabel4 == randomLabel1 || randomLabel4 == randomLabel2 || randomLabel4 == randomLabel3)
            {
                if (randomLabel4 == 3)
                {
                    randomLabel4 = 0;
                }
                else
                {
                    randomLabel4++;
                }
            }
        }

        label4.text = labels[randomLabel4];

        Debug.LogFormat("[Instructions #{0}] The button labels, in reading order, are {1}, {2}, {3}, and {4}.", 
            _moduleID, labels[randomLabel1], labels[randomLabel2], labels[randomLabel3], labels[randomLabel4]);
        
        // Calculate correct answer

        // Calculating what the positions for the buttons for screens 2, 4, and 5 are

        // If screen 2 is a color
        if (screen2 < 4)
        {
            if (buttonOneColor == screen2)
            {
                screen2Position = 1;
            }

            else if (buttonTwoColor == screen2)
            {
                screen2Position = 2;
            }

            else if (buttonThreeColor == screen2)
            {
                screen2Position = 3;
            }

            else
            {
                screen2Position = 4;
            }
        }

        // If screen 2 is a letter
        else if (screen2 > 3 && screen2 < 8)
        {
            if (randomLabel1 == screen2 - 4)
            {
                screen2Position = 1;
            }

            else if (randomLabel2 == screen2 - 4)
            {
                screen2Position = 2;
            }

            else if (randomLabel3 == screen2 - 4)
            {
                screen2Position = 3;
            }

            else
            {
                screen2Position = 4;
            }
        }

        // If screen 2 is a position
        else
        {
            screen2Position = screen2 - 7;
        }

        // If screen 4 is a color
        if (screen4 < 4)
        {
            if (buttonOneColor == screen4)
            {
                screen4Position = 1;
            }

            else if (buttonTwoColor == screen4)
            {
                screen4Position = 2;
            }

            else if (buttonThreeColor == screen4)
            {
                screen4Position = 3;
            }

            else
            {
                screen4Position = 4;
            }
        }

        // If screen 4 is a letter
        else if (screen4 > 3 && screen4 < 8)
        {
            if (randomLabel1 == screen4 - 4)
            {
                screen4Position = 1;
            }

            else if (randomLabel2 == screen4 - 4)
            {
                screen4Position = 2;
            }

            else if (randomLabel3 == screen4 - 4)
            {
                screen4Position = 3;
            }

            else
            {
                screen4Position = 4;
            }
        }

        // If screen 4 is a position
        else
        {
            screen4Position = screen4 - 7;
        }

        // If screen 5 is a color
        if (screen5 < 4)
        {
            if (buttonOneColor == screen5)
            {
                screen5Position = 1;
            }

            else if (buttonTwoColor == screen5)
            {
                screen5Position = 2;
            }

            else if (buttonThreeColor == screen5)
            {
                screen5Position = 3;
            }

            else
            {
                screen5Position = 4;
            }
        }

        // If screen 5 is a letter
        else if (screen5 > 3 && screen5 < 8)
        {
            if (randomLabel1 == screen5 - 4)
            {
                screen5Position = 1;
            }

            else if (randomLabel2 == screen5 - 4)
            {
                screen5Position = 2;
            }

            else if (randomLabel3 == screen5 - 4)
            {
                screen5Position = 3;
            }

            else
            {
                screen5Position = 4;
            }
        }

        // If screen 5 is a position
        else
        {
            screen5Position = screen5 - 7;
        }

        if (edgework[screen1] == 0)
        {
            if (screen5Position > screen2Position)
            {
                correctBtn = screen5Position;
            }

            else if (screen2Position > screen5Position)
            {
                correctBtn = screen2Position;
            }

            else
            {
                correctBtn = screen4Position;
            }
        }

        else if (edgework[screen1] < edgework[screen3])
        {
            correctBtn = screen2Position;
        }

        else if (screen2Position < screen4Position)
        {
            correctBtn = screen5Position;
        }

        else if (edgework[screen3] > 3)
        {
            correctBtn = (edgework[screen1] % 4) + 1;
        }

        else if (screen2Position != screen4Position)
        {
            if (screen2Position != screen4Position && screen4Position != screen5Position && screen2Position != screen5Position)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i != screen2Position && i != screen4Position && i != screen5Position)
                    {
                        correctBtn = i;
                    }
                }
            }
        }

        else
        {
            correctBtn = screen4Position;
        }

        Debug.LogFormat("[Instructions #{0}] The correct button to press is button {1}.", _moduleID, correctBtn);
        
}

    // Handling button presses
    // Buttons are numbered 0-3 in reading order, but gets changed to 1-4 in that if statement.

    void ButtonPressed(int btnNumber)
    {
        if (!_lightsOn || _solved) return;

        if (btnNumber + 1 == correctBtn)
        {
            _solved = true;

            Module.HandlePass();
            
            Audio.PlaySoundAtTransform("SolveSound", Module.transform);

            Debug.LogFormat("[Instructions #{0}] Button {1} was pressed. Module solved!!!!1!", _moduleID, btnNumber + 1);

            for (int i = 0; i < 5; i++)
            {
                screenButtons[i].material.color = new Color32(0, 255, 0, 255);
                screen.text = solveMessages[Random.Range(0,10)];
            }
        }

        else
        {
            Module.HandleStrike();

            Debug.LogFormat("[Instructions #{0}] Button {1} was pressed. Strike.", _moduleID, btnNumber + 1);

            StartCoroutine("test");
        }
    }

    IEnumerator test()
    {
        screen.text = failureMessages[Random.Range(0, 10)];

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

        if (btnNumber == 0)
        {
            screen.text = screens13[screen1];
        }

        else if (btnNumber == 1)
        {
            screen.text = screens245[screen2];
        }

        else if (btnNumber == 2)
        {
            screen.text = screens13[screen3];
        }

        else if (btnNumber == 3)
        {
            screen.text = screens245[screen4];
        }

        else
        {
            screen.text = screens245[screen5];
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
    
}
