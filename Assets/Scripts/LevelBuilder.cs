using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Position
{
    public int xPos, yPos; // Relative position of blocks on the grid made by the tiles

    public Position()
    {
        xPos = 0;
        yPos = 0;
    }

    public Position(int x,int y)
    {
        xPos = x;
        yPos = y;
    }
}

public class Node
{
    public Position pos;
    public List<Position> neighbors;

    public Node(Position p)
    {
        pos = p;
        neighbors = new List<Position>();
    }
}

public class LevelBuilder : MonoBehaviour {

    /*[Header("Image Set")]
    public GameObject imageSet;*/

    public void BuildLevel(GameObject level, GameObject images) {
         imageSet = images;

        // Tile sprites initialization
        int tilesNum = imageSet.transform.Find("Tiles").childCount;
        tilesSprites = new Sprite[tilesNum];
        for(int i = 0; i < tilesNum; i++)
        {
            tilesSprites[i] = imageSet.transform.Find("Tiles").GetChild(i).GetComponent<SpriteRenderer>().sprite;
        }

        // Block sprites initialization
        int blocksNum = imageSet.transform.Find("Blocks").childCount;
        blocksSprites = new Sprite[blocksNum];
        for (int i = 0; i < blocksNum; i++)
        {
            blocksSprites[i] = imageSet.transform.Find("Blocks").GetChild(i).GetComponent<SpriteRenderer>().sprite;
        }
        // Setting the block sprites for the UI screen separator
        SeparatorGenerator sep = FindObjectOfType<SeparatorGenerator>();
        if(sep) sep.sprites = blocksSprites;

        // Tile size initialization from the first tile sprite
        tileSize = tilesSprites[0].bounds.size.x;


        levelWidth = level.GetComponent<Level>().width;
        levelHeight = level.GetComponent<Level>().height;
        blockPositions = level.GetComponent<Level>().blocksPositions.ToArray();
        pickupPositions = level.GetComponent<Level>().pickUpPositions.ToArray();
        playerSpawnPositions = level.GetComponent<Level>().ufoPositions;

        // Start building level
        if ((levelWidth > 0) && (levelHeight > 0))
        {
            // Painting the level with tiles
            for (int i = -(levelWidth+1); i < (levelWidth + 2); i++) // Extended to (width and height) +1 to have tiles under the border blocks
            {
                for (int j = -(levelHeight+1); j < (levelHeight + 2); j++)
                {
                    Vector3 vect = new Vector3(tileSize * i, tileSize * j, 0);
                    CreateTile(vect);
                }
            }
        
        
            // Adding obstacles at borders
            for(int i = -(levelWidth+1); i< (levelWidth + 2); i++)
            {
                CreateBlock(i, (levelHeight + 1), "BorderBlock");
                CreateBlock(i, -(levelHeight + 1), "BorderBlock");
            }
            for(int j = -levelHeight; j < (levelHeight + 1); j++)
            {
                CreateBlock((levelWidth+1), j, "BorderBlock");
                CreateBlock(-(levelWidth + 1), j, "BorderBlock");
            }

		

            // Adding obstacles at "blocks" position
            for(int i = 0; i < blockPositions.Length; i++)
            {

                CreateBlock(blockPositions[i].xPos, blockPositions[i].yPos, "Block");
            }

            GameManager gameM = GetComponent<GameManager>();
            // Creating pickups spawn points for GameManager
            Transform[] spawnPoints = new Transform[pickupPositions.Length];
            for (int i = 0; i < pickupPositions.Length; i++)
            {
                GameObject go = new GameObject("PickUpSpawnPoint" + i);
                go .transform.position = new Vector3(tileSize * pickupPositions[i].xPos, tileSize * pickupPositions[i].yPos, 0f);
                go.transform.parent = gameObject.transform;
                spawnPoints[i] = go.transform;
            }
            if(gameM) gameM.spawnPoints = spawnPoints;

            // Creating UFO spawn points for GameManager
            for (int i = 0; i < 4; i++)
            {
                GameObject go = new GameObject("Player" + (i + 1) + "SpawnPoint");
                go.transform.position = new Vector3(tileSize * playerSpawnPositions[i].xPos, tileSize * playerSpawnPositions[i].yPos, 0f);
                go.transform.parent = gameObject.transform;
                if (gameM) gameM.ufoSpawns[i] = go.transform;
            }

            // Creating the graph for pathfinding
            SetGraph();
        }
        else
        {
            Debug.LogError("Level dimensions must be positive and greater than 0!");
        }
    }

