using UnityEngine;
using System.Timers;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using System.Collections;

public class MLMovement : Agent
{
    public float Speed;
    public float Jump;
    private float Move { get; set; }
    protected Rigidbody2D rb { get; set; }
    public bool IsOnFloor { get; set; }
    private bool FacingRight { get; set; }
    private bool IsJumping { get; set; }
    public bool DoubleJumpIsActive;
    public int jumpcount { get; set; }
    private float backUpSpeed { get; set; }
    public float durationAbilities;
    public float timeScaling;
    public float increasedSpeed;
    public float activeAbility;
    public bool reset = false;
    public Animator animator { get; set; }
    public Transform spawn;


    public Transform Target;
    float shortestDistanceToEnd = 0;
    public int episodeCounter = 0;
    public float curentReward = 0;
    public float lastEpisodeReward = 0;
    public Transform level;
    public Transform SpawnSquare;
    System.Timers.Timer timer;
    public float velocity;

    private bool onRightWall = false;
    private bool onLeftWall = false;
    private bool sliding = false;
    public PhysicsMaterial2D physicsMaterial;
    public string lastWall = "none";


    void Start()
    {
        animator= GetComponent<Animator>();
        backUpSpeed = Speed;
        rb = GetComponent<Rigidbody2D>();
        DoubleJumpIsActive = false;
        IsJumping = false;
        FacingRight = true;
        IsOnFloor = false;
        jumpcount = 1;
        activeAbility = 0;

        timer = new();
        timer.Interval = 60000;
        timer.Elapsed += timeRanOut;
        timer.AutoReset = false;
        timer.Start();

    }
    private void timeRanOut(object sender, ElapsedEventArgs e)
    {
        reset = true;
    }
    
    void Update()
    {
        if(!animator.GetBool("Die") && rb.bodyType == RigidbodyType2D.Static){
            rb.bodyType = RigidbodyType2D.Dynamic;  
        }
        if (rb.velocity.x < 0 && FacingRight)
        {

            Flip();
        }
        else if (rb.velocity.x > 0 && !FacingRight)
        {
            Flip();
        }

        animator.SetBool("inMotion", (Mathf.Abs(rb.velocity.x) > 0));

        animator.SetFloat("verticalSpeed", rb.velocity.y);

    }
    private void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
   


    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);

        //
        sensor.AddObservation(activeAbility);

        //
        sensor.AddObservation(IsOnFloor);


    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        float movement = actionBuffers.DiscreteActions[0];
        float jump = actionBuffers.DiscreteActions[1];

        float xmove = 0;
        float jumpmove = 0;
        if (movement == 1) { xmove = -1; }
        if (movement == 2) { xmove = 1; }
        if (jump == 1) { jumpmove = 1; }


        rb.velocity = new(xmove * Speed, rb.velocity.y);

        if (jumpmove == 1)
        {
            if ((IsOnFloor && rb.velocity.y == 0 || (DoubleJumpIsActive && jumpcount == 2)))
            {
                rb.AddForce(new(rb.velocity.x, Jump));
                IsOnFloor = false;
                IsJumping = true;
                jumpcount++;
            }
            else if (onRightWall && !sliding && lastWall != "right")
            {
                rb.sharedMaterial = physicsMaterial;
                rb.AddForce(new Vector2(-700, 500));
                jumpcount++;
            }
            else if (onLeftWall && !sliding && lastWall != "left")
            {
                rb.sharedMaterial = physicsMaterial;
                rb.AddForce(new Vector2(700, 500));
                jumpcount++;
            }
        }


        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);


        // Reached target
        if (distanceToTarget < 1.42f)
        {
            AddReward(1000000f);
            curentReward = GetCumulativeReward();
            EndEpisode();

        }

        if (reset)
        {
            AddReward(-100f);
            curentReward = GetCumulativeReward();
            EndEpisode();

        }
        // reward voor korter te komen dan hij al is geweest om te vermijden dat de agent zijn reward gaat maxen door gewoon links rechts te doen
        if (shortestDistanceToEnd == 0)
        {
            shortestDistanceToEnd = distanceToTarget;
        }
        else if (shortestDistanceToEnd > distanceToTarget)
        {
            shortestDistanceToEnd = distanceToTarget;
            AddReward(5f);
        }
        else
        {
            AddReward(-1f);
        }
        /* for some reason is de velocity 5 als hij stilstaan tegen een  blok probeert voort te bewegen
        // negatieve rewward voor teruglopen of stilstaan om aan te zetten om vooruit te blijven gaan
        if (rb.velocity.x < 2)
        {
            AddReward(-1f);
        }
        */
        velocity = rb.velocity.x;
        curentReward = GetCumulativeReward();



    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if ((other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Obstacle")) && rb.velocity.y == 0)
        {
            IsOnFloor = true;
            animator.SetBool("onGround", IsOnFloor);

            if (IsJumping)
            {
                IsJumping = false;
            }
            jumpcount = 1;

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

    protected void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground") || (other.gameObject.CompareTag("Obstacle") && (other.gameObject.CompareTag("Obstacle") && rb.velocity.y != 0)))
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

    public void triggerIncreasedSpeed()
    {
        Speed = increasedSpeed;
        System.Timers.Timer timer = new();
        timer.Interval = durationAbilities;
        timer.Elapsed += resetSpeed;
        timer.AutoReset = false;
        timer.Start();
        activeAbility = 2;
    }
    public void resetSpeed(object source, ElapsedEventArgs e)
    {
        Speed = backUpSpeed;
        activeAbility = 0;

    }

    public void triggerDoubleJump()
    {
        DoubleJumpIsActive = true;
        System.Timers.Timer timer = new();
        timer.Interval = durationAbilities;
        timer.Elapsed += resetDoubleJump;
        timer.AutoReset = false;
        timer.Start();
        activeAbility = 1;
    }
    public void resetDoubleJump(object source, ElapsedEventArgs e)
    {
        DoubleJumpIsActive = false;
        activeAbility = 0;

    }


    private void cancelAllAbilities()
    {
        Speed = backUpSpeed;
        DoubleJumpIsActive = false;
        activeAbility = 0;
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
        gameObject.transform.position = spawn.position;
        unFreeze();
    }
}
