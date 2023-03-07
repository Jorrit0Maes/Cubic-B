using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class AbilityObject :  InteractableObject
{
    public Ability Ability { get; set; }
    public override Vector2 endPoint { get =>  new(startPoint.x + length, startPoint.y + heigth); set => endPoint = value; }
    public override Vector2 origin {get => new(startPoint.x + (endPoint.x - startPoint.x) / 2, endPoint.y + (startPoint.y - endPoint.y) / 2); set => throw new System.NotImplementedException(); }

	public AbilityObject()
	{

	}

    public AbilityObject(Ability ability, Vector2 startPoint, Vector2 endPoint)
	{
		this.Ability = ability;
		this.startPoint = startPoint;
		this.endPoint = endPoint;		
    }




}
