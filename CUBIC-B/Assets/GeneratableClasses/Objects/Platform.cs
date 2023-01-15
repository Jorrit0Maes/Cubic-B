using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform: InteractableObject
{
    public Platform(Vector2 startPoint, Vector2 endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;

        this.origin = new Vector2(startPoint.x + (endPoint.x - startPoint.x) /2, startPoint.y + (endPoint.y - startPoint.y) / 2);
    }


    
}
