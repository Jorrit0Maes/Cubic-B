using Google.Protobuf.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleMovement : MonoBehaviour
{
    public float speed;
    public List<GameObject> Players;
    private Vector2 originalPosition;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //if the object is a certain distance from the player start moving
        if (Players.Find(e => Math.Abs(transform.localPosition.x - e.transform.localPosition.x) < 15) != null)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new(-speed, 0);
        }
        //als de originele spwan niet meer zichtbaar terug daar plaatsen
        else if (Players.Find(e => Math.Abs(originalPosition.x - e.transform.localPosition.x) < 12) == null)
        {
            transform.localPosition = originalPosition;
        }


    }
}
