using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoving : MonoBehaviour
{
    public float xVelocity = 2.0f, 
                    yVelocity = 2.0f;

    Animator anim;
    Vector2 xHat, yHat;
    Vector2 position;
    int direction = 1;

    Bullet bullet;

    // Start is called before the first frame update
    void Start()
    {
        bullet = FindObjectOfType<Bullet>();
        anim = GetComponent<Animator>();
        position = transform.position;
        xHat = new Vector2(xVelocity, 0.0f);
        yHat = new Vector2(0.0f, yVelocity);
    }

    // Update is called once per frame
    void Update()
    {
        position.x = position.x + direction * xVelocity * Time.deltaTime;
        transform.position = position;
        if (Random.Range(0.0f, 1.0f) < 0.01)
        {
            Debug.Log("Hi");
            anim.SetTrigger("attack");

            Vector3 spawnPosition = transform.position;
            spawnPosition.y -= 0.5f;
            Instantiate(bullet, spawnPosition, transform.rotation);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Right Boundary" || collision.gameObject.name == "Left Boundary")
        {
            direction = -direction;
        }
    }
}
