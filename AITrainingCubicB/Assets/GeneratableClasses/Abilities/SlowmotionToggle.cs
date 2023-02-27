using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowmotionToggle : Ability
{
    private GameObject PlayerObject;

    public void Activate(GameObject Player)
    {
        PlayerObject = Player;

        PlayerObject.TryGetComponent(out PlayerMovement playerMovement);

        playerMovement.Invoke("TriggerSlowMotion", 0);
        
    }
}
