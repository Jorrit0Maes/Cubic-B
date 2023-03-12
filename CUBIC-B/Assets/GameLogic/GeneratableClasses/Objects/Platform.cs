using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform: InteractableObject
{
    public Platform(Vector2 startPoint)
    {
        this.startPoint = startPoint;
    }

    public override Vector2 endPoint { get => new Vector2(startPoint.x +length, startPoint.y-1); set=> throw new System.NotImplementedException(); }
    public override Vector2 origin { get => new Vector2(startPoint.x + (endPoint.x - startPoint.x) / 2, startPoint.y + (endPoint.y - startPoint.y) / 2); set => throw new System.NotImplementedException(); }
}
