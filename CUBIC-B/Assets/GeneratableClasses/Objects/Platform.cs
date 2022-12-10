using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform: InteractableObject
{
    public Platform(Vector2 startPoint, Vector2 endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        //TODO : create th esprite with the correct png

    }
}
