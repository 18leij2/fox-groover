using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossScript : Enemy
{
    private Collider2D coll;
    public static BossScript instance;
    [SerializeField] public int health = 100;
    private int lastHealth = 100;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject boss;

    [SerializeField] private LayerMask ground;
    [SerializeField] private Image healthBar;

    private enum State {idle, running, jumping}
    private State state = State.idle;

    protected override void Start()
    {
        base.Start();
        coll = GetComponent<Collider2D>();
        instance = this;
        StartCoroutine(BossAbility());
    }

    private void Update()
    {
        AnimationState();
        anim.SetInteger("state", (int)state);

        if (lastHealth != health)
        {
            updateHealthBar();
            lastHealth = health;
        }
    }

    private void updateHealthBar()
    {
        healthBar.fillAmount -= 0.05f;


        if (health <= 50 && health > 20)
        {
            healthBar.color = new Color(0.8f, 0.5f, 0.3f);
        }

        else if (health <= 20)
        {
            healthBar.color = new Color(0.8f, 0f, 0f);
        }
    }

    private IEnumerator BossAbility()
    {
        while (health > 0)
        {
            yield return new WaitForSeconds(2);
            int ability = Random.Range(1, 3);

            switch (ability)
            {
                case 1:
                    Debug.Log("Run towards player");
                    if (player.transform.position.x > boss.transform.position.x)
                    {
                        rb.velocity = new Vector2(15, rb.velocity.y);
                        transform.localScale = new Vector2(-1.635486f, 1.635486f);                      
                    }
                    else if (player.transform.position.x < boss.transform.position.x)
                    {
                        rb.velocity = new Vector2(-15, rb.velocity.y);
                        transform.localScale = new Vector2(1.635486f, 1.635486f);
                    }
                    break;

                case 2:
                    Debug.Log("Jump towards player");
                    if (player.transform.position.x > boss.transform.position.x)
                    {
                        rb.velocity = new Vector2(15, 10);
                        transform.localScale = new Vector2(-1.635486f, 1.635486f);
                    }
                    else if (player.transform.position.x < boss.transform.position.x)
                    {
                        rb.velocity = new Vector2(-15, 10);
                        transform.localScale = new Vector2(1.635486f, 1.635486f);
                    }
                    break;

                case 3:
                    Debug.Log("Run away from player");
                    if (player.transform.position.x > boss.transform.position.x)
                    {
                        rb.velocity = new Vector2(-15, rb.velocity.y);
                        transform.localScale = new Vector2(1.635486f, 1.635486f);
                    }
                    else if (player.transform.position.x < boss.transform.position.x)
                    {
                        rb.velocity = new Vector2(15, rb.velocity.y);
                        transform.localScale = new Vector2(-1.635486f, 1.635486f);
                    }
                    break;

                case 4:
                    Debug.Log("Jump away from player");
                    if (player.transform.position.x > boss.transform.position.x)
                    {
                        rb.velocity = new Vector2(-15, 10);
                        transform.localScale = new Vector2(1.635486f, 1.635486f);
                    }
                    else if (player.transform.position.x < boss.transform.position.x)
                    {
                        rb.velocity = new Vector2(15, 10);
                        transform.localScale = new Vector2(-1.635486f, 1.635486f);
                    }
                    break;

                case 5:
                    Debug.Log("Fire the missiles");
                    break;

                default:
                    Debug.Log("Do nothing");
                    break;
            }
        }     
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            state = State.idle;
        }
    }

    private void AnimationState()
    {
        if (Mathf.Abs(rb.velocity.y) > 1f)
        {
            state = State.jumping;
        }

        else if (state == State.jumping)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }

        else if (Mathf.Abs(rb.velocity.x) > 2f)
        {
            state = State.running;
        }

        else if (state == State.running)
        {
            if (Mathf.Abs(rb.velocity.x) > 2f)
            {
                state = State.idle;
            }
        }
    }
}
