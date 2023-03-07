using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract  class InteractableObject 
{
    public Transform myObject { get; set; }
    public Vector2 startPoint { get; set; }
    abstract public Vector2 endPoint { get; set; }
    abstract public Vector2 origin { get; set; }
    public float length { get; set; }
    public float heigth { get; set; }
}
