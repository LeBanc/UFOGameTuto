using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : UFOController {

    public string hAxisName="Horizontal";
    public string vAxisName = "Vertical";
    public float speed=2.0f;

    public override void Start()
    {
        base.Start();
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!GetReset())
        {
            float moveHorizontal = Input.GetAxis(hAxisName) * speed;
            float moveVertical = Input.GetAxis(vAxisName) * speed;

            rb2D.AddForce(new Vector2(moveHorizontal, moveVertical));
            rb2D.MoveRotation(rb2D.rotation + rotation);
        }
        else
        {
            rb2D.velocity = Vector3.zero;
        }
    }

    private Rigidbody2D rb2D;
}
