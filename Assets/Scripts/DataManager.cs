using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls
{
    public string xAxis = "";
    public string yAxis = "";
}

public class DataManager : MonoBehaviour {

    private static DataManager _instanceData; // to check on Awake if an instance is already existing because we only wanted one instance

    public static bool ufo1IsPlayer;
    public static bool ufo2IsPlayer;
    public static bool ufo3IsPlayer;
    public static bool ufo4IsPlayer;
    public static int player1ControlsNumber;
    public static int player2ControlsNumber;
    public static int player3ControlsNumber;
    public static int player4ControlsNumber;
    public static Controls player1Controls;
    public static Controls player2Controls;
    public static Controls player3Controls;
    public static Controls player4Controls;
    public static GameObject selectedImageSet;
    public static int selectedImageSetNumber;
    public static int levelToLoad;
    public static int pointsToWin;

    public GameObject[] imageSets;
    public GameObject[] levels;

    void Awake()
    {
        if (_instanceData != null && _instanceData != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instanceData = this;
        DontDestroyOnLoad(this.gameObject);

        // Default values : first Tileset, first level, 10 points to win, all UFO are AI (setting controls is obsolete)
        SetIsPlayer(false, false, false, false);
        SetPlayersControlsNumber(0, 1, 2, 3);
        SetPointsToWin(10);
        SetSelectedImageSetNumber(0);
        SetLevelToLoad(0);
    }

    private void OnDestroy()
    {
        if (_instanceData != null && _instanceData != this) return;
    }

    public void SetIsPlayer(bool p1,bool p2, bool p3, bool p4)
    {
        ufo1IsPlayer = p1;
        ufo2IsPlayer = p2;
        ufo3IsPlayer = p3;
        ufo4IsPlayer = p4;
    }
    public bool GetIsPlayer(int n)
    {
        switch (n)
        {
            case 1:
                return ufo1IsPlayer;
            case 2:
                return ufo2IsPlayer;
            case 3:
                return ufo3IsPlayer;
            case 4:
                return ufo4IsPlayer;
            default:
                return ufo1IsPlayer;
        }
    }

    Controls SetControls(int i)
    {
        Controls ctrl = new Controls();
        switch (i)
        {
            case 0:
                ctrl.xAxis = "HorizontalP1";
                ctrl.yAxis = "VerticalP1";
                break;
            case 1:
                ctrl.xAxis = "HorizontalP2";
                ctrl.yAxis = "VerticalP2";
                break;
            case 2:
                ctrl.xAxis = "HorizontalP3";
                ctrl.yAxis = "VerticalP3";
                break;
            case 3:
                ctrl.xAxis = "HorizontalP4";
                ctrl.yAxis = "VerticalP4";
                break;
            case 4:
                ctrl.xAxis = "HorizontalController";
                ctrl.yAxis = "VerticalController";
                break;
            default:
                ctrl.xAxis = "HorizontalP1";
                ctrl.yAxis = "VerticalP1";
                break;
        }
        return ctrl;
    }

    void SetPlayersControls(int p1, int p2, int p3, int p4)
    {
        if (ufo1IsPlayer) player1Controls = SetControls(p1);
        if (ufo2IsPlayer) player2Controls = SetControls(p2);
        if (ufo3IsPlayer) player3Controls = SetControls(p3);
        if (ufo4IsPlayer) player4Controls = SetControls(p4);
    }

    public Controls GetPlayerControls(int n)
    {
        switch (n)
        {
            case 1:
                return player1Controls;
            case 2:
                return player2Controls;
            case 3:
                return player3Controls;
            case 4:
                return player4Controls;
            default:
                return player1Controls;
        }
    }

    public void SetPlayersControlsNumber(int p1, int p2, int p3, int p4)
    {
        player1ControlsNumber = p1;
        player2ControlsNumber = p2;
        player3ControlsNumber = p3;
        player4ControlsNumber = p4;
        SetPlayersControls(p1, p2, p3, p4);
    }

    public int[] GetPlayerControlsNumber()
    {
        int[] pcn = { player1ControlsNumber, player2ControlsNumber, player3ControlsNumber, player4ControlsNumber };
        return pcn;
    }

    public void SetPointsToWin(int p)
    {
        pointsToWin = p;
    }

    public int GetPointsToWin()
    {
        return pointsToWin;
    }

    public GameObject GetSelectedImageSet()
    {
        return imageSets[selectedImageSetNumber];
    }

    public void SetSelectedImageSetNumber(int i)
    {
        selectedImageSetNumber = i;
    }

    public void SetLevelToLoad(int num)
    {
        levelToLoad = num;
    }

    public GameObject GetSelectedLevel()
    {
        return levels[levelToLoad];
    }

    public void AddLevel(GameObject level)
    {
        GameObject[] tempArray = new GameObject[levels.Length + 1];
        for (int i = 0; i < levels.Length; i++)
        {
            tempArray[i] = levels[i];
        }
        tempArray[levels.Length] = level;
        levels = tempArray;

    }
}
