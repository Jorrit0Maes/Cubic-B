using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missle : DeadlyObject
{
    public int speed { get; set; }

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
