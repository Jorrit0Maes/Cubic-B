using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missle : DeadlyObject
{
    public int speed { get; set; }
    public override Vector2 endPoint { get => new(startPoint.x + length, startPoint.y + heigth); set => throw new System.NotImplementedException(); }
    public override Vector2 origin { get => new(this.startPoint.x + (this.endPoint.x - this.startPoint.x) / 2, this.endPoint.y + (this.startPoint.y - this.endPoint.y) / 2); set => throw new System.NotImplementedException(); }

    public Missle(Vector2 startPoint, Vector2 endPoint)
	{
		this.startPoint = startPoint;
		this.endPoint = endPoint;
	}

	public Missle() { }
}
