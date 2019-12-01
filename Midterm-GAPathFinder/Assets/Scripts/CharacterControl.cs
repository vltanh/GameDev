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
    private int lives = 2;

    public Text scoreText;
    public Text livesText;
    public GameObject gameOverPanel;

    public GameObject explosion;
    public AudioSource hitAudio;

    bool isDead = false;

    public Color invulnerableColor;
    public Color normalColor;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        scoreText.text = "Score: " + score;
        livesText.text = "Lives: " + lives;

        Respawn();
    }

    // Update is called once per frame
    void Update()
    {
        linearInput = Input.GetAxis("Vertical");
        angularInput = Input.GetAxis("Horizontal");

        // Fire
        if (!isDead && Input.GetButtonDown("Fire1"))
        {
            GameObject newBullet = Instantiate(bullet, transform.position, transform.rotation);
            newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
            Destroy(newBullet, 3.0f);
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
        rigidBody.velocity = Vector2.zero;
        transform.position = Vector2.zero;
        transform.rotation = Quaternion.identity;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.enabled = true;
        sr.color = invulnerableColor;

        Invoke("Invulnerable", 5.0f);
    }

    void Invulnerable()
    {
        isDead = false;
        GetComponent<SpriteRenderer>().color = normalColor;
        GetComponent<Collider2D>().enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDead && collision.relativeVelocity.magnitude > deathForce)
        {
            lives -= 1;
            livesText.text = "Lives: " + lives;

            hitAudio.Play();
            GameObject ex = Instantiate(explosion, transform.position, transform.rotation);
            Destroy(ex, 1.0f);

            isDead = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            Invoke("Respawn", 1.0f);

            if (lives <= 0)
            {
                GameOver();
            }
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

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }
}