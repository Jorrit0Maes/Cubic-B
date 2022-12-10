using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using System.Timers;


public class PlayerMovement : MonoBehaviour
{
    public float Speed { get; set; }
    public float Jump { get; set; }
    private float Move;
    protected Rigidbody2D rb;
    public bool IsOnFloor = true;
    public Animator animator;
    private bool FacingRight = true;
    private bool IsJumping = false;
    public bool DoubleJumpIsActive = false;
    public int jumpcount = 1;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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

        animator.SetFloat("PlayerSpeed", Mathf.Abs(Move));
        jumpFunction();
    }

    
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            IsOnFloor  = true;
            animator.SetBool("isOnGround", IsOnFloor);
            if (IsJumping)
            {
                animator.SetBool("isJumping", false);
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
            animator.SetBool("isOnGround", IsOnFloor);
        }

    }

    protected virtual void jumpFunction()
    {
        if (!DoubleJumpIsActive)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && IsOnFloor)
            {
                animator.SetBool("isJumping", true);
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
        Timer Timer = new();
        Timer.Elapsed += ResetTimescale;
        Timer.Interval = 7500;
        Timer.Start();
    }

    public void ResetTimescale(object source, ElapsedEventArgs e)
    {
        Time.timeScale = 1f;
    }

}
