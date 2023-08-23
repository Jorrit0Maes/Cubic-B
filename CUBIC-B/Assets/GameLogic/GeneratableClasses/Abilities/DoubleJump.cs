using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class DoubleJump : Ability
{
    private GameObject PlayerObject;


    public void Activate(GameObject Player)
    {

        if (Player.CompareTag("Player"))
        {
            PlayerObject = Player;
            PlayerObject.TryGetComponent(out PlayerMovement playerMovement);
            playerMovement?.Invoke("triggerDoubleJump", 0);
        }
        else if (Player.CompareTag("Machine"))
        {
            PlayerObject = Player;
            PlayerObject.TryGetComponent(out MLMovement mlMovement);
            mlMovement?.Invoke("triggerDoubleJump", 0);
        }

    }

    public void Task(object source, ElapsedEventArgs e)
    {
        PlayerObject.TryGetComponent(out PlayerMovement playerMovement);
        playerMovement.Invoke("resetDoubleJump", 0);

    }

}
