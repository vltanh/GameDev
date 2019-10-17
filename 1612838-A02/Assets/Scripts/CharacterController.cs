using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public KeyCode moveLeft = KeyCode.LeftArrow,
                moveRight = KeyCode.RightArrow,
                moveUp = KeyCode.UpArrow,
                moveDown = KeyCode.DownArrow,
                attack = KeyCode.Space;

    public float xVelocity = 5.0f,
                yVelocity = 300.0f;

    Animator anim;
    Rigidbody2D rigidBody;
    GameObject leg;

    int jumpCount = 0;
    int maxJumpTimes = 2;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        leg = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(moveRight))
        {
            anim.Play("CharacterWalkFront");
            rigidBody.AddForce(new Vector2(xVelocity, 0.0f));
        }

        if (Input.GetKey(moveLeft))
        {
            anim.Play("CharacterWalkBack");
            rigidBody.AddForce(new Vector2(-xVelocity, 0.0f));
        }

        if (Input.GetKey(moveDown))
        {
            anim.Play("CharacterSit");
        }

        if (Input.GetKeyDown(moveUp))
        {
            if (jumpCount < maxJumpTimes)
            {
                anim.Play("CharacterJump");
                rigidBody.AddForce(new Vector2(0.0f, yVelocity));
                jumpCount += 1;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D myCollider = collision.contacts[0].otherCollider;
        Collider2D otherCollider = collision.contacts[0].collider;
        if (otherCollider.tag == "Ground" && myCollider.name == "CharacterLeg")
        {
            jumpCount = 0;
        }
    }
}
