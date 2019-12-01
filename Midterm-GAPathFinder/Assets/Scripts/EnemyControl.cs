using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public float maxLinear;
    public float maxAngular;
    public int padding;
    public int size;

    public GameObject enemyMedium;
    public GameObject enemySmall;

    public int point;
    private GameObject player;

    Rigidbody2D rigidBody;

    public GameObject explosion;

    public GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.AddForce(new Vector2(Random.Range(-maxLinear, maxLinear), Random.Range(-maxLinear, maxLinear)));
        rigidBody.AddTorque(Random.Range(-maxAngular, maxAngular));

        player = GameObject.FindWithTag("Player");
        gm = GameObject.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        ScreenWrap();
    }

    void ScreenWrap()
    {
        Vector3 newPos = Camera.main.WorldToScreenPoint(transform.position);
        if (newPos.x < -padding)
        {
            newPos.x = Screen.width;
        }
        else if (newPos.x > Screen.width + padding)
        {
            newPos.x = 0;
        }
        if (newPos.y < -padding)
        {
            newPos.y = Screen.height;
        }
        else if (newPos.y > Screen.height + padding)
        {
            newPos.y = 0;
        }
        transform.position = Camera.main.ScreenToWorldPoint(newPos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet")
        {
            Destroy(collision.gameObject);

            if (size == 3)
            {
                Instantiate(enemyMedium, transform.position, transform.rotation);
                Instantiate(enemyMedium, transform.position, transform.rotation);
                gm.UpdateNumberOfEnemies(1);
            }
            else if (size == 2)
            {
                Instantiate(enemySmall, transform.position, transform.rotation);
                Instantiate(enemySmall, transform.position, transform.rotation);
                gm.UpdateNumberOfEnemies(1);
            }
            else if (size == 1)
            {
                gm.UpdateNumberOfEnemies(-1);
            }

            player.SendMessage("IncreaseScore", point);

            GameObject ex = Instantiate(explosion, transform.position, transform.rotation);
            Destroy(ex, 1.0f);

            Destroy(gameObject);
        }
    }
}
