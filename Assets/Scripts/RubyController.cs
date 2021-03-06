using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    public int health { get { return currentHealth; } }
    public int maxHealth = 5;
    int currentHealth;

    bool isInvincible;
    float invincibleTimer;
    public float timeInvincible = 2.0f;

    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    public GameObject projectilePrefab;

    public RaycastHit2D raycast;

    AudioSource audioSource;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip backgroundMusic;
    public AudioClip winMusic;
    public AudioClip loseMusic;

    public GameObject healthInceasePrefab;
    public GameObject healthDecreasePrefab;

    public int score;
    public Text scoreText;
    public Text winText;
    public Text cogText;

    public bool gameOver;

    public static int level = 1;

    public int cogs;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();

        score = 0;
        SetScoreText();
        winText.text = " ";

        gameOver = false;

        cogs = 4;
        SetCogText();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (cogs >= 1)
            {
                Launch();
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                if (score == 4)
                {
                    SceneManager.LoadScene("StageTwo");
                    level = 2;
                }
                else if (score < 4)
                {
                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character != null)
                    {
                        character.DisplayDialog();
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if (health == 0)
        {
            gameOver = true;
            speed = 0;
            winText.text = "You lose! Press R to restart.";
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            GameObject projectileObject = Instantiate(healthDecreasePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            animator.SetTrigger("Hit");
            PlaySound(hitSound);
        }

        if (amount >= 1)
        {
            GameObject projectileObject = Instantiate(healthInceasePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if (health == 0)
        {
            audioSource.Stop();
            audioSource.clip = loseMusic;
            audioSource.Play();
        }
    }

    public void ChangeScore(int amount)
    {
        score = score + amount;
        SetScoreText();
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        PlaySound(throwSound);

        cogs = cogs - 1;
        SetCogText();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Cogs")
        {
            cogs = cogs + 3;
            Destroy(collision.collider.gameObject);
            SetCogText();
        }
    }

    void SetScoreText()
    {
        scoreText.text = "Fixed Robots: " + score.ToString();
        if (score == 4)
        {
            if (level == 1)
            {
                winText.text = "Talk to Jambi to visit stage two!";
            }

            if (level == 2)
            {
                gameOver = true;
                winText.text = "You Win! Game created by Mike Rodriguez. Press R to restart.";
                audioSource.Stop();
                audioSource.clip = winMusic;
                audioSource.Play();
            }
        }
    }

    void SetCogText()
    {
        cogText.text = "Cogs: " + cogs.ToString();
    }
}