    private void CreateTile(Vector3 pos)
    {
        GameObject go = new GameObject("Tile");
        go.AddComponent<SpriteRenderer>();
        go.GetComponent<SpriteRenderer>().sprite = tilesSprites[Random.Range(0, tilesSprites.Length)];
        float f = Random.Range(0.9f,1f);
        go.GetComponent<SpriteRenderer>().color = new Color(f, f, f);
        go.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        go.transform.position = pos;
        go.transform.parent = gameObject.transform;
    }

    private void CreateBlock(int x, int y,string name)
    {
        GameObject go = new GameObject(name);
        go.transform.position = new Vector3(tileSize * x, tileSize * y, 0f);
        go.transform.Rotate(0f, 0f, 90 * Random.Range(0, 3));
        go.AddComponent<BoxCollider2D>();
        go.GetComponent<BoxCollider2D>().size = new Vector2(tileSize, tileSize);
        go.AddComponent<SpriteRenderer>();
        go.GetComponent<SpriteRenderer>().sprite = blocksSprites[Random.Range(0, blocksSprites.Length)];
        go.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        go.GetComponent<SpriteRenderer>().sortingOrder = 1;
        go.transform.parent = gameObject.transform;
        go.layer = LayerMask.NameToLayer("Blocks");
    }

    private List<Position> MakeAllNodes()
    {
        bool isBlock = false;
        List<Position> allNodes = new List<Position>();
        for(int w = -levelWidth; w <= levelWidth; w++)
        {
            for(int h = -levelHeight; h <= levelHeight; h++)
            {
                isBlock = false;
                for(int i = 0; i < blockPositions.Length; i++)
                {
                    if (blockPositions[i].xPos == w && blockPositions[i].yPos == h) isBlock = true;
                }
                if (!isBlock) allNodes.Add(new Position(w, h));
            }
        }
        return allNodes;
    }

    private List<Node> MakeGraph(List<Position> allNodes)
    {
        mask = LayerMask.GetMask("Blocks");
        List<Node> graph = new List<Node>();
        foreach (Position p in allNodes)
        {
            //Debug.Log("Node position is [" + p.xPos + "," + p.yPos + "]");
            Node n = new Node(p);
            /*for (int i = -1; i <= 1; i++) // all neighbors even the diagonale
            {
                for (int j = -1; j <= 1; j++)
                {
                    if ((i != 0 || j != 0))
                    {
                        foreach (Position k in allNodes)
                        {
                            if ((k.xPos == (p.xPos + i)) && (k.yPos == (p.yPos + j)))
                            {
                                Vector3 direction = new Vector3(p.xPos * tileSize, p.yPos * tileSize, 0) - new Vector3(k.xPos*tileSize,k.yPos*tileSize,0);
                                hit = Physics2D.Raycast(new Vector2(k.xPos*tileSize, k.yPos*tileSize), new Vector2(direction.x, direction.y), direction.magnitude, mask, 0f, 0f);
                                if (!hit)
                                {
                                    n.neighbors.Add(new Position(p.xPos + i, p.yPos + j));
                                    //Debug.Log("[" + k.xPos + "," + k.yPos + "] is neighbor of [" + p.xPos + "," + p.yPos + "]");
                                }
                            }
                        }
                    }
                }
            }*/

            // Test with only the four direction and not the diagonale
            foreach (Position k in allNodes)
            {
                if ((k.xPos == (p.xPos + 1)) && (k.yPos == (p.yPos)))
                {
                    n.neighbors.Add(new Position(p.xPos + 1, p.yPos));
                }
            }

            foreach (Position k in allNodes)
            {
                if ((k.xPos == (p.xPos - 1)) && (k.yPos == (p.yPos)))
                {
                    n.neighbors.Add(new Position(p.xPos - 1, p.yPos));
                }
            }

            foreach (Position k in allNodes)
            {
                if ((k.xPos == (p.xPos)) && (k.yPos == (p.yPos + 1)))
                {
                    n.neighbors.Add(new Position(p.xPos, p.yPos + 1));
                }
            }

            foreach (Position k in allNodes)
            {
                if ((k.xPos == (p.xPos)) && (k.yPos == (p.yPos - 1)))
                {
                    n.neighbors.Add(new Position(p.xPos, p.yPos - 1));
                }
            }


            graph.Add(n);
        }
        return graph;
    }

    private void SetGraph()
    {
        levelGraph = MakeGraph(MakeAllNodes());
    }

    public List<Node> GetGraph()
    {
        return levelGraph;
    }

    

    private float tileSize;
    private Sprite[] tilesSprites;
    private Sprite[] blocksSprites;
    private GameObject imageSet;
    private int levelWidth;
    private int levelHeight;
    private Position[] blockPositions;
    private Position[] pickupPositions;
    private Position[] playerSpawnPositions;
    private List<Node> levelGraph;
    private RaycastHit2D hit;
    private LayerMask mask;
}
