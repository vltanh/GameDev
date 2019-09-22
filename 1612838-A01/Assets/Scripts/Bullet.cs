using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float xVelocity = 2.0f,
                yVelocity = 2.0f;
    Vector2 xHat, yHat;
    Vector2 position;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        xHat = new Vector2(xVelocity, 0.0f);
        yHat = new Vector2(0.0f, yVelocity);
    }

    // Update is called once per frame
    void Update()
    {
        position.y -= yVelocity * Time.deltaTime;
        transform.position = position;
    }
}
