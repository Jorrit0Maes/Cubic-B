using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Obstacle
{
    public override Vector2 endPoint { get => new(startPoint.x + length, startPoint.y + heigth); set => throw new System.NotImplementedException(); }
    public override Vector2 origin { get => new(this.startPoint.x + (this.endPoint.x - this.startPoint.x) / 2, this.endPoint.y + (this.startPoint.y - this.endPoint.y) / 2); set => throw new System.NotImplementedException(); }

    public Box()
    {

    }
    public Box(Vector2 startPoint, float length, float heigth)
    {
        this.startPoint = startPoint;
        this.length = length;
        this.heigth = heigth;

    }

  
}
