using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Obstacle
{
    public float length { get; set; }
    public float heigth { get; set; }
  
    public Box(Vector2 startPoint, float length, float heigth)
    {
        this.startPoint = startPoint;
        this.length = length;
        this.heigth = heigth;
        this.endPoint = new Vector2(startPoint.x + length,  startPoint.y+ heigth);
        this.origin = new Vector2(startPoint.x + (endPoint.x - startPoint.x) / 2, endPoint.y + (startPoint.y - endPoint.y) / 2);

    }
}
