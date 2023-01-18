using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missle : DeadlyObject
{
    public int speed { get; set; }
    public override Vector2 endPoint { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public override Vector2 origin { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public Missle(Vector2 startPoint, Vector2 endPoint)
	{
		this.startPoint = startPoint;
		this.endPoint = endPoint;
		/*
		 * this.sprite =
		 * this.speed = 
		 * this.collider =
		 * */
	}
}
