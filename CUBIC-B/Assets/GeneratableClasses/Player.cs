using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : InteractableObject
{
    public int Health { get; set; }
    public int Speed { get; set; }
    public float Jump { get; set; }

    private GameObject PlayerObject;

    public Player(Vector2 startPoint, Vector2 endPoint, int Health, int Speed, float Jump )
    {
        this.startPoint = startPoint;   
        this.endPoint = endPoint;
        //this.sprite
        this.Jump = Jump;
        this.Speed = Speed;
        this.Health = Health;


    }

    public Player(Vector2 startPoint, Vector2 endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        //this.sprite
        this.Jump = 500;
        this.Speed = 7;
        this.Health = 100;

        
    }
}
