using System.Collections;
using System.Timers;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float Speed;
    public float Jump;
    private float Move { get; set; }
    protected Rigidbody2D rb { get; set; }
    public bool IsOnFloor { get; set; }
    public Animator animator { get; set; }
    private bool FacingRight { get; set; }
    public bool DoubleJumpIsActive;
    public int jumpcount { get; set; }
    private float backUpSpeed { get; set; }
    private bool doResetTime = false;
    public float durationAbilities;
    public float timeScaling ;
    public float increasedSpeed ;
    public Transform AI;

    //WallJump
    private bool onRightWall = false;
    private bool onLeftWall = false;
    private bool sliding = false;
    public PhysicsMaterial2D physicsMaterial;
    public string lastWall = "none";

    //Death animation
    private AudioSource audioSource;
    public bool dead;
    public GameObject spawn;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>(); 
        backUpSpeed =  Speed;
        rb = GetComponent<Rigidbody2D>();
        DoubleJumpIsActive = false;
        FacingRight = true;
        IsOnFloor = true;
        jumpcount = 1;

    }
    void Start()
    {
        Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), AI.GetComponent<BoxCollider2D>());
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Move = Input.GetAxis("Horizontal") * Speed;

        rb.velocity = new Vector2(Move, rb.velocity.y);
        
        if (!animator.GetBool("Die") && rb.bodyType == RigidbodyType2D.Static)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        if (Move < 0 && FacingRight)
        {
            
            Flip();
        }
        else if (Move > 0 && !FacingRight)
        {
            Flip();
        }

        animator.SetBool("inMotion", (Mathf.Abs(Move) > 0));
        jumpFunction();

        animator.SetFloat("verticalSpeed", rb.velocity.y);

        if (doResetTime)
        {
            resetTimescale();
        }
    }


    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if ((other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Obstacle")) && rb.velocity.y == 0)
        {
            IsOnFloor = true;
            animator.SetBool("onGround", IsOnFloor);
            jumpcount = 1;
            lastWall = "ground";
            rb.sharedMaterial = null;
        }
        else if (other.gameObject.CompareTag("RightWall"))
        {
            jumpcount = 1;
            //breek al lopende coroutines af
            StopAllCoroutines();
            onRightWall = true;
            //start nieuwe coroutine om de tijd van de player op deze muur te beperken
            StartCoroutine(startSliding(rb, 3, "right"));
            //als men van de grond of linkse muur komt glijden we niet naar beneden
            if (lastWall != "right")
            {
                rb.sharedMaterial = null;

            }//als we van de rechtse muur zelf komen glijden we naar beneden
            else
            {
                rb.sharedMaterial = physicsMaterial;
            }
        }
        else if (other.gameObject.CompareTag("LeftWall"))
        {
            jumpcount = 1;
            //breek al lopende coroutines af
            StopAllCoroutines();
            onLeftWall = true;
            //start nieuwe coroutine om de tijd van de player op deze muur te beperken
            StartCoroutine(startSliding(rb, 3, "left"));
            //als men van de grond of rechtse muur komt glijden we niet naar beneden
            if (lastWall != "left")
            {
                rb.sharedMaterial = null;
            }//als we van de linkse muur zelf komen glijden we naar beneden
            else
            {
                rb.sharedMaterial = physicsMaterial;
            }
        }
    }
    private IEnumerator startSliding(Rigidbody2D teLatenGlijden, float delay, string name)
    {
        yield return new WaitForSeconds(delay);
        if (name == "right")
        {
            if (onRightWall)
            {
                teLatenGlijden.sharedMaterial = physicsMaterial;
                sliding = true;
            }
        }
        else if (name == "left")
        {
            if (onLeftWall)
            {
                teLatenGlijden.sharedMaterial = physicsMaterial;
                sliding = true;
            }
        }

    }

    protected void OnCollisionExit2D(Collision2D other)
    {   // check of we can de grond of een obstakel weg gaan indien we weg gaan van een obstakel moeten we een verticale snelheid hebben anders kan dit ook zijn als men tegen een obstakel loopt als men nog op de grond is
        if (other.gameObject.CompareTag("Ground") || (other.gameObject.CompareTag("Obstacle") && rb.velocity.y != 0))
        {
            IsOnFloor = false;
            animator.SetBool("onGround", IsOnFloor);
        }
        else if (other.gameObject.CompareTag("RightWall"))
        {
            onRightWall = false;
            sliding = false;
            lastWall = "right";
        }
        else if (other.gameObject.CompareTag("LeftWall"))
        {
            onLeftWall = false;
            sliding = false;
            lastWall = "left";
        }

    }

    protected virtual void jumpFunction()
    {
        if (!DoubleJumpIsActive)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) )
            {
                if (IsOnFloor)
                {
                    rb.AddForce(new Vector2(rb.velocity.x, Jump));
                }
                else if (onRightWall && !sliding && lastWall != "right")
                {
                    rb.sharedMaterial = physicsMaterial;
                    animator.SetBool("isJumping", true);
                    rb.AddForce(new Vector2(-700, 500));

                }
                else if (onLeftWall && !sliding && lastWall != "left")
                {
                    rb.sharedMaterial = physicsMaterial;
                    animator.SetBool("isJumping", true);
                    rb.AddForce(new Vector2(700, 500));
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {

                if (IsOnFloor)
                {
                    rb.AddForce(new Vector2(rb.velocity.x, Jump));
                    jumpcount++;
                }
                else if (onRightWall && !sliding && lastWall != "right")
                {
                    rb.sharedMaterial = physicsMaterial;
                    animator.SetBool("isJumping", true);
                    rb.AddForce(new Vector2(-700, 500));
                    jumpcount++;
                }
                else if (onLeftWall && !sliding && lastWall != "left")
                {
                    rb.sharedMaterial = physicsMaterial;
                    animator.SetBool("isJumping", true);
                    rb.AddForce(new Vector2(700, 500));
                    jumpcount++;
                }
                else if (jumpcount == 2)
                {
                    rb.AddRelativeForce(new Vector2(rb.velocity.x, Jump + (rb.velocity.y)));
                    jumpcount++;
                }

            }
        }
       
    }

    private void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void TriggerSlowMotion()
    {
        Time.timeScale = 0.7f;
        System.Timers.Timer timer = new();
        timer.Elapsed += setToResetTime;
        timer.Interval = durationAbilities;
        timer.Start();
    }

    public void setToResetTime(object source, ElapsedEventArgs e)
    {
        doResetTime= true;
    }

    public void resetTimescale()
    {
        Time.timeScale = 1;
        doResetTime= false;
    }

    public void triggerIncreasedSpeed()
    {
        Speed = increasedSpeed;
        System.Timers.Timer timer = new();
        timer.Interval = durationAbilities;
        timer.Elapsed += resetSpeed;
        timer.AutoReset = false;
        timer.Start();
    }
    public void resetSpeed(object source, ElapsedEventArgs e)
    {
        Speed= backUpSpeed;
    }

    public void triggerDoubleJump()
    {
        DoubleJumpIsActive = true;
        System.Timers.Timer timer = new();
        timer.Interval = durationAbilities;
        timer.Elapsed += resetDoubleJump;
        timer.AutoReset = false;
        timer.Start();
    }
    public void resetDoubleJump(object source, ElapsedEventArgs e)
    {
        DoubleJumpIsActive = false;
    }


    private void cancelAllAbilities()
    {
        Speed = backUpSpeed;
        resetTimescale();
        DoubleJumpIsActive = false;
    }

    private void playAudio()
    {
        audioSource.Play();
    }

    private void freeze()
    {
        rb.bodyType = RigidbodyType2D.Static;
    }

    private void unFreeze()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void respawn()
    {
        animator.SetBool("Die", false);
        gameObject.transform.position = spawn.transform.position;
        unFreeze();
    }
}
