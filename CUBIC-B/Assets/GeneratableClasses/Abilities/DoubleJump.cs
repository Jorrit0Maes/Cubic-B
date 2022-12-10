using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class DoubleJump : Ability
{
    private GameObject PlayerObject;


    public void Activate(GameObject Player)
    {
        PlayerObject = Player;
        PlayerObject.TryGetComponent(out PlayerMovement playerMovement);
        playerMovement.DoubleJumpIsActive = true;
        Timer Timer = new Timer();
        Timer.Elapsed += Task;
        Timer.Interval = 7500;
        Timer.Start();

    }

    public void Task(object source, ElapsedEventArgs e)
    {
        if (PlayerObject.TryGetComponent(out PlayerMovement playerMovement))
        {
            playerMovement.DoubleJumpIsActive = false;
        }
    }

}
