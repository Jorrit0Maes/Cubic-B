using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class SpeedBoost : Ability
{
	private GameObject PlayerObject;
	public float IncreasedSpeed = 25;
	private float NormalSpeed;

	public void Activate(GameObject Player)
	{
		PlayerObject = Player;
		//get and increase movement speed of the player
		PlayerObject.TryGetComponent(out PlayerMovement playerMovement);
		playerMovement.Invoke("triggerIncreasedSpeed", 0);

        
	}


	public void Task(object source ,ElapsedEventArgs e)
	{
		PlayerObject.TryGetComponent(out PlayerMovement playerMovement);
        playerMovement.Invoke("resetSpeed", 0);
    }
}
