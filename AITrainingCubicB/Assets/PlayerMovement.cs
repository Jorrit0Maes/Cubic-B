using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using UnityEngine.WSA;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public class PlayerMovement : Agent
{
    public float Speed;
    public float Jump;
    private float Move { get; set; }
    protected Rigidbody2D rb { get; set; }
    public bool IsOnFloor;
    private bool FacingRight { get; set; }
    private bool IsJumping { get; set; }
    public bool DoubleJumpIsActive;
    public int jumpcount { get; set; }
    private float backUpSpeed { get; set; }
    private bool doResetTime = false;
    public float durationAbilities;
    public float timeScaling ;
    public float increasedSpeed ;
    public float activeAbility;
    public bool reset = false;
    public bool Died = false;
    // Start is called before the first frame update

    public Transform Target;
    float shortestDistanceToEnd = 0;
    public int episodeCounter = 0;
    public float curentReward = 0;
    public float lastEpisodeReward = 0;
    public Transform level;
    public Transform SpawnSquare;
    private Timer timer = new ();
    public float velocity;

    private bool onRightWall = false;
    private bool onLeftWall = false;
    private bool sliding = false;
    public PhysicsMaterial2D physicsMaterial;
    public string lastWall = "none";


    void Start()
    {
        backUpSpeed = Speed;
        rb = GetComponent<Rigidbody2D>();
        DoubleJumpIsActive = false;
        IsJumping = false;
        FacingRight = true;
        IsOnFloor = true;
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
        reset= true;
    }

    public override void OnEpisodeBegin()
    {
        cancelAllAbilities();
        timer.Stop();
        timer.Start();
        for (int i = 0; i < level.childCount; i++)
        {
            if (level.GetChild(i).name != "Finish")
            {
                Destroy(level.GetChild(i).gameObject, 0);
            }
        }

        SpawnSquare.GetComponent<Level>().Invoke("spawns", 0);

        this.rb.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0, 2.55f, 0);
        reset = false;
        Died =false;
        shortestDistanceToEnd= 0;
        activeAbility= 0;
        episodeCounter++;

        lastEpisodeReward = curentReward;
        curentReward= 0;
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
        if (Died)
        {
            AddReward(-10f);
            Died = false;
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
            AddReward(50f);
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
            IsOnFloor  = true;
            if (IsJumping)
            {
                IsJumping = false;
            }
            jumpcount = 1;
           
        }
        else if (other.gameObject.CompareTag("DeathBox"))
        {
            Died= true;
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
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Obstacle") && rb.velocity.y > 0)
        {
            IsOnFloor = false;
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
        activeAbility = 2;
    }
    public void resetSpeed(object source, ElapsedEventArgs e)
    {
        Speed= backUpSpeed;
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
        resetTimescale();
        DoubleJumpIsActive = false;
        activeAbility = 0;
    }


}
