using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform: InteractableObject
{
    public Platform(Vector2 startPoint, Vector2 endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
    }

    public override Vector2 endPoint { get; set;}
    public override Vector2 origin { get => new Vector2(startPoint.x + (endPoint.x - startPoint.x) / 2, startPoint.y + (endPoint.y - startPoint.y) / 2); set => throw new System.NotImplementedException(); }
}
