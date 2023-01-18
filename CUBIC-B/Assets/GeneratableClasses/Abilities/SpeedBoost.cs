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
		PlayerObject.TryGetComponent(out PlayerMovement PlayermovemtScript);
		NormalSpeed = PlayermovemtScript.Speed;
        PlayermovemtScript.Speed = IncreasedSpeed;
		// set a timer and task to take away the increased movementspeed of th eplayer
        Timer Timer = new Timer();
		Timer.Elapsed += Task;
		Timer.Interval = 30000;
		Timer.Start();


        
	}


	public void Task(object source ,ElapsedEventArgs e)
	{
        if (PlayerObject.TryGetComponent(out PlayerMovement PlayersMovementScript)){
            PlayersMovementScript.Speed = NormalSpeed;
        }
    }
}
