using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpaceshipControl : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    public Vector2 direction;
    public float speed;
    public float shootDelay;
    public float bulletSpeed;
    public Transform player;
    public GameObject bullet;
    public int point;
    public GameManager gm;
    public GameObject explosion;
    private float lastShotTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        gm = GameObject.FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (Time.time > lastShotTime + shootDelay)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

            GameObject tmpBullet = Instantiate(bullet, transform.position, q);
            tmpBullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0f, bulletSpeed));
            Destroy(tmpBullet, 10.0f);

            lastShotTime = Time.time;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        direction = (player.position - transform.position).normalized;
        rigidBody.MovePosition(rigidBody.position + direction * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet")
        {
            Destroy(collision.gameObject);
            gm.UpdateNumberOfEnemies(-1);
            player.SendMessage("IncreaseScore", point);
            GameObject ex = Instantiate(explosion, transform.position, transform.rotation);
            Destroy(ex, 1.0f);
            Destroy(gameObject);
        }
    }
}
