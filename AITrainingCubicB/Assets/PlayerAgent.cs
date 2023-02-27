using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.VisualScripting;

public class PlayerAgent : Agent
{

    Rigidbody2D rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
     

    }

    public Transform Target;
    public override void OnEpisodeBegin()
    {
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0, 1.55f, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.y);

        //
        sensor.AddObservation(gameObject.GetComponent<PlayerMovement>().activeAbility);

        //
        sensor.AddObservation(gameObject.GetComponent<PlayerMovement>().IsOnFloor);


    }

    public float speed = 5;
    public float jumpforce = 20;

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


        rBody.velocity = new(xmove * speed, rBody.velocity.y);

        if ((gameObject.GetComponent<PlayerMovement>().IsOnFloor || (gameObject.GetComponent<PlayerMovement>().DoubleJumpIsActive && gameObject.GetComponent<PlayerMovement>().jumpcount == 2)) && jumpmove == 1)
        {
            rBody.AddForce(new(0, jumpforce));
        }
        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            AddReward(100f);
            EndEpisode();
        }

        /*if (gameObject.GetComponent<PlayerMovement>().DIED)
        {
            AddReward(-5f);
            EndEpisode();
        }*/

        // zet negatieve reward voor stil staan om aan te zetten om in beweging te blijven
        if (rBody.velocity.x == 0)
        {
            AddReward(-3f);
        }
        // negatieve rewward voor teruglopen om aan te zetten om vooruit te blijven gaan
        if (rBody.velocity.x < 0)
        {
            AddReward(-1f);
        }
        //positieve reward voor naar het einde lopen
        if (rBody.velocity.x > 0)
        {
            AddReward(5f);
        }
    }


}