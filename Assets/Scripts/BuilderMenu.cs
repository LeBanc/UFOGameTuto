using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;

public class BuilderMenu : MonoBehaviour {

    public InputField levelNameIF;
    public Text widthText;
    public Text heightText;
    public Canvas addActionsCanvas;
    public Canvas objectActionsCanvas;
    public Canvas ufoSpawnActionCanvas;
    public Canvas warningCanvas;
    public Canvas replaceFileCanvas;
    public Canvas loadCanvas;
    public GameObject fileToLoadItem;
    public Button[] uiButtons;

    // Use this for initialization
    void Start () {
        tileWidth = FindObjectOfType<DataManager>().GetSelectedImageSet().transform.Find("Tiles").GetChild(0).GetComponent<SpriteRenderer>().size.x;
        tileHeigth = FindObjectOfType<DataManager>().GetSelectedImageSet().transform.Find("Tiles").GetChild(0).GetComponent<SpriteRenderer>().size.y;
        UpdatePreview();

        // Submenu initialization
        addPlayerSpawnCanvas = addActionsCanvas.transform.Find("SetUFOSpawnButton").Find("AddPlayerSpawnCanvas").GetComponent<Canvas>();

        // Boolean to allow new mouse click (set to false after a click and then true with a timer)
        allowMouseClick = true;

        eventSys = FindObjectOfType<EventSystem>();
    }

    public void UpdatePreview()
    {
        FindObjectOfType<Level>().DefineWidth(width);
        FindObjectOfType<Level>().DefineHeigth(height);
        FindObjectOfType<PreviewLevelBuilder>().CleanLevel();
        FindObjectOfType<PreviewLevelBuilder>().BuildLevel(FindObjectOfType<Level>().gameObject, FindObjectOfType<DataManager>().GetSelectedImageSet());
        FindObjectOfType<PreviewLevelBuilder>().AddPreviewColliders();
        FindObjectOfType<PreviewLevelBuilder>().AddPreviewSprites();

        float xSize = 1+(width) * tileWidth;
        float ySize = (1+height) * tileHeigth;
        FindObjectOfType<Camera>().orthographicSize = Mathf.Max(xSize, ySize);

        // Hide all the popup menu
        addActionsCanvas.enabled = false;
        objectActionsCanvas.enabled = false;
        ufoSpawnActionCanvas.enabled = false;
    }

    private void Update()
    {
        // Escape key is used to disabled any pop-up menu on screen or move action
        if (Input.GetKeyDown("escape"))
        {
            addActionsCanvas.enabled = false;
            objectActionsCanvas.enabled = false;
            ufoSpawnActionCanvas.enabled = false;
            isDrawingLine = false;
            WarningOKAction();
            CancelLoad();
        }

        if (Input.GetButton("Mouse Left Click") && allowMouseClick)
        {
            // Launch the coroutine to set the delay to allow a new click
            StartCoroutine(AllowMouseClickDelay());

            // Test to avoid changing the pop-up menu if the mouse is already over a pop-up menu
            pointer = new PointerEventData(eventSys);
            pointer.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            addActionsCanvas.GetComponent<GraphicRaycaster>().Raycast(pointer, results);
            if(results.Count!=0) return;
            addPlayerSpawnCanvas.GetComponent<GraphicRaycaster>().Raycast(pointer, results);
            if (results.Count != 0) return;
            objectActionsCanvas.GetComponent<GraphicRaycaster>().Raycast(pointer, results);
            if (results.Count != 0) return;
            ufoSpawnActionCanvas.GetComponent<GraphicRaycaster>().Raycast(pointer, results);
            if (results.Count != 0) return;

            // Do nothing if a pop-up window is displayed
            if (warningCanvas.enabled) return;
            if (replaceFileCanvas.enabled) return;
            if (loadCanvas.enabled) return;


            // Else any action below will need the cursor position so Set the cursor position in game grid reference
            cursorPosition.xPos = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x / tileWidth);
            cursorPosition.yPos = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y / tileHeigth);

            


