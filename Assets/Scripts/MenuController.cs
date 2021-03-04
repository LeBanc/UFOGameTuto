using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    //From Menu canvas
    public Text levelText;
    public Image tilesetImage;
    public Text pointsToWinText;
    public Dropdown player1DD;
    public Canvas player1ControlSelection;
    public Image player1ControlsImage;
    public Dropdown player2DD;
    public Canvas player2ControlSelection;
    public Image player2ControlsImage;
    public Dropdown player3DD;
    public Canvas player3ControlSelection;
    public Image player3ControlsImage;
    public Dropdown player4DD;
    public Canvas player4ControlSelection;
    public Image player4ControlsImage;
    public Canvas toDoCanvas;

    //From prefabs
    public Sprite[] controlsImages;

    // Use this for initialization
    void Start() {
        DataManager dataMgmt = FindObjectOfType<DataManager>();

        //Initialize level selection
        maxLevelNumber = dataMgmt.levels.Length;
        selectedLevel = 0;
        DisplayLevel();

        //Initialize tileset selection from DataManager
        maxTilesetNumber = dataMgmt.imageSets.Length - 1;
        selectedTileset = 0;
        DisplayTileset();

        //Initialize points to win selection from DataManager
        pointsToWin = dataMgmt.GetPointsToWin();
        DisplayPointsToWin();

        //Initialize players type selection from DataManager
        if (dataMgmt.GetIsPlayer(1)) { player1DD.value = 0; } else { player1DD.value = 1; }
        if (dataMgmt.GetIsPlayer(2)) { player2DD.value = 0; } else { player2DD.value = 1; }
        if (dataMgmt.GetIsPlayer(3)) { player3DD.value = 0; } else { player3DD.value = 1; }
        if (dataMgmt.GetIsPlayer(4)) { player4DD.value = 0; } else { player4DD.value = 1; }
        ChangePlayer1AI();
        ChangePlayer2AI();
        ChangePlayer3AI();
        ChangePlayer4AI();

        //Initialize players controls selection
        maxControls = controlsImages.Length;
        int[] playerControls = dataMgmt.GetPlayerControlsNumber();
        player1Controls = playerControls[0];
        player2Controls = playerControls[1];
        player3Controls = playerControls[2];
        player4Controls = playerControls[3];
        DisplayPlayer1Controls();
        DisplayPlayer2Controls();
        DisplayPlayer3Controls();
        DisplayPlayer4Controls();

    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
    }

    void DisplayLevel()
    {
        levelText.text = FindObjectOfType<DataManager>().levels[selectedLevel].name;
    }

    public void NextLevel()
    {
        selectedLevel++;
        if (selectedLevel >= maxLevelNumber) selectedLevel = 0;
        DisplayLevel();
    }

    public void PreviousLevel()
    {
        selectedLevel--;
        if (selectedLevel < 0) selectedLevel = maxLevelNumber-1;
        DisplayLevel();
    }

    void DisplayTileset()
    {
        tilesetImage.sprite = FindObjectOfType<DataManager>().imageSets[selectedTileset].transform.Find("Tiles").GetChild(0).GetComponent<SpriteRenderer>().sprite;
    }

    public void NextTileset()
    {
        selectedTileset++;
        if (selectedTileset > maxTilesetNumber) selectedTileset = 0;
        DisplayTileset();
    }

    public void PreviousTileset()
    {
        selectedTileset--;
        if (selectedTileset < 0) selectedTileset = maxTilesetNumber;
        DisplayTileset();
    }

    void DisplayPointsToWin()
    {
        pointsToWinText.text = pointsToWin.ToString("00");
    }

    public void NextPointsToWin()
    {
        pointsToWin++;
        if (pointsToWin > 99) pointsToWin = 99;
        DisplayPointsToWin();
    }

    public void PreviousPointsToWin()
    {
        pointsToWin--;
        if (pointsToWin < 1) pointsToWin = 1;
        DisplayPointsToWin();
    }

    public void ChangePlayer1AI()
    {
        switch (player1DD.value)
        {
            case 0: //Player1 is a player
                player1ControlSelection.enabled = true;
                ufo1IsPlayer = true;
                break;
            case 1: //Player1 is an AI
                player1ControlSelection.enabled = false;
                ufo1IsPlayer = false;
                break;
            default:
                player1ControlSelection.enabled = true;
                ufo1IsPlayer = true;
                break;
        }
    }

    public void ChangePlayer2AI()
    {
        switch (player2DD.value)
        {
            case 0: //Player2 is a player
                player2ControlSelection.enabled = true;
                ufo2IsPlayer = true;
                break;
            case 1: //Player2 is an AI
                player2ControlSelection.enabled = false;
                ufo2IsPlayer = false;
                break;
            default:
                player2ControlSelection.enabled = true;
                ufo2IsPlayer = true;
                break;
        }
    }

    public void ChangePlayer3AI()
    {
        switch (player3DD.value)
        {
            case 0: //Player3 is a player
                player3ControlSelection.enabled = true;
                ufo3IsPlayer = true;
                break;
            case 1: //Player3 is an AI
                player3ControlSelection.enabled = false;
                ufo3IsPlayer = false;
                break;
            default:
                player3ControlSelection.enabled = true;
                ufo3IsPlayer = true;
                break;
        }
    }

    public void ChangePlayer4AI()
    {
        switch (player4DD.value)
        {
            case 0: //Player4 is a player
                player4ControlSelection.enabled = true;
                ufo4IsPlayer = true;
                break;
            case 1: //Player4 is an AI
                player4ControlSelection.enabled = false;
                ufo4IsPlayer = false;
                break;
            default:
                player4ControlSelection.enabled = true;
                ufo4IsPlayer = true;
                break;
        }
    }

    void DisplayPlayer1Controls()
    {
        player1ControlsImage.sprite = controlsImages[player1Controls];
    }

    public void NextPlayer1Controls()
    {
        player1Controls++;
        if (player1Controls > maxControls-1) player1Controls = 0;
        DisplayPlayer1Controls();
    }

    public void PreviousPlayer1Controls()
    {
        player1Controls--;
        if (player1Controls < 0) player1Controls = maxControls-1;
        DisplayPlayer1Controls();
    }

    void DisplayPlayer2Controls()
    {
        player2ControlsImage.sprite = controlsImages[player2Controls];
    }

    public void NextPlayer2Controls()
    {
        player2Controls++;
        if (player2Controls > maxControls - 1) player2Controls = 0;
        DisplayPlayer2Controls();
    }

    public void PreviousPlayer2Controls()
    {
        player2Controls--;
        if (player2Controls < 0) player2Controls = maxControls - 1;
        DisplayPlayer2Controls();
    }

    void DisplayPlayer3Controls()
    {
        player3ControlsImage.sprite = controlsImages[player3Controls];
    }

    public void NextPlayer3Controls()
    {
        player3Controls++;
        if (player3Controls > maxControls - 1) player3Controls = 0;
        DisplayPlayer3Controls();
    }

    public void PreviousPlayer3Controls()
    {
        player3Controls--;
        if (player3Controls < 0) player3Controls = maxControls - 1;
        DisplayPlayer3Controls();
    }

    void DisplayPlayer4Controls()
    {
        player4ControlsImage.sprite = controlsImages[player4Controls];
    }

    public void NextPlayer4Controls()
    {
        player4Controls++;
        if (player4Controls > maxControls - 1) player4Controls = 0;
        DisplayPlayer4Controls();
    }

    public void PreviousPlayer4Controls()
    {
        player4Controls--;
        if (player4Controls < 0) player4Controls = maxControls - 1;
        DisplayPlayer4Controls();
    }

    public void StartGame()
    {
        DataManager dataMgmt = FindObjectOfType<DataManager>();
        dataMgmt.SetIsPlayer(ufo1IsPlayer, ufo2IsPlayer, ufo3IsPlayer, ufo4IsPlayer);
        dataMgmt.SetPlayersControlsNumber(player1Controls, player2Controls, player3Controls, player4Controls);
        dataMgmt.SetPointsToWin(pointsToWin);
        dataMgmt.SetSelectedImageSetNumber(selectedTileset);
        dataMgmt.SetLevelToLoad(selectedLevel);
        SceneManager.LoadScene("Main");
    }

    public void CreateLevel()
    {
        SceneManager.LoadScene("Builder");
    }

    public void LoadAllLevels()
    {
        string tempPath = Path.Combine(Application.persistentDataPath, "Levels");
        if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
        foreach (string file in Directory.GetFiles(tempPath))
        {
            if (file.EndsWith(".xml"))
            {
                GameObject newLevel = new GameObject();
                newLevel.AddComponent<Level>();
                LevelXml xml = new LevelXml();
                xml.LoadFromXml(Path.Combine(tempPath, file), newLevel.GetComponent<Level>());
                if (FindObjectOfType<DataManager>().transform.Find(newLevel.gameObject.name))
                {
                    Destroy(FindObjectOfType<DataManager>().transform.Find(newLevel.gameObject.name));
                }
                newLevel.transform.parent = FindObjectOfType<DataManager>().transform;
                FindObjectOfType<DataManager>().AddLevel(newLevel);
                maxLevelNumber = FindObjectOfType<DataManager>().levels.Length;
            }
        }
    }

    public void DisplayToDoPopup()
    {
        toDoCanvas.enabled = true;
    }

    public void HideToDoPopup()
    {
        toDoCanvas.enabled = false;
    }


    // Private variables
    private int selectedLevel;
    private int maxLevelNumber;

    private int selectedTileset;
    private int maxTilesetNumber;

    private int pointsToWin;

    private bool ufo1IsPlayer;
    private bool ufo2IsPlayer;
    private bool ufo3IsPlayer;
    private bool ufo4IsPlayer;

    private int maxControls;
    private int player1Controls;
    private int player2Controls;
    private int player3Controls;
    private int player4Controls;
}
