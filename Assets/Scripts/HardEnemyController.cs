using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardEnemyController : MonoBehaviour
{
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;

    public ParticleSystem smokeEffect;

    Animator animator;

    bool broken;

    private RubyController rubyController;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;

        animator = GetComponent<Animator>();
        broken = true;

        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController");

        if (rubyControllerObject != null)
        {
            rubyController = rubyControllerObject.GetComponent<RubyController>();
            print("Found the RubyController script!");
        }

        if (rubyControllerObject == null)
        {
            print("Cannot find the GameController script!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }

        if (!broken)
        {
            return;
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2D.position;

        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        rigidbody2D.MovePosition(position);

        if (!broken)
        {
            return;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-2);
        }
    }

    public void Fix()
    {
        broken = false;
        rigidbody2D.simulated = false;
        animator.SetTrigger("Fixed");
        rubyController.ChangeScore(1);

        smokeEffect.Stop();
    }
}