using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public bool bossStart = false;

    //Start() variables
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;
    private PolygonCollider2D othercoll;
    [SerializeField] private bool debounce = false;

    //Finite State Machine
    private enum State { idle, running, jumping, falling, hurt, death, climb }
    private State state = State.idle;

    //Ladder variables
    [HideInInspector] public bool canClimb = false;
    [HideInInspector] public bool bottomLadder = false;
    [HideInInspector] public bool topLadder = false;
    public LadderController ladder;
    private float naturalGravity;
    [SerializeField] float climbSpeed = 3f;
    
    //Inspector variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 25f;
    [SerializeField] private TextMeshProUGUI gemText;
    [SerializeField] private TextMeshProUGUI cherryText;
    [SerializeField] private float hurtForce = 10f;
    [SerializeField] private AudioSource cherry;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private AudioSource jump;
    [SerializeField] private AudioSource gem;
    [SerializeField] private Sprite noHealth;
    [SerializeField] private Sprite someHealth;
    [SerializeField] private Sprite fullHealth;
    public Image HP1;
    public Image HP2;
    public Image HP3;
    [SerializeField] private string gameOver;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        othercoll = GetComponent<PolygonCollider2D>();
        naturalGravity = rb.gravityScale;

        cherryText.text = PermanentUI.perm.cherries.ToString();
        gemText.text = PermanentUI.perm.gems.ToString();

        if (PermanentUI.perm.health <= 0)
        {
            PermanentUI.perm.health = 6;
            HandleHealth(0);
        }
        else
        {
            HandleHealth(0);
        }
    }

    private void Update()
    {
        if (state == State.climb)
        {
            Climb();
        }

        else if(state != State.hurt && state != State.death)
        {
            Movement();
        }

        AnimationState();
        anim.SetInteger("state", (int)state); //Sets animation based on Enumerator state
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Collectible")
        {
            cherry.Play();
            Destroy(collision.gameObject);
            PermanentUI.perm.cherries += 1;
            cherryText.text = PermanentUI.perm.cherries.ToString();
        }

        if(collision.tag == "Powerup")
        {
            gem.Play();
            Destroy(collision.gameObject);
            PermanentUI.perm.gems += 1;
            gemText.text = PermanentUI.perm.gems.ToString();
            jumpForce = 50f;
            GetComponent<SpriteRenderer>().color = Color.yellow;
            StartCoroutine(ResetPower());
        }

        if(collision.tag == "BossDetect")
        {
            bossStart = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (collision.collider is PolygonCollider2D)//(state == State.falling)
            {
                enemy.JumpedOn();
                Jump();
            }

            else
            {
                state = State.hurt;
                HandleHealth(-1);

                if (collision.gameObject.transform.position.x > transform.position.x)
                {
                    //Enemy is to the right, therefore damaged and moved left
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }

                else
                {
                    //Enemy is to the left, therefore damaged and moved right
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }

            }
        }

        if (collision.gameObject.tag == "Boss")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (collision.collider is PolygonCollider2D)
            {
                print("efeifbib");
                enemy.BossedOn();
                Jump();
            }

            else
            {
                if (debounce == false)
                {
                    state = State.hurt;
                    HandleHealth(-1);

                    if (collision.gameObject.transform.position.x > transform.position.x)
                    {
                        //Enemy is to the right, therefore damaged and moved left
                        rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                    }

                    else
                    {
                        //Enemy is to the left, therefore damaged and moved right
                        rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                    }

                    debounce = true;
                    StartCoroutine(spriteBlink());
                    StartCoroutine(healthBuffer());                 
                }                
            }
        }

        if (collision.gameObject.tag == "Respawn")
        {
            HandleHealth(-6);
        }
    }

    private IEnumerator healthBuffer()
    {
        yield return new WaitForSeconds(2);
        debounce = false;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    private IEnumerator spriteBlink()
    {
        while (debounce == true)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
            yield return new WaitForSeconds(0.2f);
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void HandleHealth(int healthChange)
    {
        int caseSwitch;
        PermanentUI.perm.health += healthChange;

        caseSwitch = PermanentUI.perm.health;

        switch (caseSwitch)
        {
            case 6:
                HP3.GetComponent<Image>().sprite = fullHealth;
                HP2.GetComponent<Image>().sprite = fullHealth;
                HP1.GetComponent<Image>().sprite = fullHealth;
                break;

            case 5:
                HP3.GetComponent<Image>().sprite = someHealth;
                HP2.GetComponent<Image>().sprite = fullHealth;
                HP1.GetComponent<Image>().sprite = fullHealth;
                break;

            case 4:
                HP3.GetComponent<Image>().sprite = noHealth;
                HP2.GetComponent<Image>().sprite = fullHealth;
                HP1.GetComponent<Image>().sprite = fullHealth;
                break;

            case 3:
                HP3.GetComponent<Image>().sprite = noHealth;
                HP2.GetComponent<Image>().sprite = someHealth;
                HP1.GetComponent<Image>().sprite = fullHealth;
                break;

            case 2:
                HP3.GetComponent<Image>().sprite = noHealth;
                HP2.GetComponent<Image>().sprite = noHealth;
                HP1.GetComponent<Image>().sprite = fullHealth;
                break;

            case 1:
                HP3.GetComponent<Image>().sprite = noHealth;
                HP2.GetComponent<Image>().sprite = noHealth;
                HP1.GetComponent<Image>().sprite = someHealth;
                break;

            case 0:
                HP3.GetComponent<Image>().sprite = noHealth;
                HP2.GetComponent<Image>().sprite = noHealth;
                HP1.GetComponent<Image>().sprite = noHealth;
                break;

            default:
                HP3.GetComponent<Image>().sprite = noHealth;
                HP2.GetComponent<Image>().sprite = noHealth;
                HP1.GetComponent<Image>().sprite = noHealth;
                break;
        }

        if (PermanentUI.perm.health <= 0)
        {
            PermanentUI.perm.finalCherry = PermanentUI.perm.cherries;
            PermanentUI.perm.finalGem = PermanentUI.perm.gems;
            PermanentUI.perm.cherries = 0;
            PermanentUI.perm.gems = 0;
            state = State.death;
            StartCoroutine(DeathJump());
        }
    }

    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal");

        if (canClimb && Mathf.Abs(Input.GetAxis("Vertical")) > .1f)
        {
            state = State.climb;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            transform.position = new Vector3(ladder.transform.position.x, rb.position.y);
            rb.gravityScale = 0f;
        }

        //Moving left
        if (hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }

        //Moving right
        else if (hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
        }

        //Jumping
        if (Input.GetButtonDown("Jump"))
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.down, 1.3f, ground);
            //RaycastHit2D hit2 = Physics2D.Raycast(rb.position, Vector2.left, 1.3f, ground);
            //RaycastHit2D hit3 = Physics2D.Raycast(rb.position, Vector2.right, 1.3f, ground);

            if (hit.collider != null || othercoll.IsTouchingLayers(ground))
            {
                jump.Play();
                Jump();
            }                
        }
    }

    private void Climb()
    {
        if (Input.GetButtonDown("Jump"))
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            canClimb = false;
            rb.gravityScale = naturalGravity;
            anim.speed = 1f;
            jump.Play();
            Jump();
            return; 
        }

        float vDirection = Input.GetAxis("Vertical");
        
        //Climbing up
        if (vDirection > .1f && !topLadder)
        {
            rb.velocity = new Vector2(0f, vDirection * climbSpeed);
            anim.speed = 1f;
        }

        //Climbing down
        else if (vDirection < -.1f && !bottomLadder)
        {
            rb.velocity = new Vector2(0f, vDirection * climbSpeed);
            anim.speed = 1f;
        }

        else if (topLadder == true)
        {
            //Do something
        }

        else if (bottomLadder == true)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            canClimb = false;
            rb.gravityScale = naturalGravity;
            anim.speed = 1f;
            state = State.idle;
            return;
        }

        //Sitting still
        else
        {
            anim.speed = 0f;
            rb.velocity = Vector2.zero;
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        state = State.jumping;
    }

    private void AnimationState()
    {
        if (state == State.climb)
        {

        }

        else if(state == State.jumping)
        {
            if(rb.velocity.y < .1f)
            {
                state = State.falling;
            }
        }

        else if(state == State.falling)
        {
            if(coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }

        else if(state == State.hurt)
        {
            if(Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }


        else if (state == State.death)
        {
            
        }

        else if(Mathf.Abs(rb.velocity.x) > 2f)
        {
            //Moving
            state = State.running;
        }

        else
        {
            state = State.idle;
        }
    }

    private void Footstep()
    {
        footstep.Play();
    }

    private IEnumerator ResetPower()
    {
        yield return new WaitForSeconds(3);
        jumpForce = 25f;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    private IEnumerator DeathJump()
    {
        yield return new WaitForSeconds(1);
        GetComponent<Collider2D>().isTrigger = true;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(gameOver);
    }
}
