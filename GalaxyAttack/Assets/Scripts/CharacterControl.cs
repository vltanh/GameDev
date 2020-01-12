using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterControl : MonoBehaviour
{
    Rigidbody2D rigidBody;

    public float linearThrust;
    public float angularThrust;
    public float deathForce = 1.5f;

    private float linearInput;
    private float angularInput;

    public GameObject bullet;
    public float bulletForce;

    private int score = 0;
    private int lives = 3;

    public Text scoreText;
    public Text livesText;
    public GameObject gameOverPanel;

    public GameObject explosion;
    public AudioSource hitAudio;

    bool isDead = false;

    public Color invulnerableColor;
    public Color normalColor;

    public GameObject shield;

    public GameManager gm;

    public float timeBetweenShotMax = 2.0f;
    public float timeBetweenShotMin = 0.5f;
    private float currentTimeBetweenShot;
    private float lastShot;

    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        scoreText.text = "Score: " + score;
        livesText.text = "Lives: " + lives;

        Respawn();
        gm.StartLevel();
        lastShot = timeBetweenShotMax;
        currentTimeBetweenShot = timeBetweenShotMax;
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = (Time.time - lastShot) / currentTimeBetweenShot;

        linearInput = Input.GetAxis("Vertical");
        angularInput = Input.GetAxis("Horizontal");

        // Fire
        if (!isDead && gm.isShootable && Input.GetButtonDown("Fire1"))
        {
            if (Time.time > lastShot + currentTimeBetweenShot)
            {
                GameObject newBullet = Instantiate(bullet, transform.position, transform.rotation);
                newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
                Destroy(newBullet, 3.0f);
                lastShot = Time.time;
            }
        }

        // Screen warp
        Vector3 newPos = Camera.main.WorldToScreenPoint(transform.position);
        if (newPos.x < -20)
        {
            newPos.x = Screen.width;
        }
        else if (newPos.x > Screen.width + 20)
        {
            newPos.x = 0;
        }
        if (newPos.y < -20)
        {
            newPos.y = Screen.height;
        }
        else if (newPos.y > Screen.height + 20)
        {
            newPos.y = 0;
        }
        transform.position = Camera.main.ScreenToWorldPoint(newPos);
    }

    void FixedUpdate()
    {
        rigidBody.AddRelativeForce(Vector2.up * linearInput * linearThrust);
        rigidBody.AddTorque(-angularInput * angularThrust);
    }

    void Respawn()
    {
        currentTimeBetweenShot = timeBetweenShotMax;
        rigidBody.velocity = Vector2.zero;
        transform.position = Vector2.zero;
        transform.rotation = Quaternion.identity;

        GameObject tempShield = Instantiate(shield, transform.position, Quaternion.identity);
        Destroy(tempShield, 3.0f);

        Invoke("Vulnerable", 3.0f);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.enabled = true;
    }

    void Vulnerable()
    {
        isDead = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDead && collision.relativeVelocity.magnitude > deathForce)
        {
            GetHit();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDead) 
        {
            Debug.Log(collision.tag);
            if (collision.CompareTag("EnemyBullet"))
            {
                Destroy(collision.gameObject);
                GetHit();
            }
            else if (collision.CompareTag("PowerUp"))
            {
                Destroy(collision.gameObject);
                currentTimeBetweenShot = Mathf.Max(currentTimeBetweenShot - 0.1f, timeBetweenShotMin);
            }
        }
    }

    private void GetHit()
    {
        lives -= 1;
        livesText.text = "Lives: " + lives;

        hitAudio.Play();
        GameObject ex = Instantiate(explosion, transform.position, transform.rotation);
        Destroy(ex, 1.0f);

        isDead = true;
        GetComponent<SpriteRenderer>().enabled = false;
        Invoke("Respawn", 1.0f);

        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void IncreaseScore(int point)
    {
        score += point;
        scoreText.text = "Score: " + score;
    }

    void GameOver()
    {
        CancelInvoke();
        gameOverPanel.SetActive(true);
    }
}