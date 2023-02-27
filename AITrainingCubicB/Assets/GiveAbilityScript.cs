using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Unity.VisualScripting;
using UnityEngine;

public class GiveAbilityScript : MonoBehaviour
{
    public Ability ability { get; set; }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //eerst de active abilities uit zetten
            collision.gameObject.GetComponent<PlayerMovement>().Invoke("cancelAllAbilities",0);
            ability.Activate(collision.gameObject);
            gameObject.SetActive(false);


            System.Timers.Timer timer = new();
            timer.Interval = 7000;
            timer.Elapsed += resetAbility;
            timer.AutoReset = false;
            timer.Start();

        }
    }

    public void resetAbility(object source, ElapsedEventArgs e)
    {
        gameObject.SetActive(true);


    }
}
