using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class DoubleJump : Ability
{
    private GameObject PlayerObject;


    public void Activate(GameObject Player)
    {
        Player.TryGetComponent(out PlayerMovement playerMovement);
        playerMovement.Invoke("triggerDoubleJump", 0);
        

    }

    public void Task(object source, ElapsedEventArgs e)
    {
        PlayerObject.TryGetComponent(out PlayerMovement playerMovement);
        playerMovement.Invoke("resetDoubleJump", 0);

    }

}
