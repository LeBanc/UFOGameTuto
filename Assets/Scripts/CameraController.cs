using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        /*PlayerController player = FindObjectOfType<PlayerController>();
        if (player)
        {
            objectToFollow = FindObjectOfType<PlayerController>().gameObject;
        } else
        {
            objectToFollow = FindObjectOfType<GameManager>().gameObject;
        }
        offset = transform.position - objectToFollow.transform.position;
        offset.x = 0;
        offset.y = 0;*/
    }
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = objectToFollow.transform.position + offset;
	}

    public void SetTarget(GameObject target)
    {
        objectToFollow = target;
        //offset = transform.position - objectToFollow.transform.position;
        offset.x = 0;
        offset.y = 0;
        offset.z = -10; // Had to add z offset specifically or else it was 0f;
    }

    private Vector3 offset;
    private GameObject objectToFollow;

}
