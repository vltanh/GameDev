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

    public int maxJumpTimes = 2;
    public int maxHP = 100;
    public int maxMP = 100;

    int jumpCount = 0;
    int HP, MP;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        leg = transform.GetChild(0).gameObject;
        HP = maxHP;
        MP = maxMP;
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

    private bool IsCollidedGround(Collision2D collision)
    {
        Collider2D myCollider = collision.contacts[0].otherCollider;
        Collider2D otherCollider = collision.contacts[0].collider;
        return (otherCollider.tag == "Ground" && myCollider.name == "CharacterLeg");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsCollidedGround(collision))
        {
            jumpCount = 0;
        }
    }

    public float GetMP()
    {
        // return MP * 1.0f / maxMP;
        return 1.0f - jumpCount * 1.0f / maxJumpTimes;
    }

    public float GetHP()
    {
        return HP * 1.0f / maxMP;
    }
}
