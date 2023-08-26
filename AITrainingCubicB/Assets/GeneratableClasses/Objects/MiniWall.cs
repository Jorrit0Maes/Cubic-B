using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniWall : InteractableObject
{
    public override Vector2 endPoint { get => new(startPoint.x + length, startPoint.y + height); set => throw new System.NotImplementedException(); }
    public override Vector2 origin { get => new(this.startPoint.x + (this.endPoint.x - this.startPoint.x) / 2, this.endPoint.y + (this.startPoint.y - this.endPoint.y) / 2); set => throw new System.NotImplementedException(); }

    public MiniWall()
    {
        this.length = 0.2f;
        this.height = 1f;
    }

}
