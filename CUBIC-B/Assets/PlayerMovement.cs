using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using UnityEngine.WSA;
using System.Threading;

public class PlayerMovement : MonoBehaviour
{
    public float Speed;
    public float Jump;
    private float Move { get; set; }
    protected Rigidbody2D rb { get; set; }
    public bool IsOnFloor { get; set; }
    public Animator animator { get; set; }
    private bool FacingRight { get; set; }
    private bool IsJumping { get; set; }
    public bool DoubleJumpIsActive;
    public int jumpcount { get; set; }
    private float backUpSpeed { get; set; }
    private bool doResetTime = false;
    // Start is called before the first frame update

    private void Awake()
    {
        animator= gameObject.GetComponent<Animator>(); 
        backUpSpeed =  Speed;
        rb = GetComponent<Rigidbody2D>();
        DoubleJumpIsActive = true;
        IsJumping = false;
        FacingRight = true;
        IsOnFloor = true;
        jumpcount = 1;

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move = Input.GetAxis("Horizontal") * Speed;

        rb.velocity = new Vector2(Move, rb.velocity.y);

        if (Move < 0 && FacingRight)
        {
            
            Flip();
        }
        else if (Move > 0 && !FacingRight)
        {
            Flip();
        }

        animator.SetBool("InMotion", (Mathf.Abs(Move) > 0));
        jumpFunction();

        animator.SetFloat("verticalSpeed", rb.velocity.y);

        if (doResetTime)
        {
            ResetTimescale();
        }
    }


    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            IsOnFloor  = true;
            animator.SetBool("onGround", IsOnFloor);
            if (IsJumping)
            {
                IsJumping = false;
            }
            jumpcount = 1;
        }
    }

    protected void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            IsOnFloor = false;
            animator.SetBool("onGround", IsOnFloor);
        }

    }

    protected virtual void jumpFunction()
    {
        if (!DoubleJumpIsActive)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && IsOnFloor)
            {
                rb.AddForce(new Vector2(rb.velocity.x, Jump));
                IsJumping = true;

            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && (!IsJumping || jumpcount == 2))
            {

                if (!IsJumping)
                {
                    rb.AddForce(new Vector2(rb.velocity.x, Jump));
                }
                else
                {
                    rb.AddRelativeForce(new Vector2(rb.velocity.x, Jump + (rb.velocity.y)));

                }

                Debug.Log("jump pressed");
                jumpcount++;

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
        timer.Interval = 3000;
        timer.Start();
    }

    public void setToResetTime(object source, ElapsedEventArgs e)
    {
        doResetTime= true;
    }

    public void ResetTimescale()
    {
        Time.timeScale = 1;
        doResetTime= false;
    }

    public void triggerIncreasedSpeed()
    {
        Speed = 25;
        System.Timers.Timer timer = new();
        timer.Interval = 3000;
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
        timer.Interval = 3000;
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
        Time.timeScale = 1f;
        DoubleJumpIsActive = false;
    }

}
