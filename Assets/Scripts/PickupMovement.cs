using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupMovement : MonoBehaviour {

    public float angle = 2.0f;
    public float shakeAmount = 0.7f;

    // Use this for initialization
    void Start () {
        origin = transform.localPosition;
        StartCoroutine(Shake());
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(0f, 0f, angle)*Time.deltaTime);

        if (shake)
        {
            transform.localPosition = origin + Random.insideUnitSphere * shakeAmount;
        }
        else
        {
            transform.localPosition = origin;
        }
    }

    private Vector3 origin;
    private bool shake;

    IEnumerator Shake()
    {
        while (true)
        {
            shake = false;
            yield return new WaitForSeconds(5f);
            shake = true;
            yield return new WaitForSeconds(1f);
        }
    }

}