            // If the move action is running
            if (isDrawingLine)
            {
                // Test if any object is already on the selected spot and do something if not
                hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero);
                if (!hit)
                {
                    // If the cursor position is in the level dimension, move object
                    if (cursorPosition.xPos <= width && cursorPosition.xPos >= -width && cursorPosition.yPos <= height && cursorPosition.yPos >= -height)
                    {
                        MoveObjectAtPos(selectedObject, cursorPosition);
                    }
                    // In any case, stop drawing line
                    isDrawingLine = false;
                    
                }                    
                return;
            }

            if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0) // to check if the mouse click is in the Camera Area
            {
                // First disable all the active menus
                addActionsCanvas.enabled = false;
                objectActionsCanvas.enabled = false;
                ufoSpawnActionCanvas.enabled = false;

                // If the cursor position is greater than the level dimension, do nothing
                if (cursorPosition.xPos > width || cursorPosition.xPos < -width || cursorPosition.yPos > height || cursorPosition.yPos < -height) return;

                hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero);
                if (hit)
                {
                    // An object is selected
                    selectedObject = hit.collider.gameObject;

                    // If the object is a block or a pickup spawn point
                    // Define the position of the remove menu to fit in the screen and prompt it
                    if (selectedObject.name.StartsWith("PickUpSpawnPoint") || (selectedObject.name == "Block"))
                    {
                        // First define the position of the Remove menu
                        objectActionsCanvas.transform.position = new Vector2(Mathf.Clamp(Input.mousePosition.x, 0, Screen.width - objectActionsCanvas.GetComponent<RectTransform>().rect.width), Mathf.Clamp(Input.mousePosition.y, objectActionsCanvas.GetComponent<RectTransform>().rect.height, Screen.height));

                        // Then set the name of the object in the menu
                        objectActionsCanvas.transform.Find("ObjectName").GetComponentInChildren<Text>().text = selectedObject.name + " ";

                        // And prompt the Remove menu
                        objectActionsCanvas.enabled = true;
                    }


                    //If the object is a UFO spawn point
                    if (selectedObject.name.StartsWith("Player"))
                    {
                        // First define the position of the ufoMenu menu
                        ufoSpawnActionCanvas.transform.position = new Vector2(Mathf.Clamp(Input.mousePosition.x, 0, Screen.width - ufoSpawnActionCanvas.GetComponent<RectTransform>().rect.width), Mathf.Clamp(Input.mousePosition.y, ufoSpawnActionCanvas.GetComponent<RectTransform>().rect.height, Screen.height));

                        // Then set the name of the object in the menu
                        ufoSpawnActionCanvas.transform.Find("UFOSpawnName").GetComponentInChildren<Text>().text = selectedObject.name + " ";

                        // And prompt the ufoMenu menu
                        ufoSpawnActionCanvas.enabled = true;
                    }
                }
                else
                {
                    // No object selected
                    // First define the position of the Add menu and its submenu to fit in the screen
                    addActionsCanvas.transform.position = new Vector2(Mathf.Clamp(Input.mousePosition.x, 0, Screen.width - addActionsCanvas.GetComponent<RectTransform>().rect.width), Mathf.Clamp(Input.mousePosition.y, addActionsCanvas.GetComponent<RectTransform>().rect.height, Screen.height));
                    if(addActionsCanvas.transform.position.x>Screen.width-(addActionsCanvas.GetComponent<RectTransform>().rect.width+ addPlayerSpawnCanvas.GetComponent<RectTransform>().rect.width))
                    {
                        addPlayerSpawnCanvas.transform.localPosition = new Vector3(-addPlayerSpawnCanvas.GetComponent<RectTransform>().rect.width, addPlayerSpawnCanvas.transform.localPosition.y, addPlayerSpawnCanvas.transform.localPosition.z);
                    }
                    else
                    {
                        addPlayerSpawnCanvas.transform.localPosition = new Vector3(addPlayerSpawnCanvas.GetComponent<RectTransform>().rect.width, addPlayerSpawnCanvas.transform.localPosition.y, addPlayerSpawnCanvas.transform.localPosition.z);
                    }
                    if (addActionsCanvas.transform.position.y < (addActionsCanvas.GetComponent<RectTransform>().rect.height + addPlayerSpawnCanvas.GetComponent<RectTransform>().rect.height))
                    {
                        addPlayerSpawnCanvas.transform.localPosition = new Vector3(addPlayerSpawnCanvas.transform.localPosition.x, -addPlayerSpawnCanvas.GetComponent<RectTransform>().rect.height/4, addPlayerSpawnCanvas.transform.localPosition.z);
                    }
                    else
                    {
                        addPlayerSpawnCanvas.transform.localPosition = new Vector3(addPlayerSpawnCanvas.transform.localPosition.x, -addPlayerSpawnCanvas.GetComponent<RectTransform>().rect.height, addPlayerSpawnCanvas.transform.localPosition.z);
                    }
                    // Then prompt the Add menu and hide the submenu
                    addActionsCanvas.enabled = true;
                    addPlayerSpawnCanvas.enabled = false;
                }
            }
            else
            {
                // If the click isn't in the camera viewport
                // Hide all the popup menu
                addActionsCanvas.enabled = false;
                objectActionsCanvas.enabled = false;
                ufoSpawnActionCanvas.enabled = false;
            }
        }
    }

    IEnumerator AllowMouseClickDelay()
    {
        allowMouseClick = false;
        yield return new WaitForSeconds(0.08f);
        allowMouseClick = true;
    }

    void DisplayWidth()
    {
        widthText.text = width.ToString("00");
    }

    public void AddWidth()
    {
        if (width < 10) width++;
        DisplayWidth();
        UpdatePreview();
    }

    public void RemoveWidth()
    {
        if (width > 1) width--;
        DisplayWidth();
        UpdatePreview();
    }

    void DisplayHeight()
    {
        heightText.text = height.ToString("00");
    }

    public void AddHeight()
    {
        if (height < 10) height++;
        DisplayHeight();
        UpdatePreview();
    }

    public void RemoveHeight()
    {
        if (height > 1) height--;
        DisplayHeight();
        UpdatePreview();
    }

    public void SetUpUFOSpawnCanvasEnabler()
    {
        addPlayerSpawnCanvas.enabled = !addPlayerSpawnCanvas.enabled;
    }

    public void AddBlockAtPos()
    {
        // List is object-oriented so it has to copy a new object "position" else any modification to cursorPosition would be apply to all instances in the list
        Position tempPos = new Position();
        tempPos.xPos = cursorPosition.xPos;
        tempPos.yPos = cursorPosition.yPos;
        FindObjectOfType<Level>().AddBlock(tempPos);
        UpdatePreview();
    }

    public void AddPickupAtPos()
    {
        // List is object-oriented so it has to copy a new object "position" else any modification to cursorPosition would be apply to all instances in the list
        Position tempPos = new Position();
        tempPos.xPos = cursorPosition.xPos;
        tempPos.yPos = cursorPosition.yPos;
        FindObjectOfType<Level>().AddPickup(tempPos);
        UpdatePreview();
    }

    public void SetUFOAtPos(int i)
    {
        // List is object-oriented so it has to copy a new object "position" else any modification to cursorPosition would be apply to all instances in the list
        Position tempPos = new Position();
        tempPos.xPos = cursorPosition.xPos;
        tempPos.yPos = cursorPosition.yPos;
        FindObjectOfType<Level>().SetUFOPosition(i, tempPos);
        UpdatePreview();
    }

    public void RemoveObjectAtPos()
    {
        // List is object-oriented so it has to copy a new object "position" else any modification to cursorPosition would be apply to all instances in the list
        Position tempPos = new Position();
        tempPos.xPos = cursorPosition.xPos;
        tempPos.yPos = cursorPosition.yPos;

        if (selectedObject.name.StartsWith("PickUpSpawnPoint"))
        {
            FindObjectOfType<Level>().RemovePickup(tempPos);
        }
        if ((hit.collider.gameObject.name == "Block"))
        {
            FindObjectOfType<Level>().RemoveBlock(tempPos);
        }
        UpdatePreview();
    }

    public void MoveObjectGraphics()
    {
        // Hide the menu
        ufoSpawnActionCanvas.enabled = false;
        objectActionsCanvas.enabled = false;
        // Draw a line from cursorPos to mousePosition
        isDrawingLine = true;
        StartCoroutine(DrawLine());
    }

    IEnumerator DrawLine()
    {
        LineRenderer lr = gameObject.GetComponent<LineRenderer>();
        lr.SetPosition(0,new Vector3(cursorPosition.xPos* tileWidth, cursorPosition.yPos*tileHeigth,-5));
        lr.SetPosition(1,new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
        while (isDrawingLine)
        {
            lr.SetPosition(1, new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
            yield return null;
        }
        lr.SetPosition(0, Vector3.zero);
        lr.SetPosition(1, Vector3.zero);
    }

    public void MoveObjectAtPos(GameObject obj, Position pos)
    {
        Position tempPos = new Position();
        tempPos.xPos = Mathf.FloorToInt(obj.transform.position.x / tileWidth);
        tempPos.yPos = Mathf.FloorToInt(obj.transform.position.y / tileHeigth);
        switch (obj.name)
        {
            case "Player1SpawnPoint":
                SetUFOAtPos(0);
                break;
            case "Player2SpawnPoint":
                SetUFOAtPos(1);
                break;
            case "Player3SpawnPoint":
                SetUFOAtPos(2);
                break;
            case "Player4SpawnPoint":
                SetUFOAtPos(3);
                break;
            case "Block":
                FindObjectOfType<Level>().RemoveBlock(tempPos);
                AddBlockAtPos();
                break;
            default:
                if (obj.name.StartsWith("PickUpSpawnPoint"))
                {
                    FindObjectOfType<Level>().RemovePickup(tempPos);
                    AddPickupAtPos();
                }
                break;
        }
        UpdatePreview();
    }

    public void SaveLevel()
    {
        //Debug.Log(Application.persistentDataPath);
        string levelName = FindObjectOfType<Level>().gameObject.name = levelNameIF.text;
        if (levelName.Equals("")) {
            // Display the pop-up warning and disable the interaction on other UI opject
            warningCanvas.transform.Find("WarningText").GetComponent<Text>().text = "Warning!\nYou must put a level name to save the level.\n(The level was not saved)";
            warningCanvas.enabled = true;
            levelNameIF.interactable = false;
            foreach (Button button in uiButtons)
            {
                button.interactable = false;
            }
            return;
        }

        string tempPath = Path.Combine(Application.persistentDataPath, "Levels");

        if (File.Exists(Path.Combine(tempPath, levelNameIF.text + ".xml")))
        {
            StartCoroutine(WaitingToSave());
            return;
        } else
        {
            
            if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
            LevelXml xml = new LevelXml();
            xml.SaveToXml(FindObjectOfType<Level>(), Path.Combine(tempPath, levelNameIF.text + ".xml"));
        }
    }

    IEnumerator WaitingToSave()
    {
        // Display the pop-up and disable the interaction on other UI objects
        replaceFileCanvas.enabled = true;
        levelNameIF.interactable = false;
        foreach (Button button in uiButtons){
            button.interactable = false;
        }
        
        replaceFileToSave = false; // default, just to be sure

        // Wait for user answer
        while (replaceFileCanvas.enabled)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        // Allow the interaction on other UY objects after the pop-up was hidden
        levelNameIF.interactable = true;
        foreach (Button button in uiButtons)
        {
            button.interactable = true;
        }
        // Save level if replaceFileToSave is true
        if (replaceFileToSave)
        {
            string tempPath = Path.Combine(Application.persistentDataPath, "Levels");
            LevelXml xml = new LevelXml();
            xml.SaveToXml(FindObjectOfType<Level>(), Path.Combine(tempPath, levelNameIF.text + ".xml"));
        }

    }

    public void AnswerReplaceFile(bool answer)
    {
        replaceFileToSave = answer; // select the user answer
        replaceFileCanvas.enabled = false; // hide the pop-up

    }

    public void WarningOKAction()
    {
        // Hide the Warning pop-up and allow the interaction on other UI objects
        warningCanvas.enabled = false;
        levelNameIF.interactable = true;
        foreach (Button button in uiButtons)
        {
            button.interactable = true;
        }
    }

    public void DisplayLoadLevelMenu()
    {
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Levels"))) Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Levels"));

        int numFile = 0;
        foreach (string fileInDir in Directory.GetFiles(Path.Combine(Application.persistentDataPath, "Levels")))
        {
            if (fileInDir.EndsWith(".xml"))
            {
                GameObject tempButton = Instantiate(fileToLoadItem);
                tempButton.transform.SetParent(loadCanvas.transform.Find("Scroll View").Find("Viewport").Find("Content"));
                //tempButton.transform.parent = loadCanvas.transform.Find("Scroll View").Find("Viewport").Find("Content");
                tempButton.GetComponent<RectTransform>().offsetMax = new Vector2(0f, - 20f * numFile);
                tempButton.GetComponent<RectTransform>().offsetMin = new Vector2(0f, -20 - 20f * numFile);
                tempButton.GetComponentInChildren<Text>().text = Path.GetFileNameWithoutExtension(fileInDir);
                numFile++;
                loadCanvas.transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponent<RectTransform>().offsetMin = new Vector2(0, -20f * numFile);
            }
        }

        // Display the Load Pop-up Menu and disable the interaction on other UI objects
        loadCanvas.enabled = true;
        levelNameIF.interactable = false;
        foreach (Button button in uiButtons)
        {
            button.interactable = false;
        }
     }

    public void SetFileToLoad(string fileName)
    {
        fileToLoad = fileName;
    }

    public void CancelLoad()
    {
        // Hide the menu and enable interaction on other UI objects
        loadCanvas.enabled = false;
        levelNameIF.interactable = true;
        foreach (Button button in uiButtons)
        {
            button.interactable = true;
        }
    }

    public void LoadSelectedLevel()
    {
        string dirPath = Path.Combine(Application.persistentDataPath, "Levels");
        string file = Path.Combine(dirPath, fileToLoad + ".xml");

        if (File.Exists(file))
        {
            LevelXml xml = new LevelXml();
            xml.LoadFromXml(file, FindObjectOfType<Level>());
            width = FindObjectOfType<Level>().width;
            DisplayWidth();
            height = FindObjectOfType<Level>().height;
            DisplayHeight();
            levelNameIF.text = FindObjectOfType<Level>().gameObject.name;

            // Hide the menu and enable interaction on other UI objects
            loadCanvas.enabled = false;
            levelNameIF.interactable = true;
            foreach (Button button in uiButtons)
            {
                button.interactable = true;
            }
            UpdatePreview();
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private int width = 1;
    private int height = 1;
    private float tileWidth;
    private float tileHeigth;
    private RaycastHit2D hit;
    private Canvas addPlayerSpawnCanvas;
    private GraphicRaycaster addMenuCanvasRaycaster;
    private GraphicRaycaster setUFOSpawnCanvasRaycaster;
    private GraphicRaycaster objectMenuCanvasRayscaster;
    private GraphicRaycaster ufoSpawnMenuCanvasRaycaster;
    private EventSystem eventSys;
    private PointerEventData pointer;
    private Position cursorPosition = new Position();
    private GameObject selectedObject;
    private bool isDrawingLine;
    private bool allowMouseClick;
    private bool waitingToSave;
    private bool replaceFileToSave;
    private bool waitingWarning;
    private string fileToLoad;
}
