using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Header("UFOs")]
    public GameObject player;
    public GameObject enemy;
    public Transform[] ufoSpawns;

    [Header("UI")]
    public float camSize=7;

    [Header("Pick Ups")]
    public GameObject pickUpObject;
    public Transform[] spawnPoints;

    [Header("UI")]
    public Text scoreP1Text;
    public Text scoreP2Text;
    public Text scoreP3Text;
    public Text scoreP4Text;
    public Text gameOverTextP1;
    public Text gameOverTextP2;
    public Text gameOverTextP3;
    public Text gameOverTextP4;


    private void Start()
    {
        LevelBuilder builderEditor = FindObjectOfType<LevelBuilder>();
        if (builderEditor) builderEditor.BuildLevel(FindObjectOfType<DataManager>().GetSelectedLevel(), FindObjectOfType<DataManager>().GetSelectedImageSet());

        for (int i = 1; i< 5; i++)
        {
            CreatePlayer(i, FindObjectOfType<DataManager>().GetIsPlayer(i), FindObjectOfType<DataManager>().GetPlayerControls(i));
        }

        maxScore = FindObjectOfType<DataManager>().GetPointsToWin();

        DisplayScore(0, 1);
        DisplayScore(0, 2);
        DisplayScore(0, 3);
        DisplayScore(0, 4);

        SpawnPickUp();

    }

    void CreatePlayer(int n,bool isPlayer,Controls playerControls)
    {
        if (isPlayer)
        {
            GameObject p = Instantiate(player, ufoSpawns[n-1]);
            p.name = "Player" + (n);
            p.GetComponent<PlayerController>().SetUFONumber(n);
            CreateCamera(p, n-1);
            p.GetComponent<PlayerController>().hAxisName = playerControls.xAxis;
            p.GetComponent<PlayerController>().vAxisName = playerControls.yAxis;
        }
        else
        {
            GameObject e = Instantiate(enemy, ufoSpawns[n-1]);
            e.name = "Ennemy" + (n);
            e.GetComponent<EnemyController>().SetUFONumber(n);
            CreateCamera(e, n-1);
        }
    }

    void CreateCamera(GameObject target,int i)
    {
        GameObject go = new GameObject("Camera" + (i+1));
        go.transform.position.Set(ufoSpawns[i].position.x, ufoSpawns[i].position.y, -10);
        go.AddComponent<Camera>();
        Camera c = go.GetComponent<Camera>();
        c.orthographic = true;
        c.orthographicSize = camSize;
        c.depth = 0;
        c.backgroundColor = Color.black;
        switch (i+1)
        {
            case 1:
                c.rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
                break;
            case 2:
                c.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                break;
            case 3:
                c.rect = new Rect(0f, 0f, 0.5f, 0.5f);
                break;
            case 4:
                c.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                break;
            default:
                c.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                break;
        }
        c.gameObject.AddComponent<CameraController>();
        c.gameObject.GetComponent<CameraController>().SetTarget(target);
    }
    
    void SpawnPickUp()
    {
            GameObject pick = Instantiate(pickUpObject, spawnPoints[Random.Range(0, spawnPoints.Length)]);
            k++;
            pick.name = "PickUp" + k;
    }

    public void PickUpDestroyed()
    {
        if(!gameOver) SpawnPickUp();
    }
	
    public void DisplayScore(float score, int playerNumber)
    {
        switch (playerNumber)
        {
            case 1:
                scoreP1Text.text = "" + score;
                break;
            case 2:
                scoreP2Text.text = "" + score;
                break;
            case 3:
                scoreP3Text.text = "" + score;
                break;
            case 4:
                scoreP4Text.text = "" + score;
                break;
        }

        if (score >= maxScore)
        {
            gameOverTextP1.text = "You Loose!";
            gameOverTextP2.text = "You Loose!";
            gameOverTextP3.text = "You Loose!";
            gameOverTextP4.text = "You Loose!";

            switch (playerNumber)
            {
                case 1:
                    gameOverTextP1.text = "You Win!";
                    break;
                case 2:
                    gameOverTextP2.text = "You Win!";
                    break;
                case 3:
                    gameOverTextP3.text = "You Win!";
                    break;
                case 4:
                    gameOverTextP4.text = "You Win!";
                    break;
                default:
                    gameOverTextP1.text = "You Win!";
                    break;
            }
            GameOver();
        }
        
    }

    public void GameOver()
    {
        gameOverTextP1.GetComponent<Animator>().SetTrigger("GameOver");
        gameOverTextP2.GetComponent<Animator>().SetTrigger("GameOver");
        gameOverTextP3.GetComponent<Animator>().SetTrigger("GameOver");
        gameOverTextP4.GetComponent<Animator>().SetTrigger("GameOver");
        gameOver = true;
        StartCoroutine(BackToMainMenu());
    }

    IEnumerator BackToMainMenu()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Menu");
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    private bool gameOver = false;
    private int k = 0;
    private int maxScore;

}
