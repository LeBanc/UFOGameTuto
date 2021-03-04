using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewLevelBuilder : LevelBuilder {

    public Sprite player1Sprite;
    public Sprite player2Sprite;
    public Sprite player3Sprite;
    public Sprite player4Sprite;
    public Sprite pickupSprite;

    public void CleanLevel()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if((transform.GetChild(i).name == "Tile") || transform.GetChild(i).name.StartsWith("PickUpSpawnPoint") ||
                (transform.GetChild(i).name == "BorderBlock") || (transform.GetChild(i).name == "Block") || 
                (transform.GetChild(i).name == "Player1SpawnPoint") || (transform.GetChild(i).name == "Player2SpawnPoint") || 
                (transform.GetChild(i).name == "Player3SpawnPoint") || (transform.GetChild(i).name == "Player4SpawnPoint"))
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    public void AddPreviewColliders()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.StartsWith("PickUpSpawnPoint") ||
                (transform.GetChild(i).name == "Player1SpawnPoint") || (transform.GetChild(i).name == "Player2SpawnPoint") ||
                (transform.GetChild(i).name == "Player3SpawnPoint") || (transform.GetChild(i).name == "Player4SpawnPoint"))
            {
                transform.GetChild(i).gameObject.AddComponent<BoxCollider2D>();
                if (transform.GetChild(i).name.StartsWith("PickUpSpawnPoint"))
                {
                    transform.GetChild(i).gameObject.GetComponent<BoxCollider2D>().size = new Vector2(3.58f, 3.58f);
                }
                else
                {
                    transform.GetChild(i).gameObject.GetComponent<BoxCollider2D>().size = new Vector2(4.7f, 4.7f);
                }
                
            }
        }
    }

    public void AddPreviewSprites()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Player1SpawnPoint")
            {
                if(!transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>())transform.GetChild(i).gameObject.AddComponent<SpriteRenderer>();
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = player1Sprite;
                transform.GetChild(i).gameObject.transform.localScale = new Vector3(0.75f, 0.75f);
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
            if (transform.GetChild(i).name == "Player2SpawnPoint")
            {
                if (!transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>()) transform.GetChild(i).gameObject.AddComponent<SpriteRenderer>();
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = player2Sprite;
                transform.GetChild(i).gameObject.transform.localScale = new Vector3(0.75f, 0.75f);
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
            if (transform.GetChild(i).name == "Player3SpawnPoint")
            {
                if (!transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>()) transform.GetChild(i).gameObject.AddComponent<SpriteRenderer>();
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = player3Sprite;
                transform.GetChild(i).gameObject.transform.localScale = new Vector3(0.75f, 0.75f);
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
            if (transform.GetChild(i).name == "Player4SpawnPoint")
            {
                if (!transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>()) transform.GetChild(i).gameObject.AddComponent<SpriteRenderer>();
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = player4Sprite;
                transform.GetChild(i).gameObject.transform.localScale = new Vector3(0.75f, 0.75f);
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
            if (transform.GetChild(i).name.StartsWith("PickUpSpawnPoint"))
            {
                if (!transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>()) transform.GetChild(i).gameObject.AddComponent<SpriteRenderer>();
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = pickupSprite;
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Pickups";
                transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
                
            }
        }
    }

}
