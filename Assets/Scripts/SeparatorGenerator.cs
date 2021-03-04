using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeparatorGenerator : MonoBehaviour {

    public Sprite[] sprites;

	// Use this for initialization
	void Start () {

        for(int i = 0; i < transform.childCount; i++)
        {
            Image imageN = transform.GetChild(i).GetComponent<Image>();
            imageN.sprite = sprites[Random.Range(0, sprites.Length)];
            imageN.gameObject.transform.Rotate(new Vector3(0, 0, Random.Range(0, 3) * 90f));
        }
        
        
	}
}
