using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReimuMoving : MonoBehaviour
{
    public KeyCode moveLeft = KeyCode.LeftArrow, 
                    moveRight = KeyCode.RightArrow, 
                    moveUp = KeyCode.UpArrow,
                    moveDown = KeyCode.DownArrow,
                    attack = KeyCode.Space;
    public float xVelocity = 5.0f, 
                    yVelocity = 5.0f;

    Animator anim;
    Vector2 position;

    Vector2 xHat, yHat;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        position = transform.position;
        xHat = new Vector2(xVelocity, 0.0f);
        yHat = new Vector2(0.0f, yVelocity);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(moveLeft) || Input.GetKeyUp(moveRight))
        {
            anim.Play("ReimuFlyingNormal");
        }

        if (Input.GetKey(moveLeft))
        {
            GetComponent<SpriteRenderer>().flipX = false;
            anim.Play("ReimuFlyingLeft");
            position -= xHat * Time.deltaTime;
        }
        else if (Input.GetKey(moveRight))
        {
            GetComponent<SpriteRenderer>().flipX = true;
            anim.Play("ReimuFlyingLeft");
            position += xHat * Time.deltaTime;
        }

        if (Input.GetKey(moveUp))
        {
            position += yHat * Time.deltaTime;
        }
        else if (Input.GetKey(moveDown))
        {
            position -= yHat * Time.deltaTime;
        }

        transform.position = position;
    }
}
