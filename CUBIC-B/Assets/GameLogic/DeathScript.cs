using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScript : MonoBehaviour
{
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject player = collision.gameObject;
        if (player.CompareTag("Player") || player.CompareTag("Machine"))
        {
            if (player.CompareTag("Player"))
            {
                audioSource.Play();
            }
            player.GetComponent<Animator>().SetBool("Die", true);
        }
    }
}
