using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleMovement : MonoBehaviour
{
    public float speed;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if the object is a certain distance from the player start moving
        if (transform.localPosition.x - player.transform.localPosition.x < 20)
        {
            // if the distance is negative enough so that it would be off screen we destroy the object to safe computing power
            if (transform.localPosition.x - player.transform.localPosition.x < -15)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
            gameObject.GetComponent<Rigidbody2D>().velocity = new(-speed, 0);
        }


    }
}
