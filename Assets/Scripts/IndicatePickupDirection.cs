using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatePickupDirection : MonoBehaviour {
	
	// Update is called once per frame
	void LateUpdate () {
        ObjectController obj = FindObjectOfType<ObjectController>();
        if (obj)
        {
            //transform.LookAt(obj.gameObject.transform, new Vector3(1, 1, -1));
            transform.up = obj.transform.position - transform.position;
        } else
        {
            gameObject.SetActive(false);
        }
        
	}
}
