using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowmotionToggle : Ability
{
    private GameObject PlayerObject;

    public void Activate(GameObject Player)
    {
        if (Player.CompareTag("Player"))
        {
            PlayerObject = Player;
            PlayerObject.TryGetComponent(out PlayerMovement playerMovement);
            playerMovement.Invoke("TriggerSlowMotion", 0);
        }
        else if (Player.CompareTag("Machine"))
        {
            PlayerObject = Player;
            PlayerObject.TryGetComponent(out MLMovement mlMovement);
            mlMovement.Invoke("TriggerSlowMotion", 0);
        }

    }
}
